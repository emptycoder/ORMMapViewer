using HelixToolkit.Wpf;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using ORMMap;
using ORMMap.Model.Entitites;
using ORMMap.VectorTile.Geometry;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;
using ORMMapViewer.Utils.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TransportationProblem;

namespace ORMMapViewer
{
	public partial class MainWindow
	{
		const double range = 100;
		int countOfSources = 3;

		private HashSet<RaycastMapResult> pointsList = new HashSet<RaycastMapResult>();

		private void AddPointWithCheck(RaycastMapResult rayCastPoint)
		{
			if (rayCastPoint == null) { return; }

			RaycastMapResult resultPoint = pointsList.FirstOrDefault((point) =>
				point.TileLatLng.IsEquals(rayCastPoint.TileLatLng)
				&& point.PointLatLng.InRange(rayCastPoint.PointLatLng, range));

			if (resultPoint == null)
			{
				AddPoint(rayCastPoint);
			}
			else
			{
				RemovePoint(resultPoint);
			}
		}

		private void AddPoint(RaycastMapResult rayCastPoint)
		{
			Console.WriteLine($"Add point: {rayCastPoint}");
			// TODO: Add 3d models to scene as map points
			//RectangleVisual3D myCube = new RectangleVisual3D();
			//myCube.Origin = rayCastPoint.HitPoint;
			//myCube.Width = 200;
			//myCube.Length = 200;
			//myCube.Normal = new Vector3D(0, 1, 0);
			//myCube.LengthDirection = new Vector3D(0, 1, 0);
			//myCube.Material = new DiffuseMaterial(Brushes.Red);
			//myCube.UpdateModel();
			//viewport.Children.Add(myCube);
			//viewport.UpdateLayout();
			pointsList.Add(rayCastPoint);
			if (pointsList.Count == 1)
			{
				countOfSources = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Count of sources", "Please, enter the field", "1"));
			}
		}

		private void RemovePoint(RaycastMapResult rayCastPoint)
		{
			Console.WriteLine($"Remove point: {rayCastPoint}");
			pointsList.Remove(rayCastPoint);
		}

		private void PrepareDataForSimplexMethod()
		{
			// TODO: Algorithm:
			// - Standartization of points
			// - Calculate hashes for tiles
			// - Get graphs by hashes or real time add tiles
			// - Combine graphs
			// - Path finding
			// - Convert data for simplex method

			// TODO: UI usability structure
			help.Content = "Right click on map to select point";
			double[] constraints = new double[pointsList.Count / countOfSources];
			for (int i = 0; i < constraints.Length; i++)
			{
				constraints[i] = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Cost condition for {i}:", "Please, enter the field", "1"));
			}

			double[] simplex = new double[countOfSources];
			for (int i = 0; i < simplex.Length; i++)
			{
				simplex[i] = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Satisfaction condition for {i}:", "Please, enter the field", "1"));
			}

			double cZoom = dataController.ConvertToMapZoom(zoom);
			Vector2<double> tile = MercatorProjection.LatLngToTile(pointsList.First().TileLatLng, cZoom).Floor();
			Graph graph = dataController.GetRoads(new Vector3<double>(tile.X, tile.Y, cZoom));

			if (graph == null) { return; }

			//List<string> hashes = pointsList.GetMapTileHashes(dataController.ConvertToMapZoom(zoom));
			//Graph.CombineGraphs(dataController.GetRoads(hashes[0]).nodes, dataController.GetRoads(hashes[1]).nodes);

			List<Node> points = new List<Node>();

			foreach (RaycastMapResult point in pointsList)
			{
				//var proj = MercatorProjection.LatLngToTile(, cZoom);
				var proj = point.PointLatLng;
				points.Add(graph.FindNearest(proj));
			}

			Console.WriteLine("Clear list of points...");
			pointsList.Clear();

			double[,] distances = new double[points.Count / countOfSources, countOfSources];
			int counterX = 0;
			int counterY = 0;
			for (int i = 1; i < points.Count; i++)
			{
				if (counterX == countOfSources)
				{
					counterY = 0;
					counterX++;
				}
				AStarPathSearch.AStarPathSearchResult result = AStarPathSearch.FindPath(points[0], points[i]);
				if (result == null) { return; }
				distances[counterX, counterY] = result.Score;
				counterY++;
			}

			Matrix<double> distanceMatrix = DenseMatrix.OfArray(distances);
			Vector<double> constraintsVector = DenseVector.OfArray(constraints);
			Vector<double> simplexFunction = DenseVector.OfArray(simplex);

			BasisFinder basisFinder = new BasisFinder(distanceMatrix, simplexFunction, constraintsVector);
			int[] basis = basisFinder.GetBasis();

			Console.WriteLine($"Basis: {string.Join(", ", basis)}");

			Simplex simplexMethod = new Simplex(basisFinder, basis);
			simplexMethod.FindAnswer();

			Console.WriteLine($"Matrix: {simplexMethod.matrix.ToString()}");
			Console.WriteLine($"Constraints: {constraintsVector.ToString()}");
			Console.WriteLine($"Simplex function: {simplexFunction.ToString()}");
			double answer = simplexMethod.GetAnswer();
			Console.WriteLine($"Answer: {answer}");
		}

		private RaycastMapResult RayCastToMap(Point startPoint)
		{
			RayHitTestResult rayHitTestResult = VisualTreeHelper.HitTest(viewport, startPoint) as RayHitTestResult;

			if (rayHitTestResult != null)
			{
				Point3D hitPoint = rayHitTestResult.PointHit;
				// Convert to tile coords
				var hitPointOnTile = new Vector2<double>(hitPoint.X - rayHitTestResult.ModelHit.Bounds.X, -(rayHitTestResult.ModelHit.Bounds.Y - hitPoint.Y));
				hitPointOnTile.X = 4096 - hitPointOnTile.X;
				hitPointOnTile = hitPointOnTile.Multiply(2);

				Projection projection = CalculateProjection();
				var hitTile = new Vector2<double>(hitPoint.X, hitPoint.Y).Divide(4096).ToVectorInt().Multiply(4096);

				double startPosX = (int)Math.Floor((hitTile.X - (projection.Bottom / 2)) / 4096) + 1;
				double startPosY = (int)Math.Floor((hitTile.Y - (projection.Left / 2)) / 4096) + 1;

				double cZoom = dataController.ConvertToMapZoom(zoom);
				Vector2<double> startTileCoordinations = MercatorProjection.LatLngToTile(Settings.startPosition, cZoom);
				LatLng tileLatLng = MercatorProjection.TileToLatLng(new Vector2<double>(startTileCoordinations.X - startPosX, startTileCoordinations.Y + startPosY), cZoom);
				//LatLng pointLatLng = MercatorProjection.TileToLatLng(, cZoom);

				Vector2<double> tile = MercatorProjection.LatLngToTile(tileLatLng, cZoom).Floor();
				Graph graph = dataController.GetRoads(new Vector3<double>(tile.X, tile.Y, cZoom));
				help.Content = graph.FindNearest(hitPointOnTile);
				
				return new RaycastMapResult(tileLatLng, hitPointOnTile, hitPoint);
			}

			return null;
		}

		private RaycastMapResult RayCastToMap()
		{
			return RayCastToMap(Mouse.GetPosition(window));
		}
	}
}
