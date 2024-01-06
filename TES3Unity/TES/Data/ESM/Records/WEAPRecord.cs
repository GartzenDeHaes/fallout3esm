using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class WEAPRecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public WeaponData Data { get; private set; }
		public string Icon { get; private set; }
		public string Enchantment { get; private set; }
		public string Script { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "MODL")
			{
				Model = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "WPDT")
			{
				Data = new WeaponData
				{
					Weight = reader.ReadSingle(),
					Value = reader.ReadInt32(),
					Type = reader.ReadInt16(),
					Health = reader.ReadInt16(),
					Speed = reader.ReadSingle(),
					Reach = reader.ReadSingle(),
					EnchantPts = reader.ReadInt16(),
					ChopMin = reader.ReadByte(),
					ChopMax = reader.ReadByte(),
					SlashMin = reader.ReadByte(),
					SlashMax = reader.ReadByte(),
					ThrustMin = reader.ReadByte(),
					ThrustMax = reader.ReadByte(),
					Flags = reader.ReadInt32(),
				};
			}
			else if (subRecordName == "ITEX")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "ENAM")
			{
				Enchantment = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
