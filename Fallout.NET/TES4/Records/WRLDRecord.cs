using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.WRLD;

using System;
using System.Buffers;
using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	public class WRLDRecord : Record
	{
		public STRSubRecord FULL = new();
		public FormID WNAM = new();
		public STRSubRecord SNAM = new();
		public STRSubRecord ICON = new();
		public FormID CNAM = new();
		public FormID NAM2 = new();
		public FormID NAM3 = new();
		public FloatSubRecord NAM4 = new();
		public WRLD_DNAMSubRecord DNAM = new();
		public WRLD_MNAMSubRecord MNAM = new();
		public WRLD_ONAMSubRecord ONAM = new();
		public FormID INAM = new();
		public WRLD_DATASubRecord DATA = new();
		public Vector2fSubRecord NAM0 = new();
		public Vector2fSubRecord NAM9 = new();
		public STRSubRecord NNAM = new();
		public STRSubRecord XNAM = new();
		public UInt32SubRecord XXXX = new();
		public FormID XEZN = new();
		public WRLD_PNAMSubRecord PNAM = new();
		public FormID ZNAM = new();

		public List<Group> SubGroups;

		public override void NoteSubGroup(Group subGroup)
		{
			SubGroups ??= new List<Group>();
			SubGroups.Add(subGroup);
		}

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			//var bytes = reader.ReadBytes((int)size);
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
						case "FULL":
							FULL.Deserialize(stream, name);
							break;
						case "WNAM":
							WNAM.Deserialize(stream, name);
							break;
						case "CNAM":
							CNAM.Deserialize(stream, name);
							break;
						case "NAM2":
							NAM2.Deserialize(stream, name);
							break;
						case "NAM3":
							NAM3.Deserialize(stream, name);
							break;
						case "NAM4":
							NAM4.Deserialize(stream, name);
							break;
						case "DNAM":
							DNAM.Deserialize(stream, name);
							break;
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MNAM":
							MNAM.Deserialize(stream, name);
							break;
						case "ONAM":
							ONAM.Deserialize(stream, name);
							break;
						case "INAM":
							INAM.Deserialize(stream, name);
							break;
						case "DATA":
							DATA.Deserialize(stream, name);
							break;
						case "NAM0":
							NAM0.Deserialize(stream, name);
							break;
						case "NAM9":
							NAM9.Deserialize(stream, name);
							break;
						case "NNAM":
							NNAM.Deserialize(stream, name);
							break;
						case "XNAM":
							XNAM.Deserialize(stream, name);
							break;
						case "XXXX":
							//var xxxxSize = stream.ReadUInt16();
							//var xxxxData = stream.ReadBytes(xxxxSize);
							//var xxxxDataStr = System.Text.Encoding.ASCII.GetString(xxxxData);
							XXXX.Deserialize(stream, name);
							break;
						case "OFST":
							var ofstSize = Convert.ToInt32(stream.ReadUInt16());
							if (ofstSize == 0)
							{
								ofstSize = Convert.ToInt32(XXXX.Value);
							}
							var ofstData = stream.ReadBytes(ofstSize);
							break;
						case "XEZN":
							XEZN.Deserialize(stream, name);
							break;
						case "PNAM":
							PNAM.Deserialize(stream, name);
							break;
						case "ZNAM":
							ZNAM.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"{Type} unknown {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}

		public override string ToString()
		{
			return EDID.ToString();
		}
	}
}
