using System;
using System.Collections.Generic;

namespace DungeonCrawler.Scripts.Utils
{
    /// <summary>
    ///     Represents a priority queue where each element also has a priority.
    ///     Elements with a low priority are served before elements with a high priority.
    ///     Implementation is based on a binary min heap.
    /// </summary>
    /// <typeparam name="TValue">Value to be stored.</typeparam>
    /// <typeparam name="TPriority">Priority associated with each value.</typeparam>
    public class PriorityQueue<TValue, TPriority>
    {
        private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;
        private readonly IComparer<TPriority> _comparer;

        public PriorityQueue(IComparer<TPriority> comparer = null)
        {
            _baseHeap = new List<KeyValuePair<TPriority, TValue>>();
            _comparer = comparer ?? Comparer<TPriority>.Default;
        }

        public bool IsEmpty => _baseHeap.Count == 0;

        public void Enqueue(TPriority priority, TValue value)
        {
            Insert(new KeyValuePair<TPriority, TValue>(priority, value));
        }

        private void Insert(KeyValuePair<TPriority, TValue> value)
        {
            _baseHeap.Add(value);
            BubbleUp(_baseHeap.Count - 1);
        }

        private int BubbleUp(int pos)
        {
            if (pos >= _baseHeap.Count) return -1;

            // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];
            while (pos > 0)
            {
                var parentPos = (pos - 1) / 2;
                if (_comparer.Compare(_baseHeap[parentPos].Key, _baseHeap[pos].Key) > 0)
                {
                    SwapElements(parentPos, pos);
                    pos = parentPos;
                }
                else
                {
                    break;
                }
            }

            return pos;
        }

        private void SwapElements(int pos1, int pos2)
        {
            (_baseHeap[pos1], _baseHeap[pos2]) = (_baseHeap[pos2], _baseHeap[pos1]);
        }

        public TValue Dequeue()
        {
            if (IsEmpty) throw new InvalidOperationException($"{GetType().Name} is empty");
            var result = _baseHeap[0].Value;
            DeleteRoot();
            return result;
        }

        public TValue Peek()
        {
            if (IsEmpty) throw new InvalidOperationException($"{GetType().Name} is empty");
            return _baseHeap[0].Value;
        }

        private void DeleteRoot()
        {
            if (_baseHeap.Count <= 1)
            {
                _baseHeap.Clear();
                return;
            }

            _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
            _baseHeap.RemoveAt(_baseHeap.Count - 1);
            BubbleDown(0);
        }

        private void BubbleDown(int pos)
        {
            if (pos >= _baseHeap.Count) return;

            // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];
            while (true)
            {
                // on each iteration exchange element with its smallest child
                var smallest = pos;
                var left = (2 * pos) + 1;
                var right = (2 * pos) + 2;
                if ((left < _baseHeap.Count) &&
                    (_comparer.Compare(_baseHeap[smallest].Key, _baseHeap[left].Key) > 0))
                    smallest = left;
                if ((right < _baseHeap.Count) &&
                    (_comparer.Compare(_baseHeap[smallest].Key, _baseHeap[right].Key) > 0))
                    smallest = right;

                if (smallest != pos)
                {
                    SwapElements(smallest, pos);
                    pos = smallest;
                }
                else
                {
                    break;
                }
            }
        }
    }
}