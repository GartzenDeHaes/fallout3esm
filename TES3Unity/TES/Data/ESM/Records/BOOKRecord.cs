using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class BOOKRecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public string Ename { get; private set; }
		public BookData Data { get; private set; }
		public string Icon { get; private set; }
		public string Script { get; private set; }
		public string Text { get; private set; }

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
			else if (subRecordName == "ENAM")
			{
				Ename = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "BKDT")
			{
				Data = new BookData
				{
					Weight = reader.ReadSingle(),
					Value = reader.ReadInt32(),
					Scroll = reader.ReadInt32(),
					SkillID = reader.ReadInt32(),
					EnchantPts = reader.ReadInt32()
				};
			}
			else if (subRecordName == "ITEX")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "TEXT")
			{
				Text = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
