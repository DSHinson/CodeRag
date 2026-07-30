namespace CodeRag.Cli.Screens.Components.WelcomeBanner
{
    internal sealed class WindowsWelcomeBanner : IWelcomeBanner
    {
        public void Show()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   ____          _      ____
  / ___|___   __| | ___|  _ \ __ _  __ _
 | |   / _ \ / _` |/ _ \ |_) / _` |/ _` |
 | |__| (_) | (_| |  __/  _ < (_| | (_| |
  \____\___/ \__,_|\___|_| \_\__,_|\__, |
                                    |___/
");
            Console.ResetColor();
            Console.WriteLine("  Local-first code intelligence for MCP clients");
            Console.WriteLine();
        }
    }
}
