using System;
using System.Collections.Generic;

namespace DRimEditor.DetailView
{
    public static class ListExtensions
    {
		// Token: 0x0600003C RID: 60 RVA: 0x000035C8 File Offset: 0x000017C8
		public static void AddUnique<T>(this List<T> list, T item)
		{
			bool flag = list == null || list.Contains(item);
			if (!flag)
			{
				list.Add(item);
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000035F4 File Offset: 0x000017F4
		public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> items)
		{
			bool flag = list == null || items == null;
			if (!flag)
			{
				foreach (T item in items)
				{
					bool flag2 = !list.Contains(item);
					if (flag2)
					{
						list.Add(item);
					}
				}
			}
		}
	}
}
