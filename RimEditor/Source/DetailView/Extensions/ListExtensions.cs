using System;
using System.Collections.Generic;

namespace DRimEditor.DetailView
{
    public static class ListExtensions
    {
		public static void AddUnique<T>(this List<T> list, T item)
		{
			bool flag = list == null || list.Contains(item);
			if (!flag)
			{
				list.Add(item);
			}
		}
	}
}
