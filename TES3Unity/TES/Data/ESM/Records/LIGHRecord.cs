using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum LightFlags
	{
		Dynamic = 0x0001,
		CanCarry = 0x0002,
		Negative = 0x0004,
		Flicker = 0x0008,
		Fire = 0x0010,
		OffDefault = 0x0020,
		FlickerSlow = 0x0040,
		Pulse = 0x0080,
		PulseSlow = 0x0100
	}

	public sealed class LIGHRecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Name { get; private set; }
		public LightData? Data { get; private set; }
		public string Script { get; private set; }
		public string Icon { get; private set; }
		public string Model { get; private set; }
		public string Sound { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "LHDT")
			{
				Data = new LightData
				{
					Weight = reader.ReadSingle(),
					Value = reader.ReadInt32(),
					Time = reader.ReadInt32(),
					Radius = reader.ReadInt32(),
					Red = reader.ReadByte(),
					Green = reader.ReadByte(),
					Blue = reader.ReadByte(),
					NullByte = reader.ReadByte(),
					Flags = reader.ReadInt32()
				};
			}
			else if (subRecordName == "SCPT" || subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "ITEX")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "MODL")
			{
				Model = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "SNAM")
			{
				Sound = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
