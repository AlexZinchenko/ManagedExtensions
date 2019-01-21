using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagedExtensions.Core.Out.TableOutput
{
    public sealed class Row
    {
        public Row(int cellsCount)
        {
            CellsCount = cellsCount;
            _cells = new List<Content>(CellsCount);
        }

        public Row(IEnumerable<string> cells)
        {
            _cells = new List<Content>(cells.Select(c => new Content(c)));
            CellsCount = _cells.Count;
        }

        public Row(IEnumerable<Content> cells)
        {
            _cells = new List<Content>(cells);
            CellsCount = _cells.Count;
        }

        public int CellsCount { get; private set; }
        public IReadOnlyList<Content> Cells { get { return _cells.AsReadOnly(); } }

        public void Add(Content cell)
        {
            if (_cells.Count == CellsCount)
                throw new InvalidOperationException("row is full");

            _cells.Add(cell);
        }

        private readonly List<Content> _cells;
    }
}
