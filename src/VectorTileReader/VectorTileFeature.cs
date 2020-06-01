using System;
using System.Collections.Generic;
using ORMMap.VectorTile.Geometry;

namespace ORMMap.VectorTile
{
	public class VectorTileFeature
	{
		// TODO: how to cache without using object
		// may a dictionary with parameters clip and scale as key to keep different requests fast
		private object _cachedGeometry;
		private uint? _clipBuffer;


		private float? _previousScale; //cache previous scale to not return
		private float? _scale;

		/// <summary>
		///     Initialize VectorTileFeature
		/// </summary>
		/// <param name="layer">Parent <see cref="VectorTileLayer" /></param>
		public VectorTileFeature(VectorTileLayer layer, uint? clipBuffer = null, float scale = 1.0f)
		{
			Layer = layer;
			_clipBuffer = clipBuffer;
			_scale = scale;
			Tags = new List<int>();
		}


		/// <summary>Id of this feature https://github.com/mapbox/vector-tile-spec/blob/master/2.1/vector_tile.proto#L32</summary>
		public ulong Id { get; set; }


		/// <summary>Parent <see cref="VectorTileLayer" /> this feature belongs too</summary>
		public VectorTileLayer Layer { get; }


		/// <summary><see cref="GeomType" /> of this feature</summary>
		public GeomType GeometryType { get; set; }


		/// <summary>Geometry in internal tile coordinates</summary>
		public List<uint> GeometryCommands { get; set; }


		/// <summary>Tags to resolve properties https://github.com/mapbox/vector-tile-spec/tree/master/2.1#44-feature-attributes</summary>
		public List<int> Tags { get; set; }


		public List<List<Vector2<T>>> Geometry<T>(
			uint? clipBuffer = null
			, float? scale = null
		)
		{
			// parameters passed to this method override parameters passed to the constructor
			if (_clipBuffer.HasValue && !clipBuffer.HasValue) clipBuffer = _clipBuffer;
			if (_scale.HasValue && !scale.HasValue) scale = _scale;

			// TODO: how to cache 'finalGeom' without making whole class generic???
			// and without using an object (boxing) ???
			var finalGeom = _cachedGeometry as List<List<Vector2<T>>>;
			if (null != finalGeom && scale == _previousScale) return finalGeom;

			//decode commands and coordinates
			var geom = DecodeGeometry.GetGeometry(
				Layer.Extent
				, GeometryType
				, GeometryCommands
				, scale.Value
			);
			if (clipBuffer.HasValue)
			{
				// HACK !!!
				// work around a 'feature' of clipper where the ring order gets mixed up
				// with multipolygons containing holes
				if (geom.Count < 2 || GeometryType != GeomType.POLYGON)
				{
					// work on points, lines and single part polygons as before
					geom = UtilGeom.ClipGeometries(geom, GeometryType, (long) Layer.Extent, clipBuffer.Value,
						scale.Value);
				}
				else
				{
					// process every ring of a polygon in a separate loop
					var newGeom = new List<List<Vector2<long>>>();
					var geomCount = geom.Count;
					for (var i = 0; i < geomCount; i++)
					{
						var part = geom[i];
						var tmp = new List<List<Vector2<long>>>();
						// flip order of inner rings to look like outer rings
						var isInner = signedPolygonArea(part) >= 0;
						if (isInner) part.Reverse();
						tmp.Add(part);
						tmp = UtilGeom.ClipGeometries(tmp, GeometryType, (long) Layer.Extent, clipBuffer.Value,
							scale.Value);
						// ring was completely outside of clip border
						if (0 == tmp.Count) continue;
						// one part might result in several geoms after clipping, eg 'u'-shape where the
						// lower part is completely beyond the actual tile border but still within the buffer
						foreach (var item in tmp)
						{
							// flip winding order of inner rings back
							if (isInner) item.Reverse();
							newGeom.Add(item);
						}
					}

					geom = newGeom;
				}
			}

			//HACK: use 'Scale' to convert to <T> too
			finalGeom = DecodeGeometry.Scale<T>(geom, scale.Value);

			//set field needed for next iteration
			_previousScale = scale;
			_cachedGeometry = finalGeom;

			return finalGeom;
		}


		private float signedPolygonArea(List<Vector2<long>> vertices)
		{
			var num_points = vertices.Count - 1;
			float area = 0;
			for (var i = 0; i < num_points; i++)
			{
				area +=
					(vertices[i + 1].X - vertices[i].X) *
					(vertices[i + 1].Y + vertices[i].Y) / 2;
			}

			return area;
		}


		/// <summary>
		///     Get properties of this feature. Throws exception if there is an uneven number of feature tag ids
		/// </summary>
		/// <returns>Dictionary of this feature's properties</returns>
		public Dictionary<string, object> GetProperties()
		{
			if (0 != Tags.Count % 2)
				throw new Exception(string.Format("Layer [{0}]: uneven number of feature tag ids", Layer.Name));
			var properties = new Dictionary<string, object>();
			var tagCount = Tags.Count;
			for (var i = 0; i < tagCount; i += 2) properties.Add(Layer.Keys[Tags[i]], Layer.Values[Tags[i + 1]]);
			return properties;
		}


		/// <summary>
		///     Get property by name
		/// </summary>
		/// <param name="key">Name of the property to request</param>
		/// <returns>Value of the requested property</returns>
		public object GetValue(string key)
		{
			var idxKey = Layer.Keys.IndexOf(key);
			if (-1 == idxKey) throw new Exception(string.Format("Key [{0}] does not exist", key));

			var tagCount = Tags.Count;
			for (var i = 0; i < tagCount; i++)
			{
				if (idxKey == Tags[i])
					return Layer.Values[Tags[i + 1]];
			}

			return null;
		}

		public override string ToString()
		{
			var text = $"Feature: {GeometryType}\n";
			foreach (var prop in GetProperties()) text += $"   {prop.Key} ({prop.Value.GetType()}): {prop.Value}\n";
			return text;
		}
	}
}
