namespace CodeRag.Cli.Screens.Components.StatusBar
{
    internal interface IStatusBar
    {
        void SetStatus(string text);
        void WriteLine(string content);
    }
}
