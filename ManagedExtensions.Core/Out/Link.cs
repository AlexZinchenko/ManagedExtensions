namespace ManagedExtensions.Core.Out
{
    public sealed class Link
    {
        public Link(object displayName, string cmd)
        {
            Cmd = cmd;
            DisplayName = displayName.ToString();
        }

        public string Cmd { get; private set; }
        public string DisplayName { get; private set; }

        public override string ToString()
        {
            return string.Format(_linkTemplate, Cmd, DisplayName);
        }

        private readonly string _linkTemplate = "<link cmd=\"{0}\">{1}</link>";
    }
}
