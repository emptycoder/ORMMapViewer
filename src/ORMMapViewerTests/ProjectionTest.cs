using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORMMapViewer.Model.Entitites;
using ORMMapViewer.Utils;
using System;

namespace ORMMapViewerTests
{
	[TestClass]
	public class ProjectionTest
	{
		[TestMethod]
		public void TestPerspectiveProjection()
		{
			Projection projection = ProjectionUtils.GetPerspectiveProjection(90, ProjectionUtils.FovVFromAspectRatio(90, 1280.0 / 720), 48000, 0.11111111111 * Math.PI);
			if (projection.Left == projection.Right)
			{
				Assert.IsTrue(projection.Left == 17164.392292479552
					&& projection.Top == 75047.836096120256
					&& projection.Bottom == 155498.4182922107);
				return;
			}

			Assert.Fail("Left side != right side.");
		}

		[TestMethod]
		public void TestRectangleProjection()
		{
			Projection projection = ProjectionUtils.GetRectangleProjection(90, ProjectionUtils.FovVFromAspectRatio(90, 1280.0 / 720), 48000);
			if (projection.Left == projection.Right && projection.Top == projection.Bottom)
			{
				Assert.IsTrue(projection.Left == 17445.15105326362 && projection.Top == 155498.4182922107);
				return;
			}

			Assert.Fail("Left side != right side or top side != bottom side.");
		}

		[TestMethod]
		public void TestFovVFromAspectRatio()
		{
			double fovV = ProjectionUtils.FovVFromAspectRatio(90, 1280.0 / 720);
			Assert.IsTrue(fovV == 50.625);
		}
	}
}
