using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Out
{
    public sealed class Table : IDisposable
    {
        public Table(Output output, Column[] columns)
        {
            _output = output;
            _columns = columns;

            WriteHeader();
        }

        private void WriteHeader()
        {
            WriteRow(_columns.Select(c => c.Header).ToArray());

            var delimWidth = columnDelimeter.Length;
            var rowWidth = _columns.Sum(c => c.Width) + delimWidth * (_columns.Length - 1);

            _output.WriteLine("".PadLeft(rowWidth, '-'));
        }

        public void WriteRow(Row row)
        {
            WriteRow(row.Cells.ToArray());
        }

        private void WriteRow(object[] cells)
        {
            if (cells.Length != _columns.Length)
                throw new ArgumentException("row must contain cellsCount equal to table columns count");

            for (int i = 0; i < _columns.Length; i++)
            {
                var cell = cells[i];
                var column = _columns[i];
                var isLastColumn = i == _columns.Length - 1;

                var cellLink = cell as Link;
                if (cellLink != null)
                {
                    if (!isLastColumn)
                    {
                        WriteLinkInFixedSpace(column.Width, cellLink);
                    }
                    else
                    {
                        _output.Write("{0}", cellLink);
                    }
                }
                else
                {
                    if (!isLastColumn)
                    {
                        WriteInFixedSpace(column.Width, cell.ToString());
                    }
                    else
                    {
                        _output.Write(cell.ToString());
                    }
                }

                if (!isLastColumn)
                    _output.Write(columnDelimeter);
            }
            _output.WriteLine();
        }

        public Row CreateRow()
        {
            return new Row(_columns.Length);
        }

        public void Dispose()
        {
            _output.WriteLine();
        }

        private void WriteInFixedSpace(int columnWidth, string format, params object[] args)
        {
            var formattedString = string.Format(format, args);

            if (columnWidth > 0)
            {
                formattedString = formattedString.PadLeft(columnWidth);

                if (formattedString.Length > columnWidth)
                {
                    formattedString = "..." + formattedString.Substring(formattedString.Length - (columnWidth - 3));
                }
            }

            _output.Write(formattedString);
        }

        private void WriteLinkInFixedSpace(int columnWidth, Link link)
        {
            var formatString = "{0}";

            if (columnWidth > 0)
            {
                var leftPadding = columnWidth - link.DisplayName.Length;

                if (leftPadding > 0)
                    formatString = formatString.PadLeft(leftPadding + formatString.Length);
            }

            _output.Write(formatString, link);
        }

        private Column[] _columns;
        private Output _output;

        private readonly string columnDelimeter = " | ";
    }

    public sealed class Row
    {
        public Row(int cellsCount)
        {
            CellsCount = cellsCount;
            _cells = new List<object>(CellsCount);
        }

        public int CellsCount { get; private set; }
        public IReadOnlyList<object> Cells { get { return _cells.AsReadOnly(); } }

        public void Add(object cell)
        {
            if (_cells.Count == CellsCount)
                throw new InvalidOperationException("row is full");

            _cells.Add(cell);
        }

        private readonly List<object> _cells;
    }

    public sealed class Column
    {
        public Column(string header, int width)
        {
            Header = header;
            Width = width;
        }

        public string Header { get; private set; }
        public int Width { get; private set; }
    }
}
