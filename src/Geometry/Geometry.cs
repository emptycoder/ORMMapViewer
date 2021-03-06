﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace ORMMap.VectorTile.Geometry
{
	/// <summary>
	///     Available geometry types
	/// </summary>
	public enum GeomType
	{
		UNKNOWN = 0,
		[Description("Point")] POINT = 1,
		[Description("LineString")] LINESTRING = 2,
		[Description("Polygon")] POLYGON = 3
	}


	/// <summary>
	///     Structure to hold a LatLng coordinate pair
	/// </summary>
	public struct LatLng
	{
		public double Lat { get; set; }
		public double Lng { get; set; }

		public LatLng(double lat, double lng)
		{
			Lat = lat;
			Lng = lng;
		}

		public bool IsEquals(LatLng latLng)
		{
			return Lat == latLng.Lat && Lng == latLng.Lng;
		}

		public bool InRange(LatLng latLng, double range)
		{
			return Math.Abs(Lat - latLng.Lat) <= range && Math.Abs(Lng - latLng.Lng) <= range;
		}

		public LatLng Round(int digits)
		{
			this.Lat = Math.Round(Lat, digits);
			this.Lng = Math.Round(Lng, digits);

			return this;
		}

		public override string ToString()
		{
			return string.Format(
				NumberFormatInfo.InvariantInfo
				, "{0:0.000000}/{1:0.000000}"
				, Lat
				, Lng);
		}
	}


	/// <summary>
	///     Structure to hold a 2D point coordinate pair
	/// </summary>
	public struct Vector2<T>
	{
		public T X;
		public T Y;

		public Vector2(T x, T y)
		{
			X = x;
			Y = y;
		}

		public Vector2(Vector2<T> vector2)
		{
			X = vector2.X;
			Y = vector2.Y;
		}

		public Vector2<T> Set(T x, T y)
		{
			X = x;
			Y = y;

			return this;
		}

		public LatLng ToLngLat(ulong z, ulong x, ulong y, ulong extent, bool checkLatLngMax = false)
		{
			double size = extent * Math.Pow(2, z);
			double x0 = extent * (double) x;
			double y0 = extent * (double) y;

			double dblY = Convert.ToDouble(Y);
			double dblX = Convert.ToDouble(X);
			double y2 = 180d - (dblY + y0) * 360d / size;
			double lng = (dblX + x0) * 360d / size - 180d;
			double lat = 360d / Math.PI * Math.Atan(Math.Exp(y2 * Math.PI / 180d)) - 90d;

			if (checkLatLngMax)
			{
				if (lng < -180d || lng > 180d)
				{
					throw new ArgumentOutOfRangeException("Longitude out of range");
				}

				if (lat < -85.051128779806589d || lat > 85.051128779806589d)
				{
					throw new ArgumentOutOfRangeException("Latitude out of range");
				}
			}

			LatLng latLng = new LatLng
			{
				Lat = lat,
				Lng = lng
			};

			return latLng;
		}

		public override string ToString()
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{0}/{1}", X, Y);
		}
	}
}
