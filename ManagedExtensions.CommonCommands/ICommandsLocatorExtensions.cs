using ManagedExtensions.CommonCommands.DependencyProperties;
using ManagedExtensions.Core.Commands;

namespace ManagedExtensions.CommonCommands
{
    public static class ICommandsLocatorExtensions
    {
        public static OpenFileCommand OpenFileCommand(this ICommandsLocator locator)
        {
            return locator.Get<OpenFileCommand>();
        }
        
        public static DumpDepPropertiesCommand DumpDpsCommand(this ICommandsLocator locator)
        {
            return locator.Get<DumpDepPropertiesCommand>();
        }

        public static DumpDepPropertyCommand DumpDpCommand(this ICommandsLocator locator)
        {
            return locator.Get<DumpDepPropertyCommand>();
        }

        public static HelpCommand HelpCommand(this ICommandsLocator locator)
        {
            return locator.Get<HelpCommand>();
        }
    }
}
