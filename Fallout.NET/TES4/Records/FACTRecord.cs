using System;
using System.Buffers;
using System.Collections.Generic;
using System.Xml.Linq;
using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Faction
	/// </summary>
	public class FACTRecord : Record
	{
		public STRSubRecord FULL_Name;
		public List<XNAMSubRecord> XNAMRelations =  new();
		public Flags1 DATA_Flags1;
		public Flags2 DATA_Flags2;
		public List<RankSubRecord> Ranks = new();
		
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			string name;

			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));

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
						case "FULL":
							FULL_Name = new STRSubRecord();
							FULL_Name.Deserialize(stream, name);
							break;
						case "XNAM":
							var xnam = new XNAMSubRecord();
							xnam.Deserialize(stream, name);
							XNAMRelations.Add(xnam);
							break;
						case "DATA":
							var dsize = stream.ReadUInt16();
							if (dsize > 0)
							{
								DATA_Flags1 = (Flags1)stream.ReadByte();
							}
							if (dsize > 1)
							{
								DATA_Flags2 = (Flags2)stream.ReadByte();
							}
							if (dsize == 4)
							{
								// Unused
								stream.ReadUInt16();
							}
							break;
						case "RNAM":
							var rank = new RankSubRecord();
							//rank.Deserialize(stream, name);
							rank.RankNumber.Deserialize(stream, name);
							Ranks.Add(rank);
							break;
						case "MNAM":
							Ranks[Ranks.Count - 1].Male.Deserialize(stream, name);
							break;
						case "FNAM":
							Ranks[Ranks.Count - 1].Female.Deserialize(stream, name);
							break;
						case "INAM":
							Ranks[Ranks.Count - 1].Insignia.Deserialize(stream, name);
							break;
						case "CNAM":
							// Unused, crime gold multiplier
							stream.ReadBytes(stream.ReadUInt16());
							break;
						default:
							UnityEngine.Debug.Log($"Unknown FACT subrecord {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}

		[Flags]
		public enum Flags1 : byte
		{
			None = 0,
			HiddenFromPC = 0x1,
			Evil = 0x2,
			SpecialCombat = 0x4,
		}

		[Flags]
		public enum Flags2 : byte
		{
			None = 0,
			TrackCrime = 0x1,
			AllowSell = 0x2,
		}

		public class RankSubRecord
		{
			public UInt32SubRecord RankNumber = new();
			public STRSubRecord Male = new();
			public STRSubRecord Female = new();
			public STRSubRecord Insignia = new();

			//public void Deserialize(BetterReader reader, string name)
			//{
			//	//string data = reader.ReadString((int)Size);

			//	//name = reader.ReadString(4);
			//	//var male = reader.read
			//	RankNumber.Deserialize(reader, name);

			//	//name = reader.ReadString(4);
			//	Male.Deserialize(reader, name);
			//	//name = reader.ReadString(4);
			//	Female.Deserialize(reader, name);
			//	//name = reader.ReadString(4);
			//	Insignia.Deserialize(reader, name);
			//}
		}
	}
}