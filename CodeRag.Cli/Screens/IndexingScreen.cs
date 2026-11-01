using CodeRag.Cli.Events;
using CodeRag.Cli.Screens.Components.Buttons;
using CodeRag.Cli.Screens.Components.StatusBar;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using CodeRag.db.Store;
using CodeRag.bl.Indexing;
using static CodeRag.Cli.Screens.ScreenDefs;

namespace CodeRag.Cli.Screens
{
    internal class IndexingScreen : IScreen
    {
        private readonly ISqlStore _sqlStore;
        private readonly IRepositoryScanner _scanner;
        private readonly IEventBus _eventBus;
        private readonly IStatusBar _statusBar;
        private readonly List<IButtonComponent> _buttons;

        private List<(string Path, int FileCount)> _summary = new();
        private int _focusedIndex;

        public ScreenIds ScreenId => ScreenIds.IndexingScreen;

        public IndexingScreen(
            ISqlStore sqlStore,
            IRepositoryScanner scanner,
            IEventBus eventBus,
            IStatusBar statusBar,
            IButtonComponent btnStart,
            IButtonComponent btnBack)
        {
            _sqlStore = sqlStore;
            _scanner = scanner;
            _eventBus = eventBus;
            _statusBar = statusBar;

            btnStart.Label = "Start Indexing";
            btnStart.IsActive = true;
            btnBack.Label = "Back";
            btnBack.IsActive = false;

            _buttons = new List<IButtonComponent> { btnStart, btnBack };
        }

        public async Task<Result> Initialize()
        {
            var result = await _sqlStore.QueryAsync<string>(
                "SELECT path FROM repositories ORDER BY added_at");

            if (result is Result<IReadOnlyList<string>>.Success success)
            {
                _summary = success.Data.Select(p => (p, _scanner.CountFiles(p))).ToList();
            }
            else
            {
                _summary = new List<(string, int)>();
            }

            _focusedIndex = 0;
            UpdateFocus();

            if (result is Result<IReadOnlyList<string>>.Failure failure)
            {
                return failure.FailureMessage.ToFailure();
            }

            return ResultHelpers.ToResult();
        }

        public Task<Result> Render()
        {
            Console.Clear();

            Console.WriteLine("Ready to index the following repositories:");
            Console.WriteLine();

            foreach (var (path, fileCount) in _summary)
            {
                Console.WriteLine($"  {path}  ({fileCount} files)");
            }

            int totalFiles = _summary.Sum(s => s.FileCount);
            Console.WriteLine();
            Console.WriteLine($"Total: {_summary.Count} repositories, {totalFiles} files");

            LayoutButtons();
            foreach (var btn in _buttons)
            {
                btn.Render();
            }

            _statusBar.SetStatus("Ready...");
            return Task.FromResult(ResultHelpers.ToResult());
        }

        public Task<Result> HandleEvent(InputKeyEvent inputKeyEvent)
        {
            var key = inputKeyEvent.Key.Key;

            if (key == ConsoleKey.Tab)
            {
                _focusedIndex = (_focusedIndex + 1) % _buttons.Count;
                UpdateFocus();

                foreach (var btn in _buttons)
                {
                    btn.Clear();
                    btn.Render();
                }
            }
            else if (key == ConsoleKey.Enter)
            {
                var active = _buttons[_focusedIndex];

                if (active.Label == "Start Indexing")
                {
                    _statusBar.SetStatus("Indexing not yet implemented");
                }
                else if (active.Label == "Back")
                {
                    //TODO: Implement navigation to the previous screen
                }
            }

            return Task.FromResult(ResultHelpers.ToResult());
        }

        private void UpdateFocus()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].IsActive = i == _focusedIndex;
            }
        }

        private void LayoutButtons()
        {
            int row = Console.WindowHeight - 3;
            int left = 0;

            foreach (var btn in _buttons)
            {
                btn.Left = left;
                btn.Top = row;
                left += btn.Width + 4;
            }
        }

        void IDisposable.Dispose()
        {
        }
    }
}