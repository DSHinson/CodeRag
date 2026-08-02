using CodeRag.Cli.Events;
using CodeRag.Cli.Screens.Components.Buttons;
using CodeRag.Cli.Screens.Components.List;
using CodeRag.Cli.Screens.Components.StatusBar;
using CodeRag.Cli.Screens.Components.WelcomeBanner;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using CodeRag.db.Store;
using static CodeRag.Cli.Screens.Defs;

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

        // 0 = list, 1..N = buttons
        private int _focusedIndex;

        public WelcomeScreen(
            IWelcomeBanner welcomeBanner,
            IStatusBar statusBar,
            IEventBus eventBus,
            IButtonComponent btnAddRepo,
            IButtonComponent btnNext,
            ISqlStore sqlStore,
            IListComponent listComponent)
        {
            _welcomeMessage = welcomeBanner ?? throw new ArgumentNullException(nameof(welcomeBanner));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _statusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));
            _sqlStore = sqlStore ?? throw new ArgumentNullException(nameof(sqlStore));
            _listComponent = listComponent ?? throw new ArgumentNullException(nameof(listComponent));

            btnAddRepo.Label = "Add Repository";
            btnAddRepo.IsActive = false;

            btnNext.Label = "Next";
            btnNext.IsActive = false;

            btnList = new List<IButtonComponent> { btnAddRepo, btnNext };
        }

        public ScreenIds ScreenId => ScreenIds.WelcomeScreen;

        public async Task<Result> HandleEvent(InputKeyEvent inputKeyEvent)
        {
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
                // list is focused — arrows move selection
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        _listComponent.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        _listComponent.MoveDown();
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

        private async Task AddRepositoryAsync()
        {
            var folderPath = PickFolder();
            if (string.IsNullOrEmpty(folderPath))
                return;

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

        public async Task<Result> Initialize()
        {
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
                btnList[i].IsActive = _focusedIndex == i + 1;
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
                btn.Render();

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
                    result = dialog.SelectedPath;
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