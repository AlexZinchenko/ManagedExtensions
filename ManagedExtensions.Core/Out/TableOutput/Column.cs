namespace ManagedExtensions.Core.Out.TableOutput
{
    public sealed class Column
    {
        public Column(Content header)
        {
            Header = header;
        }

        public Column(Content header, int minWidth) : this(header)
        {
            MinWidth = minWidth;
        }

        public Content Header { get; private set; }

        // used only if columns don't fit to page width and if real width > MinWidth
        public int? MinWidth { get; private set; }
        internal int CalculatedWidth { get; set; }
        public Align Align { get; set; } = Align.Left;
        internal int OneLineMinWidth { get; set; }
    }
}
