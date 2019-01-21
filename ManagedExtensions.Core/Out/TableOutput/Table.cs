using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Out.TableOutput
{
    public sealed class Table : IDisposable
    {
        public Table(Output output, List<Column> columns)
        {
            _output = output;
            _columns = columns.ToList();
        }

        public void AddRow(params Content[] cells)
        {
            AddRow(new Row(cells));
        }

        public void AddRow(Row row)
        {
            if (row.Cells.Count != _columns.Count)
                throw new ArgumentException(nameof(row));

            _rows.Add(row);
        }

        public Row CreateRow()
        {
            return new Row(_columns.Count);
        }

        public void Dispose()
        {
            OutputTable();
            _output.WriteLine();
        }

        private void OutputTable()
        {
            if (!TryOutputAsTable())
            {
                OutputAsBlocks();
            }
        }

        private bool TryOutputAsTable()
        {
            var rows = _rows.ToList();

            rows.Insert(0, new Row(_columns.Select(c => c.Header)));

            if (TryAdjustColumnsToVerticalOutput(rows))
            {
                rows.Insert(1, new Row(_columns.Select(c => string.Empty.PadLeft(c.CalculatedWidth, '-'))));

                _output.WriteLine();

                foreach (var row in rows)
                {
                    OutputRow(row);
                }
                return true;
            }
            return false;
        }

        private void OutputAsBlocks()
        {
            var maxColumnWidth = _columns.Max(c => c.Header.Width);

            _output.WriteLine();

            foreach (var row in _rows)
            {
                for (int i = 0; i < _columns.Count(); i++)
                {
                    _columns[i].Header.OutputInOneLine(maxColumnWidth, Align.Right, _output);
                    _output.Write(columnDelimeter);
                    row.Cells[i].Output(_output);
                    _output.WriteLine();
                }
                _output.WriteLine();
            }
        }

        private void OutputRow(Row row)
        {
            _output.Write(columnDelimeter);

            for (int colIndex = 0; colIndex < row.CellsCount; colIndex++)
            {
                var cell = row.Cells[colIndex];
                var colWidth = _columns[colIndex].CalculatedWidth;

                cell.OutputInOneLine(colWidth, _columns[colIndex].Align, _output);

                _output.Write(columnDelimeter);
            }
            _output.WriteLine();
        }

        private bool TryAdjustColumnsToVerticalOutput(List<Row> rows)
        {
            int maxSumWidth = MaxLineWidth - (_columns.Count + 1) * columnDelimeter.Length;

            for (int colIndex = 0; colIndex < _columns.Count; colIndex++)
            {
                var column = _columns[colIndex];
                column.CalculatedWidth = rows.Max(r => r.Cells[colIndex].Width);

                column.OneLineMinWidth = column.MinWidth.HasValue ?
                    Math.Max(column.MinWidth.Value, rows.Max(r => r.Cells[colIndex].OneLineMinWidth))
                    : column.CalculatedWidth;
            }

            var sumWidth = _columns.Sum(c => c.CalculatedWidth);

            if (sumWidth > maxSumWidth)
            {
                var columns = _columns
                    .Where(c => c.CalculatedWidth > c.OneLineMinWidth)
                    .ToList();

                var sumDecrease = sumWidth - maxSumWidth;
                if (!TryDecreaseColumnWidths(columns, sumDecrease))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryDecreaseColumnWidths(List<Column> columns, int sumDecrease)
        {
            if (!columns.Any())
                return false;

            var remainingDecrease = sumDecrease - columns.Sum(c => c.CalculatedWidth - c.OneLineMinWidth);

            var averageWidthIncrease = remainingDecrease < 0 ? -remainingDecrease / columns.Count : 0;

            foreach (var c in columns)
                c.CalculatedWidth = c.OneLineMinWidth + averageWidthIncrease;

            return remainingDecrease <= 0;
        }

        private List<Column> _columns;
        private List<Row> _rows = new List<Row>();
        private Output _output;

        private readonly string columnDelimeter = " | ";
        private const int MaxLineWidth = 125;
    }
}
