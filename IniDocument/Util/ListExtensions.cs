using System;
using System.Collections.Generic;

namespace Takap.Utility
{
    // List<T> に対する拡張機能
    public static class ListExtensions
    {
        /// <summary>
        /// 指定した位置の要素を指定したインデックス位置に変更します。
        /// </summary>
        public static void ChangeOrder<T>(this List<T> list, int oldIndex, int newIndex)
        {
            if (newIndex > list.Count - 1) throw new ArgumentOutOfRangeException(nameof(newIndex));
            if (oldIndex == newIndex) return;

            T item = list[oldIndex];
            list.RemoveAt(oldIndex);

            if (newIndex > list.Count)
            {
                list.Add(item);
            }
            else
            {
                list.Insert(newIndex, item);
            }
        }

        /// <summary>
        /// 指定した条件に一致した最初の要素を指定したインデックス位置へ移動します。
        /// </summary>
        public static void ChangeOrder<T>(this List<T> list, Predicate<T> condition, int newIndex)
        {
            if (newIndex > list.Count - 1) throw new ArgumentOutOfRangeException(nameof(newIndex));
            int oldIndex = list.FindIndex(condition);
            ChangeOrder(list, oldIndex, newIndex);
        }
    }
}
