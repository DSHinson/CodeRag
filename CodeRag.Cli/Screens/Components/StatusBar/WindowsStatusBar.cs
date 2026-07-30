using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.StatusBar
{
    internal sealed class WindowsStatusBar : IStatusBar
    {
        private string _currentText = string.Empty;

        public void SetStatus(string text)
        {
            _currentText = text;
            Render();
        }

        public void WriteLine(string content)
        {
            int bottomRow = Console.WindowHeight - 1;
            if (Console.CursorTop >= bottomRow - 1)
            {
                Console.MoveBufferArea(0, 1, Console.WindowWidth, bottomRow - 1, 0, 0);
                Console.SetCursorPosition(0, bottomRow - 2);
            }

            Console.WriteLine(content);
            Render();
        }

        private void Render()
        {
            int bottomRow = Console.WindowHeight - 1;
            var (left, top) = (Console.CursorLeft, Console.CursorTop);

            Console.SetCursorPosition(0, bottomRow);
            var prevColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(_currentText.PadRight(Console.WindowWidth - 1));
            Console.BackgroundColor = prevColor;

            Console.SetCursorPosition(left, top);
        }
    }
}
