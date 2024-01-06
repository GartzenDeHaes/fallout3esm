using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.LAND;

using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Landscape Texture
	/// </summary>
	public class LANDRecord : Record
	{
		public BytesSubRecord DATA_Unknown;
		public VNMLorVCLRSubRecord VNML_VertexNormal;
		public VNMLorVCLRSubRecord VCLR_VertexColor;
		public VHGTSubRecord VHGT_VertexHeightMap;
		public ATXTorBTXTSubRecord BTXT_BaseLayerHeader = new ();
		public ATXTorBTXTSubRecord ATXT_AlphaLayerHeader = new ();
		public VTXTSubRecord VTXT_Texture = new ();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;

				while (stream.Position < end)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "DATA":
							DATA_Unknown = new BytesSubRecord();
							DATA_Unknown.Deserialize(stream, name);
							break;
						case "VNML":
							VNML_VertexNormal = new VNMLorVCLRSubRecord();
							VNML_VertexNormal.Deserialize(stream, name);
							break;
						case "VCLR":
							VCLR_VertexColor = new VNMLorVCLRSubRecord();
							VCLR_VertexColor.Deserialize(stream, name);
							break;
						case "VHGT":
							VHGT_VertexHeightMap = new VHGTSubRecord();
							VHGT_VertexHeightMap.Deserialize(stream, name);
							break;
						case "BTXT":
							BTXT_BaseLayerHeader.Deserialize(stream, name);
							break;
						case "ATXT":
							ATXT_AlphaLayerHeader.Deserialize(stream, name);
							break;
						case "VTXT":
							VTXT_Texture.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"{Type} unknown {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}