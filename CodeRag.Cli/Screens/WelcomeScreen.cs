using CodeRag.Cli.Events;
using CodeRag.Cli.Screens.Components.Buttons;
using CodeRag.Cli.Screens.Components.Confirmation;
using CodeRag.Cli.Screens.Components.List;
using CodeRag.Cli.Screens.Components.StatusBar;
using CodeRag.Cli.Screens.Components.WelcomeBanner;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using CodeRag.db.Store;
using static CodeRag.Cli.Screens.ScreenDefs;

namespace CodeRag.Cli.Screens
{
    internal class WelcomeScreen : IScreen
    {
        private IWelcomeBanner? _welcomeMessage;
        private IEventBus? _eventBus;
        private List<IButtonComponent> btnList;
        private IStatusBar? _statusBar;
        private ISqlStore _sqlStore;
        private IListComponent _listComponent;
        private IConfirmationComponent _confirmation;

        private bool _confirmationActive;
        private string? _pendingDeletePath;

        // 0 = list, 1..N = buttons
        private int _focusedIndex;

        public WelcomeScreen(
            IWelcomeBanner welcomeBanner,
            IStatusBar statusBar,
            IEventBus eventBus,
            IButtonComponent btnAddRepo,
            IButtonComponent btnNext,
            ISqlStore sqlStore,
            IListComponent listComponent,
            IConfirmationComponent confirmation)
        {
            _welcomeMessage = welcomeBanner ?? throw new ArgumentNullException(nameof(welcomeBanner));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _statusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));
            _sqlStore = sqlStore ?? throw new ArgumentNullException(nameof(sqlStore));
            _listComponent = listComponent ?? throw new ArgumentNullException(nameof(listComponent));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));

            btnAddRepo.Label = "Add Repository";
            btnAddRepo.IsActive = false;

            btnNext.Label = "Next";
            btnNext.IsActive = false;

            btnList = new List<IButtonComponent> { btnAddRepo, btnNext };
        }

        public ScreenIds ScreenId => ScreenIds.WelcomeScreen;

        public async Task<Result> HandleEvent(InputKeyEvent inputKeyEvent)
        {
            // A pending confirmation owns all input until it resolves
            if (_confirmationActive)
            {
                return await HandleConfirmationInputAsync(inputKeyEvent);
            }

            var key = inputKeyEvent.Key.Key;
            int totalFocusTargets = 1 + btnList.Count; // list + buttons

            if (key == ConsoleKey.Tab)
            {
                _focusedIndex = (_focusedIndex + 1) % totalFocusTargets;
                UpdateFocus();
                RedrawFocusables();
                return ResultHelpers.ToResult();
            }

            if (_focusedIndex == 0)
            {
                // list is focused: arrows move selection, Delete asks for confirmation
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        _listComponent.MoveUp();
                        break;

                    case ConsoleKey.DownArrow:
                        _listComponent.MoveDown();
                        break;

                    case ConsoleKey.Enter:
                        break;

                    case ConsoleKey.Delete:
                        if (_listComponent.SelectedItem is string path)
                        {
                            _pendingDeletePath = path;
                            _confirmationActive = true;
                            LayoutConfirmation();
                            _confirmation.Prompt($"Delete '{path}'?");

                            // return early so the list doesn't redraw over the prompt
                            return ResultHelpers.ToResult();
                        }
                        break;
                }

                _listComponent.Render();
                return ResultHelpers.ToResult();
            }

            if (key == ConsoleKey.Enter)
            {
                var activeButton = btnList[_focusedIndex - 1];

                switch (activeButton.Label)
                {
                    case "Add Repository":
                        await AddRepositoryAsync();
                        break;

                    case "Next":
                        _eventBus?.Publish(new NavigateEvent(ScreenIds.IndexingScreen));
                        break;
                }
            }

            return ResultHelpers.ToResult();
        }

        private async Task<Result> HandleConfirmationInputAsync(InputKeyEvent inputKeyEvent)
        {
            _confirmation.HandleInput(inputKeyEvent.Key);

            if (_confirmation.IsPending)
            {
                return ResultHelpers.ToResult();
            }

            bool confirmed = _confirmation.LastResult;
            string? path = _pendingDeletePath;

            _confirmationActive = false;
            _pendingDeletePath = null;
            _confirmation.Clear();

            if (confirmed && path is not null)
            {
                await DeleteRepoAsync(path);
            }

            RedrawFocusables();
            return ResultHelpers.ToResult();
        }

        private async Task AddRepositoryAsync()
        {
            var folderPath = PickFolder();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            var insertResult = await _sqlStore.ExecuteAsync(
                "INSERT INTO repositories (path, added_at) VALUES (@Path, @AddedAt)",
                new { Path = folderPath, AddedAt = DateTime.UtcNow });

            switch (insertResult)
            {
                case Result<int>.Success:
                    await ReloadRepoListAsync();
                    _statusBar!.SetStatus($"Added {folderPath}");
                    break;

                case Result<int>.Failure failure:
                    _statusBar!.SetStatus($"Failed to add repo: {failure.FailureMessage}");
                    break;
            }

            _listComponent.Render();
        }

        private async Task DeleteRepoAsync(string path)
        {
            var deleteResult = await _sqlStore.ExecuteAsync(
                "DELETE FROM repositories WHERE path = @Path",
                new { Path = path });

            switch (deleteResult)
            {
                case Result<int>.Success:
                    await ReloadRepoListAsync();
                    _statusBar!.SetStatus($"Removed {path}");
                    break;

                case Result<int>.Failure failure:
                    _statusBar!.SetStatus($"Failed to remove repo: {failure.FailureMessage}");
                    break;
            }
        }

        public async Task<Result> Initialize()
        {
            _confirmationActive = false;
            _pendingDeletePath = null;

            var loadResult = await ReloadRepoListAsync();
            UpdateFocus();
            return loadResult;
        }

        private async Task<Result> ReloadRepoListAsync()
        {
            var result = await _sqlStore.QueryAsync<string>(
                "SELECT path FROM repositories ORDER BY added_at");

            return result switch
            {
                Result<IReadOnlyList<string>>.Success success => UpdateListAndSucceed(success.Data),
                Result<IReadOnlyList<string>>.Failure failure => failure.FailureMessage.ToFailure(),
                _ => ResultHelpers.ToResult()
            };
        }

        private Result UpdateListAndSucceed(IReadOnlyList<string> items)
        {
            _listComponent.SetItems(items);
            return ResultHelpers.ToResult();
        }

        private void UpdateFocus()
        {
            _listComponent.IsActive = _focusedIndex == 0;
            for (int i = 0; i < btnList.Count; i++)
            {
                btnList[i].IsActive = _focusedIndex == i + 1;
            }
        }

        private void RedrawFocusables()
        {
            _listComponent.Render();
            foreach (var btn in btnList)
            {
                btn.Clear();
                btn.Render();
            }
        }

        public Task<Result> Render()
        {
            Console.Clear();
            _welcomeMessage!.Show();

            LayoutList();
            LayoutButtons();

            _listComponent.Render();
            foreach (var btn in btnList)
            {
                btn.Render();
            }

            _statusBar!.SetStatus("Ready...");
            return Task.FromResult(ResultHelpers.ToResult());
        }

        private void LayoutList()
        {
            _listComponent.Left = 0;
            _listComponent.Top = 10;
            _listComponent.Width = Math.Min(Console.WindowWidth, 70);
            _listComponent.Height = Math.Max(1, Console.WindowHeight - 3 - 10 - 2);
        }

        private void LayoutButtons()
        {
            int buttonRow = Console.WindowHeight - 3;
            int left = 0;

            foreach (var btn in btnList)
            {
                btn.Left = left;
                btn.Top = buttonRow;
                left += btn.Width + 4;
            }
        }

        private void LayoutConfirmation()
        {
            _confirmation.Left = 0;
            _confirmation.Top = Console.WindowHeight - 5;
        }

        public string? PickFolder()
        {
            string? result = null;

            var thread = new Thread(() =>
            {
                using var dialog = new FolderBrowserDialog
                {
                    Description = "Select a repository to add",
                    UseDescriptionForTitle = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    result = dialog.SelectedPath;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        void IDisposable.Dispose()
        {
            _welcomeMessage = null;
            _eventBus = null;
        }
    }
}