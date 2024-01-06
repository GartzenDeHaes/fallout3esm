using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords.CELL;
using Fallout.NET.TES4.SubRecords;
using System;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/BOOK.md
	/// </summary>
	public sealed class BOOKRecord : Record
	{
		public STRSubRecord FULL;
		public OBNDSubRecord OBND_Bounds;
		public MODLSubRecord MODL;
		public STRSubRecord ICON_InventoryImage;
		public STRSubRecord MICO_SmallIcon;
		public FormID SCRI_ScriptId;
		public STRSubRecord DESC;
		public FormID YNAM_PickupSound;
		public FormID ZNAM_DropSound;
		public BOOK_DATASubRecord Data;

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
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "FULL":
							FULL = new STRSubRecord();
							FULL.Deserialize(stream, name);
							break;
						case "OBND":
							OBND_Bounds = new OBNDSubRecord();
							OBND_Bounds.Deserialize(stream, name);
							break;
						case "MODL":
							MODL = new MODLSubRecord();
							MODL.Deserialize(stream, name);
							break;
						case "SCRI":
							SCRI_ScriptId = new();
							SCRI_ScriptId.Deserialize(stream, name);
							break;
						case "DESC":
							DESC = new STRSubRecord();
							DESC.Deserialize(stream, name);
							break;
						case "YNAM":
							YNAM_PickupSound = new FormID();
							YNAM_PickupSound.Deserialize(stream, name);
							break;
						case "ICON":
							ICON_InventoryImage = new STRSubRecord();
							ICON_InventoryImage.Deserialize(stream, name);
							break;
						case "MICO":
							MICO_SmallIcon = new STRSubRecord();
							MICO_SmallIcon.Deserialize(stream, name);
							break;
						case "ZNAM":
							ZNAM_DropSound = new FormID();
							ZNAM_DropSound.Deserialize(stream, name);
							break;
						case "DATA":
							Data = new BOOK_DATASubRecord();
							Data.Deserialize(stream, name);
							break;
						default:
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}

		public override string ToString()
		{
			return EDID.ToString();
		}
	}

	public sealed class BOOK_DATASubRecord : SubRecord
	{
		public BOOK_DATAFlags Flags;
		public SkillType SKILL_Flags;
		public int Value;
		public float Weight;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Flags = (BOOK_DATAFlags)reader.ReadByte();
			SKILL_Flags = (SkillType)reader.ReadByte();
			Value = reader.ReadInt32();
			Weight = reader.ReadSingle();
		}
	}

	[Flags]
	public enum BOOK_DATAFlags
	{
		UnSet = 0,
		Unknown = 0x01,
		CantBeTaken = 0x02,
	}
}