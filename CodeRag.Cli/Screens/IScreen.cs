using CodeRag.Cli.Events;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using static CodeRag.Cli.Screens.Defs;

namespace CodeRag.Cli.Screens
{
    internal interface IScreen: IDisposable
    {
        public ScreenIds ScreenId { get; }
        public Result Initialize();
        public Result Render();

        public Result HandleEvent(InputKeyEvent screenEvent);
    }
}
