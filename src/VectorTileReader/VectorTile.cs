﻿using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ORMMap.VectorTile
{
	/// <summary>
	///     Class to access the tile data
	/// </summary>
	[DebuggerDisplay("{Zoom}/{TileColumn}/{TileRow}")]
	public class VectorTileObj
	{
		private readonly VectorTileReader _VTR;


		/// <summary>
		///     Class to access the tile data
		/// </summary>
		/// <param name="data">Byte array containing the raw (already unzipped) tile data</param>
		/// <param name="validate">If true, run checks if the tile contains valid data. Decreases decoding speed.</param>
		public VectorTileObj(byte[] data, bool validate = true)
		{
			_VTR = new VectorTileReader(data, validate);
		}


		/// <summary>
		///     Collection of layers contained in the tile
		/// </summary>
		/// <returns>Collection of layer names</returns>
		public ReadOnlyCollection<string> LayerNames()
		{
			return _VTR.LayerNames();
		}


		/// <summary>
		///     Get a tile layer by name
		/// </summary>
		/// <param name="layerName">Name of the layer to request</param>
		/// <returns>Decoded <see cref="VectorTileLayer" /></returns>
		public VectorTileLayer GetLayer(string layerName)
		{
			return _VTR.GetLayer(layerName);
		}
	}
}
