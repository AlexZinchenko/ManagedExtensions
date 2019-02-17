using System;
using System.Linq;
using ManagedExtensions.Core.Out.Primitives;
using Microsoft.Diagnostics.Runtime.Interop;

namespace ManagedExtensions.Core.Out
{
    public sealed class Output
    {
        public Output(IDebugClient client)
        {
            _client = client;
            _control = (IDebugControl)client;
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(Chunk chunk)
        {
            chunk.Output(this);
            WriteLine();
        }

        public void WriteLine(string format, params object[] args)
        {
            Write(format + "\n", args);
        }

        public void WriteWarningLine(string format, params object[] args)
        {
            WriteLine("Warning: " + format, args);
        }

        public void WriteErrorLine(string format, params object[] args)
        {
            WriteLine("Error: " + format, args);
        }

        public void Write(string format, params object[] args)
        {
            Write(format, args, dml: false);
        }

        public void WriteDml(string format, params object[] args)
        {
            Write(format, args, dml: true);
        }

        public void Execute(string format, params object[] args)
        {
            _control.Execute(DEBUG_OUTCTL.THIS_CLIENT, string.Format(format, args), DEBUG_EXECUTE.DEFAULT);
        }

        private void Write(string format, object[] args, bool dml)
        {
            var outputCtl = DEBUG_OUTCTL.ALL_CLIENTS;

            if (dml)
                outputCtl |= DEBUG_OUTCTL.DML;

            _control.ControlledOutput(outputCtl, DEBUG_OUTPUT.NORMAL, string.Format(format, args));
        }

        private IDebugClient _client;
        private IDebugControl _control;
    }
}
