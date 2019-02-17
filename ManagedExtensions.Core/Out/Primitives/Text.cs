using ManagedExtensions.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Out.Primitives
{
    public class Text : Chunk
    {
        public Text(string value)
        {
            _value = value;
        }

        public Text(string value, Color backgroundColor, Color foregroundColor) : this(value)
        {
            _backgroundColor = backgroundColor;
            _foregroundColor = foregroundColor;
        }

        public static Text CreateWithInvertedColor(string value)
        {
            return new Text(value, Color.DefaultForeground, Color.DefaultBackground);
        }

        public override int Width => _value.Length;

        public override void Output(Output output)
        {
            Write(_value, output);
        }

        internal override int MinWidth => Math.Min(1 + _dots.Length, Width);

        internal override void Output(int fixedWidth, Align align, Output output)
        {
            if (fixedWidth < MinWidth)
                throw new ArgumentException($"Can't output {_value} in {fixedWidth}. MinWidth = {MinWidth}");

            if (fixedWidth < Width)
            {
                if (align == Align.Left)
                {
                    Write(_value.Substring(0, fixedWidth - _dots.Count()), output);
                    Write(_dots, output);
                }
                else
                {
                    Write(_dots, output);
                    Write(_value.Substring(_dots.Count() + (Width - fixedWidth)), output);
                }
            }
            else
            {
                Write(_value.InFixedSpace(fixedWidth, align), output);
            }
        }

        private void Write(string str, Output output)
        {
            if (_backgroundColor == null && _foregroundColor == null)
            {
                output.Write(str);
            }
            else
            {
                var bgc = ConvertColor(_backgroundColor, _colors, _defaultBackgroundColor);
                var fgc = ConvertColor(_foregroundColor, _colors, _defaultForegroundColor);

                output.WriteDml($"<col fg=\"{fgc}\" bg=\"{bgc}\">{str}</col>");
            }
        }

        private static string ConvertColor(Color? color, Dictionary<Color, string> colors, Color defaultColor)
        {
            if (color == null || !colors.ContainsKey(color.Value))
            {
                color = defaultColor;
            }
            return colors[color.Value];
        }

        public static implicit operator Text(string str)
        {
            return new Text(str);
        }

        private string _value;
        private string _dots = "...";
        private readonly Color? _backgroundColor;
        private readonly Color? _foregroundColor;

        private static readonly Dictionary<Color, string> _colors = new Dictionary<Color, string>
        {
            { _defaultBackgroundColor, "wbg" },
            { _defaultForegroundColor, "wfg" }
        };

        private const Color _defaultBackgroundColor = Color.DefaultBackground;
        private const Color _defaultForegroundColor = Color.DefaultForeground;
    }
}
