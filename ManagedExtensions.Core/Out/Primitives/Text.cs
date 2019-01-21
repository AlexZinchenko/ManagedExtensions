using ManagedExtensions.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedExtensions.Core.Out.Primitives
{
    public class Text : Chunk
    {
        public Text(string value)
        {
            _value = value;
        }

        public override int Width => _value.Length;

        internal override int MinWidth => Math.Min(1 + _dots.Length, Width);

        internal override void Output(int fixedWidth, Align align, Output output)
        {
            if (fixedWidth < MinWidth)
                throw new ArgumentException($"Can't output {_value} in {fixedWidth}. MinWidth = {MinWidth}");

            if (fixedWidth < Width)
            {
                if (align == Align.Left)
                {
                    output.Write(_value.Substring(0, fixedWidth - _dots.Count()));
                    output.Write(_dots);
                }
                else
                {
                    output.Write(_dots);
                    output.Write(_value.Substring(_dots.Count() + (Width - fixedWidth)));
                }
            }
            else
            {
                output.Write(_value.InFixedSpace(fixedWidth, align));
            }
        }

        internal override void Output(Output output)
        {
            output.Write(_value);
        }

        public static implicit operator Text(string str)
        {
            return new Text(str);
        }

        private string _value;
        private string _dots = "...";
    }
}
