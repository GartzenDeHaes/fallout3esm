using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class MISCRecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public MiscData Data { get; private set; }
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
			else if (subRecordName == "MCDT")
			{
				Data = new MiscData
				{
					Weight = reader.ReadSingle(),
					Value = reader.ReadUInt32(),
					Unknown = reader.ReadUInt32()
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
