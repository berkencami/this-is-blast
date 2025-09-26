using System.Collections.Generic;

namespace ThisIsBlast.Utility
{
    public class IndexedQueue<T>
    {
        private readonly List<T> _items = new List<T>();
        public IReadOnlyList<T> Items => _items;

        public void EnqueueBack(T item)
        {
            _items.Add(item);
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public T PeekFront()
        {
            return _items.Count > 0 ? _items[0] : default;
        }

        public bool TryRemoveAt(int row)
        {
            if (row < 0 || row >= _items.Count) return false;
            _items.RemoveAt(row);
            return true;
        }
    }

    public class MultiColumnQueue<T>
    {
        private int _columnCount;
        public List<IndexedQueue<T>> Columns { get; } = new List<IndexedQueue<T>>();

        public MultiColumnQueue(int columnCount)
        {
            SetColumnCount(columnCount);
        }

        private void SetColumnCount(int columnCount)
        {
            _columnCount = columnCount < 1 ? 1 : columnCount;
            Columns.Clear();
            for (var i = 0; i < _columnCount; i++) Columns.Add(new IndexedQueue<T>());
        }

        public void AddBack(int col, T item)
        {
            if (col < 0 || col >= _columnCount) return;
            Columns[col].EnqueueBack(item);
        }

        public bool TryRemove(T item, out int col, out int row)
        {
            for (var c = 0; c < _columnCount; c++)
            {
                var r = Columns[c].IndexOf(item);
                if (r < 0) continue;
                Columns[c].TryRemoveAt(r);
                col = c;
                row = r;
                return true;
            }

            col = -1;
            row = -1;
            return false;
        }

        public T GetFront(int col)
        {
            if (col < 0 || col >= _columnCount) return default;
            return Columns[col].PeekFront();
        }
    }
}