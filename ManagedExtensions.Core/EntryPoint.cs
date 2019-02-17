using System;
using System.Diagnostics;
using System.Linq;
using ManagedExtensions.Core.Commands;
using ManagedExtensions.Core.Native;
using ManagedExtensions.Core.Out.Primitives;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Core
{
    public static class EntryPoint
    {
        [DllExport]
        public static HRESULT DebugExtensionInitialize(ref uint version, ref uint flags)
        {
            // Set the extension version to 1, which expects exports with this signature:
            //      void _stdcall function(IDebugClient *client, const char *args)
            version = GetExtensionVersion(1, 0);
            flags = 0;
            return HRESULT.S_OK;
        }
        
        public static void Execute<TCommand>(IntPtr client, Action<TCommand> commandMethod)
            where TCommand : NativeCommand
        {
            InitApi(client);

            try
            {
                _commandsHost.Execute(commandMethod);
            }
            catch (Exception e)
            {
                _debugger.Output.WriteLine("Unhandled exception {0}: {1}, \nstackTrace:\n{2}", e.GetType(), e.Message, e.StackTrace);
            }
        }
        
        private static void InitApi(IntPtr ptrClient)
        {
            if (_debugger == null)
            {
                _debugger = new DebugServices(ptrClient);
            }

            if (_commandsHost == null)
            {
                ClrRuntime runtime;
                if (TryCreateRuntime(out runtime))
                {
                    _commandsHost = new CommandsHost(_debugger, runtime);
                    _typesPreloader.LoadAllTypes(_debugger, runtime);
                }
                else
                {
                    _commandsHost = new CommandsHost(_debugger);
                }
            }
        }

        private static bool TryCreateRuntime(out ClrRuntime runtime)
        {
            var mscorDacWksModule = FindMsCorDacModule();

            if (mscorDacWksModule != null)
            {
                runtime = _debugger.DataTarget.ClrVersions.Single().CreateRuntime(mscorDacWksModule.FileName);
                return true;
            }

            runtime = null;

            _debugger.Output.WriteLine("Mscordacwks.dll wasn't loaded into the debugger.");
            _debugger.Output.WriteLine(Text.CreateWithInvertedColor("Only commands for analyzing native application dumps are available."));
            _debugger.Output.WriteLine();

            return false;
        }

        private static ProcessModule FindMsCorDacModule()
        {
            var mscorDacWksModule = Process
                .GetCurrentProcess()
                .Modules
                .OfType<ProcessModule>()
                .FirstOrDefault(module => module.FileName.ToLower().Contains("mscordacwks"));
            return mscorDacWksModule;
        }

        private static uint GetExtensionVersion(uint Major, uint Minor)
        {
            return ((((Major) & 0xffff) << 16) | ((Minor) & 0xffff));
        }

        private static CommandsHost _commandsHost;
        private static DebugServices _debugger;
        private static TypesPreloader _typesPreloader = new TypesPreloader();
    }
}