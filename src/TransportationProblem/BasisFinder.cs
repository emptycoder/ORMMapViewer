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
			var basis = new int[matrix.RowCount];
			basis.Fill(-1);

			for (int columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				int rowIndex = IsConvertibleToBasis(columnIndex);
				if (rowIndex != -1 && basis[rowIndex] == -1)
				{
					// Reduction to a basis
					Vector<double> columnData = matrix.Column(columnIndex);
					for (int i = 0; i < columnData.Count; i++)
					{
						matrix[rowIndex, i] = columnData[i] / columnData[rowIndex];
					}

					constraints[rowIndex] = constraints[rowIndex] / columnData[rowIndex];

					basis[rowIndex] = columnIndex;
				}
			}

			// Add artificial vector for basis creation
			for (int i = 0; i < basis.Length; i++)
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
			var newMatrixData = new double[matrix.RowCount + 1, matrix.ColumnCount];
			for (int row = 0; row < matrix.RowCount; row++)
			for (int column = 0; i < matrix.ColumnCount; column++)
			{
				newMatrixData[row, column] = matrix[row, column];
			}

			int rowCount = matrix.RowCount;
			for (int column = 0; column < matrix.ColumnCount; column++)
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
			int columnIndex = -1;

			Vector<double> rowData = matrix.Column(row);
			for (int i = 0; i < rowData.Count; i++)
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
