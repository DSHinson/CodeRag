using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.List
{
    internal sealed class WindowsListComponent : IListComponent
    {
        private List<string> _items = new();
        private int _selectedIndex;
        private int _scrollOffset;

        private const string InstructionsText = "Up/Down to navigate, Tab to switch focus";

        public bool IsActive { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; } = 60;
        public int Height { get; set; } = 5;

        public int? SelectedIndex => _items.Count == 0 ? null : _selectedIndex;

        public string? SelectedItem => SelectedIndex.HasValue ? _items[SelectedIndex.Value] : null;

        public void SetItems(IReadOnlyList<string> items)
        {
            _items = items.ToList();
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _items.Count - 1));
            _scrollOffset = 0;
        }

        public void MoveUp()
        {
            if (_items.Count == 0)
            {
                return; 
            }
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            EnsureVisible();
        }

        public void MoveDown()
        {
            if (_items.Count == 0) 
            { 
                return; 
            }
            _selectedIndex = Math.Min(_items.Count - 1, _selectedIndex + 1);
            EnsureVisible();
        }

        private void EnsureVisible()
        {
            if (_selectedIndex < _scrollOffset)
            {
                _scrollOffset = _selectedIndex;
            }
            else if (_selectedIndex >= _scrollOffset + Height)
            {
                _scrollOffset = _selectedIndex - Height + 1;
            }
        }

        public void Render()
        {
            for (int row = 0; row < Height; row++)
            {
                int itemIndex = _scrollOffset + row;
                Console.SetCursorPosition(Left, Top + row);

                if (itemIndex >= _items.Count)
                {
                    Console.Write(new string(' ', Width));
                    continue;
                }

                bool isSelected = IsActive && itemIndex == _selectedIndex;
                string text = _items[itemIndex];
                string line = (isSelected ? "> " : "  ") + text;

                if (line.Length > Width)
                {
                    line = line[..Width];
                }
                else
                {
                    line = line.PadRight(Width);
                }

                if (isSelected)
                {
                    var prevBg = Console.BackgroundColor;
                    var prevFg = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(line);
                    Console.BackgroundColor = prevBg;
                    Console.ForegroundColor = prevFg;
                }
                else
                {
                    Console.Write(line);
                }
            }

            RenderInstructions();
        }

        private void RenderInstructions()
        {
            Console.SetCursorPosition(Left, Top + Height);

            if (IsActive)
            {
                var prevFg = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(InstructionsText.PadRight(Width)[..Width]);
                Console.ForegroundColor = prevFg;
            }
            else
            {
                Console.Write(new string(' ', Width));
            }
        }

        public void Clear()
        {
            for (int row = 0; row < Height; row++)
            {
                Console.SetCursorPosition(Left, Top + row);
                Console.Write(new string(' ', Width));
            }

            Console.SetCursorPosition(Left, Top + Height);
            Console.Write(new string(' ', Width));
        }
    }
}