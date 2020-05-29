using ORMMap.VectorTile;
using ORMMap.VectorTile.Geometry;
using System.Collections.Generic;
using System.Windows.Media;

namespace ORMMap
{
    public static class MVTDrawer
    {
        private delegate GeometryGroup DrawDelegate(VectorTileFeature feature);

        public static GeometryGroup GetGeometryFromLayer(VectorTileLayer layer)
        {
            GeometryGroup geometryGroup = new GeometryGroup();

            int featureCount = layer.FeatureCount();
            for (int i = 0; i < featureCount; i++)
            {
                VectorTileFeature feature = layer.GetFeature(i);
                if (feature.GeometryType == GeomType.POLYGON || feature.GeometryType == GeomType.LINESTRING)
                {
                    // geometryGroup.Children.Add(featureDrawDictionary[feature.GeometryType](feature));
                }
                else
                {
                    // Console.WriteLine(feature.ToString());
                }
            }

            return geometryGroup;
        }

        private static Dictionary<GeomType, DrawDelegate> featureDrawDictionary = new Dictionary<GeomType, DrawDelegate>()
        {
            { GeomType.POLYGON, DrawPolygon },
            { GeomType.LINESTRING, DrawLineString }
        };

        private static GeometryGroup DrawPolygon(VectorTileFeature feature)
        {
            /*List<PointF> points = new List<PointF>();
            var list = feature.Geometry<int>();
            foreach (var item in list)
            {
                foreach (var point in item)
                {
                    points.Add(new PointF(point.X, point.Y));
                }
            }

            using (SolidBrush solidBrush = new SolidBrush(pallete.MainFillColor))
            {
                using (Pen pen = new Pen(pallete.MainDrawColor))
                {
                    new 
                    graphics.FillPolygon(solidBrush, points.ToArray());
                    graphics.DrawPolygon(pen, points.ToArray());
                }
            }*/

            return null;
        }

        private static GeometryGroup DrawPoint(VectorTileFeature feature)
        {
            return null;
        }

        private static GeometryGroup DrawLineString(VectorTileFeature feature)
        {
            /*var props = feature.GetProperties();

            // Draw name of street
            if (props.ContainsKey("name"))
            {
                using (SolidBrush brush = new SolidBrush(pallete.getPropFillColor("name")))
                {
                    string text = (string)props["name"];
                    foreach (var point in feature.Geometry<int>()[0])
                    {
                        graphics.DrawString(text, SystemFonts.DefaultFont, brush, new Point(point.X, point.Y));
                    }
                }
            }*/

            return null;
        }
    }
}
