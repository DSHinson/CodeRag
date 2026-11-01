using CodeRag.Cli.Screens.Components.Buttons;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.Confirmation
{
    internal sealed class WindowsConfirmationComponent : IConfirmationComponent
    {
        private readonly IButtonComponent _btnCancel;
        private readonly IButtonComponent _btnConfirm;
        private readonly List<IButtonComponent> _btnList;

        private string _message = string.Empty;
        private int _focusedIndex;

        public int Left { get; set; }
        public int Top { get; set; }

        public bool IsPending { get; private set; }
        public bool LastResult { get; private set; }

        public WindowsConfirmationComponent(IButtonComponent btnCancel, IButtonComponent btnConfirm)
        {
            _btnCancel = btnCancel ?? throw new ArgumentNullException(nameof(btnCancel));
            _btnConfirm = btnConfirm ?? throw new ArgumentNullException(nameof(btnConfirm));

            _btnCancel.Label = "Cancel";
            _btnConfirm.Label = "Confirm";
            _btnList = new List<IButtonComponent> { _btnCancel, _btnConfirm };
        }

        public void Prompt(string message)
        {
            _message = message;
            _focusedIndex = 0;

            for (int i = 0; i < _btnList.Count; i++)
            {
                _btnList[i].IsActive = i == _focusedIndex;
            }

            IsPending = true;
            Render();
        }

        public void HandleInput(ConsoleKeyInfo key)
        {
            if (!IsPending)
            {
                return;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                _focusedIndex = (_focusedIndex + 1) % _btnList.Count;

                for (int i = 0; i < _btnList.Count; i++)
                {
                    _btnList[i].IsActive = i == _focusedIndex;
                }

                foreach (var btn in _btnList)
                {
                    btn.Clear();
                    btn.Render();
                }

                return;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                var activeButton = _btnList[_focusedIndex];

                switch (activeButton.Label)
                {
                    case "Cancel":
                        LastResult = false;
                        IsPending = false;
                        break;

                    case "Confirm":
                        LastResult = true;
                        IsPending = false;
                        break;
                }

                return;
            }

            if (key.Key == ConsoleKey.Y)
            {
                LastResult = true;
                IsPending = false;
            }
            else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape)
            {
                LastResult = false;
                IsPending = false;
            }
        }

        public void Render()
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(_message);

            LayoutButtons();
            foreach (var btn in _btnList)
            {
                btn.Render();
            }
        }

        public void Clear()
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(new string(' ', Math.Max(_message.Length, 40)));

            foreach (var btn in _btnList)
            {
                btn.Clear();
            }
        }

        private void LayoutButtons()
        {
            int row = Top + 1;
            int left = Left;

            foreach (var btn in _btnList)
            {
                btn.Left = left;
                btn.Top = row;
                left += btn.Width + 4;
            }
        }
    }
}
