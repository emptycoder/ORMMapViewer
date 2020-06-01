using MathNet.Numerics.LinearAlgebra;
using TransportationProblem.Utils;

namespace TransportationProblem
{
	public class Simplex
	{
		private readonly int[] basis; // Input: rowIndex
		public Vector<double> constraints; // Input: rowIndex
		public Matrix<double> matrix;
		public Vector<double> simplexFunction; // Input: columnIndex

		public Simplex(BasisFinder basisFinder, int[] basis)
		{
			matrix = basisFinder.matrix;
			simplexFunction = basisFinder.simplexFunction;
			constraints = basisFinder.constraints;
			this.basis = basis;
		}

		public double GetAnswer()
		{
			double result = 0;

			for (var rowIndex = 0; rowIndex < basis.Length; rowIndex++)
			{
				var columnIndex = basis[rowIndex];
				result += simplexFunction[columnIndex] * constraints[rowIndex];
			}

			return result;
		}

		public Simplex FindAnswer()
		{
			var pivot = FindPivot();
			while (pivot != null)
			{
				RecalculateMatrix(pivot);
				pivot = FindPivot();
			}

			return this;
		}

		private Pivot FindPivot()
		{
			// Calculate simplexCalculations
			double[] simplexCalculations = new double[matrix.ColumnCount];
			int columnIndex;
			Vector<double> columnData;

			for (columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				double value = 0;
				columnData = matrix.Column(columnIndex);
				for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
				{
					value += simplexFunction[basis[rowIndex]] * columnData[rowIndex];
				}

				value -= simplexFunction[columnIndex];
				simplexCalculations[columnIndex] = value;
			}

			// Find pivot
			columnIndex = simplexCalculations.GetMinIndex();
			if (simplexCalculations[columnIndex] >= 0)
			{
				return null;
			}

			// Find min value for row
			var minRowIndex = -1;
			var minRowValue = double.MaxValue;
			columnData = matrix.Column(columnIndex);
			for (var rowIndex = 0; rowIndex < columnData.Count; rowIndex++)
			{
				if (columnData[rowIndex] <= 0)
				{
					continue;
				}

				var value = constraints[rowIndex] / columnData[rowIndex];
				if (value < minRowValue)
				{
					minRowIndex = rowIndex;
					minRowValue = value;
				}
			}

			if (minRowIndex == -1)
			{
				return null;
			}

			return new Pivot(minRowIndex, columnIndex, matrix[minRowIndex, columnIndex]);
		}

		private void RecalculateMatrix(Pivot pivot)
		{
			// Change basis
			basis[pivot.rowIndex] = pivot.columnIndex;
			// Calculate constraints
			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				if (rowIndex == pivot.rowIndex)
				{
					continue;
				}

				constraints[rowIndex] = constraints[rowIndex] - matrix[rowIndex, pivot.columnIndex] * constraints[pivot.rowIndex] / pivot.value;
			}

			constraints[pivot.rowIndex] = constraints[pivot.rowIndex] / pivot.value;
			// Calculate matrix without main lines
			for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				if (columnIndex == pivot.columnIndex)
				{
					continue;
				}

				Vector<double> columnData = matrix.Column(columnIndex);
				for (var rowIndex = 0; rowIndex < columnData.Count; rowIndex++)
				{
					if (rowIndex == pivot.rowIndex)
					{
						continue;
					}

					matrix[rowIndex, columnIndex] = matrix[rowIndex, columnIndex] - matrix[rowIndex, pivot.columnIndex] * columnData[pivot.rowIndex] / pivot.value;
				}
			}

			// Calculate main lines of matrix
			Vector<double> mainRowData = matrix.Row(pivot.rowIndex);
			for (var columnIndex = 0; columnIndex < mainRowData.Count; columnIndex++)
			{
				matrix[pivot.rowIndex, columnIndex] = mainRowData[columnIndex] / pivot.value;
			}

			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				matrix[rowIndex, pivot.columnIndex] = 0;
			}

			matrix[pivot.rowIndex, pivot.columnIndex] = 1;
		}

		public sealed class Pivot
		{
			public int columnIndex;
			public int rowIndex;
			public double value;

			public Pivot(int rowIndex, int columnIndex, double value)
			{
				this.rowIndex = rowIndex;
				this.columnIndex = columnIndex;
				this.value = value;
			}
		}
	}
}
