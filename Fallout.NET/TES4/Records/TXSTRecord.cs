using System.Xml.Linq;
using System;

using Fallout.NET.Core;
using System.Buffers;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum TXST_Flags
	{
		None = 0,
		NoSpecularMap = 0x1
	}

	/// <summary>
	/// Texture Set
	/// </summary>
	public sealed class TXSTRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord TX00_BaseImage = new();
		public STRSubRecord TX01_Normal = new();
		public STRSubRecord TX02_EnvironmentalMapMask = new();
		public STRSubRecord TX03_Emision = new();
		public STRSubRecord TX04_ParallaxMap = new();
		public STRSubRecord TX05_EnvironmentalMap = new();
		public DODTSubRecord DODT_DecalData = new();
		public TXST_Flags DNAM_Flags;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				//var end = stream.Length;

				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "OBND":
							OBND.Deserialize(stream, name);
							break;
						case "TX00":
							TX00_BaseImage.Deserialize(stream, name);
							break;
						case "TX01":
							TX01_Normal.Deserialize(stream, name);
							break;
						case "TX02":
							TX02_EnvironmentalMapMask.Deserialize(stream, name);
							break;
						case "TX03":
							TX03_Emision.Deserialize(stream, name);
							break;
						case "TX04":
							TX04_ParallaxMap.Deserialize(stream, name);
							break;
						case "TX05":
							TX05_EnvironmentalMap.Deserialize(stream, name);
							break;
						case "DODT":
							DODT_DecalData.Deserialize(stream, name);
							break;
						case "DNAM":
							var dnamesize= stream.ReadUInt16();
							DNAM_Flags = (TXST_Flags)stream.ReadUInt16();
							break;
						default:
							var rest = stream.ReadUInt16();
							UnityEngine.Debug.Log($"Unknown TXST sub record {name} size={rest}");
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}