using ManagedExtensions.Core.Out.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Out
{
    public class Content
    {
        public Content(string str)
        {
            Text text = str;
            _chunks.Add(text);
        }

        public Content(params Chunk[] chunks)
        {
            _chunks = chunks.ToList();
        }
        
        public int Width
        {
            get
            {
                if (!_chunks.Any())
                    return 0;

                return _chunks.Sum(c => c.Width) + TotalSeparatorsWidth;
            }
        }

        internal int OneLineMinWidth
        {
            get
            {
                if (!_chunks.Any())
                    return 0;

                // support only text
                if (IsText())
                {
                    return _chunks.First().MinWidth;
                }

                return Width;
            }
        }

        private bool IsText()
        {
            return _chunks.Count == 1 && _chunks.First() is Text;
        }

        public void OutputInOneLine(int fixedWidth, Align align, Output output)
        {
            if (IsText())
            {
                _chunks.First().Output(fixedWidth, align, output);
                return;
            }

            if (fixedWidth < Width)
                throw new ArgumentException($"Can't output in {fixedWidth} width, minimal width is {Width}");

            if (align == Align.Right)
                output.Write(string.Empty.PadLeft(fixedWidth - Width), ' ');

            Output(output);

            if (align == Align.Left)
                output.Write(string.Empty.PadLeft(fixedWidth - Width), ' ');
        }

        public Content Append(Chunk chunk)
        {
            _chunks.Add(chunk);
            return this;
        }

        internal void Output(Output output)
        {
            foreach (var chunk in _chunks)
            {
                chunk.Output(output);
                if (chunk != _chunks.Last())
                {
                    output.Write(_separator);
                }
            }
        }

        public static implicit operator Content(Chunk chunk)
        {
            return new Content(chunk);
        }

        public static implicit operator Content(string str)
        {
            return new Content(str);
        }

        private int TotalMinWidth
        {
            get
            {
                if (!_chunks.Any())
                    return 0;

                return _chunks.Sum(c => c.MinWidth) + TotalSeparatorsWidth;
            }
        }

        private int TotalSeparatorsWidth
        {
            get
            {
                if (!_chunks.Any())
                    return 0;

                return _separator.Length * _chunks.Count - 1;
            }
        }

        public static Content Empty { get; } = new Content(string.Empty);

        private List<Chunk> _chunks = new List<Chunk>();

        private string _separator = " ";
    }
}
