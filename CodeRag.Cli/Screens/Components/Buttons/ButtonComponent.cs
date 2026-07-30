using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.Buttons
{
    internal sealed class ButtonComponent :IButtonComponent
    {
        public string Label { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Left { get; set; }
        public int Top { get; set; } = Console.CursorTop;

        public int Width => $"[ {Label} ]".Length; // active form is the longer of the two renderings



        public void Render()
        {
            Clear();
            Console.SetCursorPosition(Left, Top);

            if (IsActive)
            {
                var prevBg = Console.BackgroundColor;
                var prevFg = Console.ForegroundColor;
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"[ {Label} ]");
                Console.BackgroundColor = prevBg;
                Console.ForegroundColor = prevFg;
            }
            else
            {
                Console.Write($"  {Label}  ");
            }
        }

        public void Clear()
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(new string(' ', Width));
        }
    }
}
