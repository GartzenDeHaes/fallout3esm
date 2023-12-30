using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.LAND;

using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Landscape
	/// </summary>
	public class LANDRecord : Record
	{
		public BytesSubRecord DATA_Unknown;
		public VNMLorVCLRSubRecord VNML_VertexNormal;
		public VNMLorVCLRSubRecord VCLR_VertexColor;
		public VHGTSubRecord VHGT_VertexHeightMap;
		public List<ATXTorBTXTSubRecord> BTXT_BaseLayerHeader = new List<ATXTorBTXTSubRecord>();
		public List<ATXTorBTXTSubRecord> ATXT_AlphaLayerHeader = new List<ATXTorBTXTSubRecord>();
		public List<VTXTSubRecord> VTXT_Textures = new List<VTXTSubRecord>();

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
							var btxt = new ATXTorBTXTSubRecord();
							btxt.Deserialize(stream, name);
							BTXT_BaseLayerHeader.Add(btxt);

							if (BTXT_BaseLayerHeader.Count > 1)
							{
								break;
							}

							break;
						case "ATXT":
							var atxt = new ATXTorBTXTSubRecord();
							atxt.Deserialize(stream, name);
							ATXT_AlphaLayerHeader.Add(atxt);

							if (ATXT_AlphaLayerHeader.Count > 1)
							{
								break;
							}

							break;
						case "VTXT":
							var vtxt = new VTXTSubRecord();
							vtxt.Deserialize(stream, name);
							VTXT_Textures.Add(vtxt);

							if (VTXT_Textures.Count > 1)
							{
								break;
							}

							break;
						default:
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}