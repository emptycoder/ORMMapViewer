using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORMMapViewerTests.Utils;
using TransportationProblem;

namespace ORMMapViewerTests
{
	[TestClass]
	public class TransportationProblemTest
	{
		[TestMethod]
		public void TestSimplexMethod()
		{
			Matrix<double> matrix = DenseMatrix.OfArray(new double[,] { { 1, 0, 2, -1 }, { 0, 1, 4, 2 } });
			Vector<double> simplexFunction = DenseVector.OfArray(new double[] { 1, -4, 3, 4 });
			Vector<double> constraints = DenseVector.OfArray(new double[] { 2, 6 });

			BasisFinder basisFinder = new BasisFinder(matrix, simplexFunction, constraints);
			int[] basis = basisFinder.GetBasis();

			Console.WriteLine($"Basis: {String.Join(", ", basis)}");

			Simplex simplex = new Simplex(basisFinder, basis);
			simplex.FindAnswer();

			Console.WriteLine($"Matrix: {simplex.matrix.ToString()}");
			Console.WriteLine($"Constraints: {constraints.ToString()}");
			Console.WriteLine($"Simplex function: {simplexFunction.ToString()}");
			double answer = simplex.GetAnswer();
			Console.WriteLine($"Answer: {answer}");

			double[,] matrixResult = new double[,] { { 1, 0.5, 4, 0 }, { 0, 0.5, 2, 1 } };
			Assert.IsTrue(answer == 17.0 && matrixResult.IsEqualsValues(simplex.matrix));
		}
	}
}
