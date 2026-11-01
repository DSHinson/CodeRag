using CodeRag.Cli.Events;
using CodeRag.Common.Events;
using CodeRag.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using static CodeRag.Cli.Screens.ScreenDefs;

namespace CodeRag.Cli.Screens
{
    /// <summary>
    /// Represents a screen in the application that can be initialized, rendered, and handle input events.
    /// </summary>
    internal interface IScreen: IDisposable
    {
        public ScreenIds ScreenId { get; }
        public Task<Result> Initialize();
        public Task<Result> Render();
        public Task<Result> HandleEvent(InputKeyEvent screenEvent);
    }
}
