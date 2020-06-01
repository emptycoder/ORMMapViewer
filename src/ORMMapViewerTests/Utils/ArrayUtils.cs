using MathNet.Numerics.LinearAlgebra;

namespace ORMMapViewerTests.Utils
{
	public static class ArrayUtils
	{
		public static bool IsEqualsValues(this double[,] array1, double[,] array2)
		{
			if (array1.GetLength(0) != array2.GetLength(0)
			    || array1.GetLength(1) != array2.GetLength(1))
			{
				return false;
			}

			for (var i = 0; i < array1.GetLength(0); i++)
			{
				for (var j = 0; j < array1.GetLength(1); j++)
				{
					if (array1[i, j] != array2[i, j])
					{
						return false;
					}
				}
			}

			return true;
		}

		public static bool IsEqualsValues(this double[,] array1, Matrix<double> matrix)
		{
			if (array1.GetLength(0) != matrix.RowCount
			    || array1.GetLength(1) != matrix.ColumnCount)
			{
				return false;
			}

			for (var i = 0; i < array1.GetLength(0); i++)
			{
				for (var j = 0; j < array1.GetLength(1); j++)
				{
					if (array1[i, j] != matrix[i, j])
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
