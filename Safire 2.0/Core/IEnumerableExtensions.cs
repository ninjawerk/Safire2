using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Safire.Core
{
	public static class ThreadSafeRandom
	{
		[ThreadStatic]
		private static Random Local;

		public static Random ThisThreadsRandom
		{
			get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
		}
	}

	static class MyExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
	public static class IObservableCollectionExtensions
	{
		public static ObservableCollection<T> Shuffle<T>(this ObservableCollection<T> list)
		{
			var r = new Random((int)DateTime.Now.Ticks);
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = r.Next(0, i - 1);
				var e = list[i];
				list[i] = list[j];
				list[j] = e;
			}
			return list;
		}
	}
}
