using System.Collections;
using System.Collections.Generic;

namespace Tokamak.Utilities
{
    public class SetList<TEntity> : IEnumerable<TEntity>
    {
        private readonly List<TEntity> m_list;

        private readonly HashSet<TEntity> m_set;

        public SetList()
        {
            m_list = [];
            m_set = [];
        }

        public SetList(int capacity)
        {
            m_list = new List<TEntity>(capacity);
            m_set = new HashSet<TEntity>(capacity);
        }

        public int Count => m_list.Count;

        public TEntity this[int index] { get => m_list[index]; }

        public int Add(TEntity entity)
        {
            if (!m_set.Add(entity))
                return m_list.IndexOf(entity);
            else
            {
                int idx = m_list.Count;
                m_list.Add(entity);
                return idx;
            }
        }

        public void Clear()
        {
            m_list.Clear();
            m_set.Clear();
        }

        public bool Contains(TEntity item) => m_set.Contains(item);

        public void CopyTo(TEntity[] array, int index) => m_list.CopyTo(array, index);

        public IEnumerator<TEntity> GetEnumerator() => m_list.GetEnumerator();

        public bool Remove(TEntity item) => m_set.Remove(item) ? m_list.Remove(item) : false;

        IEnumerator IEnumerable.GetEnumerator() => m_list.GetEnumerator();

        public int IndexOf(TEntity item) => m_set.Contains(item) ? m_list.IndexOf(item) : -1;
    }
}
