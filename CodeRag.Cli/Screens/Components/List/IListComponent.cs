namespace CodeRag.Cli.Screens.Components.List
{
    /// <summary>
    /// Represents a list component that can display a list of items and allows navigation through the items.
    /// </summary>
    internal interface IListComponent
    {
        bool IsActive { get; set; }
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; set; }
        int Height { get; set; }          // visible rows; list scrolls if items exceed this

        void SetItems(IReadOnlyList<string> items);
        void MoveUp();
        void MoveDown();
        int? SelectedIndex { get; }
        string? SelectedItem { get; }
        void Render();
        void Clear();
    }
}
