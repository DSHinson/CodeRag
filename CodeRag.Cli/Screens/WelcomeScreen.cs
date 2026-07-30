using CodeRag.Cli.Events;
using CodeRag.Cli.Screens.Components.Buttons;
using CodeRag.Cli.Screens.Components.StatusBar;
using CodeRag.Cli.Screens.Components.WelcomeBanner;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using static CodeRag.Cli.Screens.Defs;

namespace CodeRag.Cli.Screens
{
    internal class WelcomeScreen : IScreen
    {
        private IWelcomeBanner? _welcomeMessage;
        private IEventBus? _eventBus;
        private List<IButtonComponent> btnList;
        private IStatusBar? _statusBar;
        public WelcomeScreen(IWelcomeBanner welcomeBanner, IStatusBar statusBar, IEventBus eventBus, IButtonComponent btnAddRepo, IButtonComponent btnNext)
        {
            Console.CursorVisible = false;
            _welcomeMessage = welcomeBanner ?? throw new ArgumentNullException(nameof(welcomeBanner));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _statusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));

            btnAddRepo.Label = "Add Repository";
            btnAddRepo.IsActive = false;

            btnNext.Label = "Next";
            btnNext.IsActive = false;

            btnList = new List<IButtonComponent>();
            btnList.Add(btnAddRepo);
            btnList.Add(btnNext);
        }
        public ScreenIds ScreenId => ScreenIds.WelcomeScreen;

        public Result HandleEvent(InputKeyEvent inputKeyEvent)
        {  
            if (inputKeyEvent.Key.Key == ConsoleKey.Tab)
            {
                int currentIndex = btnList.FindIndex(b => b.IsActive);
                int nextIndex = (currentIndex + 1) % btnList.Count;

                foreach (var btn in btnList)
                {
                    btn.IsActive = false;
                }

                btnList[nextIndex].IsActive = true;

                foreach (var btn in btnList)
                {
                    btn.Clear();
                    btn.Render();
                }
            }

            if (inputKeyEvent.Key.Key == ConsoleKey.Enter)
            {
                var activeButton = btnList.Find(b => b.IsActive);
                if (activeButton != null)
                {
                    if(activeButton.Label == "Add Repository")
                    {
                        var folderPath = PickFolder();
                        if (!string.IsNullOrEmpty(folderPath))
                        {
                            // Handle adding repository logic here
                        }
                    }
                    else if (activeButton.Label == "Next")
                    {
                        _eventBus?.Publish(new NavigateEvent(ScreenIds.MainMenuScreen));
                    }
                }
            }

            return ResultHelpers.ToResult();
        }

        public Result Initialize()
        {
            return ResultHelpers.ToResult();
        }

        public Result Render()
        {
            Console.Clear();
            _welcomeMessage!.Show();
            LayoutButtons();

            foreach (var btn in btnList)
            {
                btn.Render();
            }
            _statusBar!.SetStatus("Ready...");
            return ResultHelpers.ToResult();
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
            btnList = null;
            _eventBus = null;
        }
    }
}
