using System;

namespace TransportationProblem.Utils
{
	public static class ArrayUtil
	{
		public static void Fill<T>(this T[] array, int start, int count, T value)
		{
			if (array == null) throw new ArgumentNullException("array");

			if (count < 0) throw new ArgumentOutOfRangeException("count");

			if (start + count >= array.Length) throw new ArgumentOutOfRangeException("count");

			for (var i = start; i < start + count; i++) array[i] = value;
		}

		public static void Fill<T>(this T[] array, T value)
		{
			if (array == null) throw new ArgumentNullException("array");

			for (var i = 0; i < array.Length; i++) array[i] = value;
		}
	}
}
