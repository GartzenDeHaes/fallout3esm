using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public class WTHRRecord : Record
	{
		public FormID IAD_SunriseImageSpaceModifer = new();
		public FormID IAD_DayImageSpaceModifier = new();
		public FormID IAD_SunsetImageSpaceModifier = new();	
		public FormID IAD_NightImageSpaceModifier = new();
		public STRSubRecord DNAM_CloudTexturesLayer0 = new();
		public STRSubRecord CNAM_CloudTexturesLayer1 = new();
		public STRSubRecord ANAM_CloudTexturesLayer2 = new();
		public STRSubRecord BNAM_CloudTexturesLayer3 = new();
		public MODLSubRecord MODL;
		public byte[] ONAM_CloudLayerSpeed;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "\0x0000IAD":
							IAD_SunriseImageSpaceModifer.Deserialize(stream, name);
							break;
						case "\0x0001IAD":
							IAD_DayImageSpaceModifier.Deserialize(stream, name);
							break;
						case "\0x0002IAD":
							IAD_SunsetImageSpaceModifier.Deserialize(stream, name);
							break;
						case "\0x0003IAD":
							IAD_NightImageSpaceModifier.Deserialize(stream, name);
							break;
						case "DNAM":
							DNAM_CloudTexturesLayer0.Deserialize(stream, name);
							break;
						case "CNAM":
							CNAM_CloudTexturesLayer1.Deserialize(stream, name);
							break;
						case "ANAM":
							ANAM_CloudTexturesLayer2.Deserialize(stream, name);
							break;
						case "BNAM":
							BNAM_CloudTexturesLayer3.Deserialize(stream, name);
							break;
						case "MODL":
							MODL = new();
							MODL.Deserialize(stream, name);
							break;
						case "ONAM":
							var datasize = stream.ReadUInt16();
							ONAM_CloudLayerSpeed = stream.ReadBytes(datasize);
							break;
						default:
							//UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.SkipBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}