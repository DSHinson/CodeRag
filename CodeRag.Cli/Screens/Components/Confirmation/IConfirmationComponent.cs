using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.Confirmation
{
    internal interface IConfirmationComponent
    {
        int Left { get; set; }
        int Top { get; set; }

        bool IsPending { get; }
        bool LastResult { get; }

        void Prompt(string message);
        void HandleInput(ConsoleKeyInfo key);
        void Render();
        void Clear();
    }
}
