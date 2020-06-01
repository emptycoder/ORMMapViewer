using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using TransportationProblem.Utils;

namespace TransportationProblem
{
	public class BasisFinder
	{
		public readonly Vector<double> constraints;
		public Matrix<double> matrix;
		public Vector<double> simplexFunction;

		public BasisFinder(Matrix<double> matrix, Vector<double> simplexFunction, Vector<double> constraints)
		{
			this.matrix = matrix;
			this.simplexFunction = simplexFunction;
			this.constraints = constraints;
		}

		public int[] GetBasis()
		{
			int[] basis = new int[matrix.RowCount];
			basis.Fill(-1);

			for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				var rowIndex = IsConvertibleToBasis(columnIndex);
				if (rowIndex != -1 && basis[rowIndex] == -1)
				{
					// Reduction to a basis
					Vector<double> columnData = matrix.Column(columnIndex);
					for (var i = 0; i < columnData.Count; i++)
					{
						matrix[rowIndex, i] = columnData[i] / columnData[rowIndex];
					}

					constraints[rowIndex] = constraints[rowIndex] / columnData[rowIndex];

					basis[rowIndex] = columnIndex;
				}
			}

			// Add artificial vector for basis creation
			for (var i = 0; i < basis.Length; i++)
			{
				if (basis[i] == -1)
				{
					basis[i] = AddArtificialVector(i);
				}
			}

			return basis;
		}

		private int AddArtificialVector(int i)
		{
			double[,] newMatrixData = new double[matrix.RowCount + 1, matrix.ColumnCount];
			for (var row = 0; row < matrix.RowCount; row++)
			for (var column = 0; i < matrix.ColumnCount; column++)
			{
				newMatrixData[row, column] = matrix[row, column];
			}

			var rowCount = matrix.RowCount;
			for (var column = 0; column < matrix.ColumnCount; column++)
			{
				newMatrixData[rowCount, column] = 0;
			}

			newMatrixData[rowCount, i] = 1;

			matrix = DenseMatrix.OfArray(newMatrixData);
			simplexFunction = DenseVector.OfEnumerable(simplexFunction.Append(0));

			return rowCount;
		}

		private int IsConvertibleToBasis(int row)
		{
			var columnIndex = -1;

			Vector<double> rowData = matrix.Column(row);
			for (var i = 0; i < rowData.Count; i++)
			{
				if (rowData[i] == 0)
				{
					continue;
				}

				if (columnIndex == -1)
				{
					columnIndex = i;
				}
				else
				{
					return columnIndex;
				}
			}

			return columnIndex;
		}
	}
}
