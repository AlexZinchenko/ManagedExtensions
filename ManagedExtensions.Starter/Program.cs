using ManagedExtensions.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ManagedExtensions.Core.Helpers;
using Microsoft.Diagnostics.Runtime;

namespace ManagedExtensions.Starter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string fileName = null;

            if (TryGetFileFromArgs(args, out fileName) 
             || TryChooseDump(out fileName))
            {
                OpenDump(fileName);
            }
        }

        private static bool TryGetFileFromArgs(string[] args, out string fileName)
        {
            if (args.Length == 1)
            {
                fileName = args[0];
                return true;
            }

            fileName = null;
            return false;
        }

        private static bool TryChooseDump(out string fileName)
        {
            var openFileDlg = new OpenFileDialog
            {
                Filter = "dmp (*.dmp)|*.dmp"
            };

            var res = (openFileDlg.ShowDialog() == DialogResult.OK);
            fileName = openFileDlg.FileName;

            return res;
        }

        private static void OpenDump(string fileName)
        {
            var startupCommands = new List<string>(startCommands)
            {
                GetLoadExtensionCommand()
            };

            ulong? startupObjectAddress = GetObjectAddress(fileName, "SampleApp.TestRoot", "Dictionaries.ExampleDictionary");

            if (startupObjectAddress != null)
            {
                startupCommands.Add($"!do {startupObjectAddress:x}");
            }

            var winDbgCommandsFile = GetCommandsTempFile(
                startCommands.Concat(startupCommands));

            Process
                .Start("windbg.exe", $@"-z ""{fileName}"" -c ""$<{winDbgCommandsFile}""")
                .WaitForExit();
        }

        private static ulong? GetObjectAddress(string dumpPath, string typeName, string propertyPath)
        {
            try
            {
                var properties = propertyPath.Split('.');
                using (var dataTarget = DataTarget.LoadCrashDump(dumpPath))
                {
                    var runtimeInfo = dataTarget.ClrVersions[0];
                    var runtime = runtimeInfo.CreateRuntime();

                    var rootType = runtime.Heap.GetTypeByName(typeName);
                    var staticField = rootType.GetStaticFieldByName(properties.First());
                    var targetObj = (ulong)staticField.GetValue(runtime.AppDomains.First());
                    var targetType = staticField.Type;

                    foreach (var property in properties.Skip(1))
                    {
                        var field = targetType.GetFieldByName(property);
                        targetObj = (ulong)field.GetValue(targetObj);
                    }

                    return targetObj;
                }
            }
            catch { return null; }
        }

        private static string GetCommandsTempFile(IEnumerable<string> commands)
        {
            return TempFile.Create(string.Join("\n", commands), "txt");
        }

        private static string GetLoadExtensionCommand()
        {
            var extensionAssembly = typeof(EntryPoint).Assembly.Location;
            return $".load {extensionAssembly}";
        }

        private static string[] startCommands = new string[]
        {
            ".prefer_dml 1",
            ".symfix",
            ".cordll -ve -u -l"
            //"!bhi"
        };
    }
}
