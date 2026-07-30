using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRag.Cli.Screens.Components.Buttons
{
    public interface IButtonComponent
    {
        string Label { get; set; }
        bool IsActive { get; set; }
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; }          // how many characters this button occupies when rendered
        void Render();
        void Clear();               // erase just this button's own footprint
    }
}
