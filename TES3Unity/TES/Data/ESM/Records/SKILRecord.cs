using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum SkillSpecification
	{
		Combat = 0, Magic, Stealth
	}

	public sealed class SKILRecord : Record
	{
		public int SkillId { get; private set; }
		public SkillData SKDT { get; private set; }
		public string Description { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "INDX")
			{
				SkillId = (int)reader.ReadIntRecord(dataSize);
			}
			else if (subRecordName == "SKDT")
			{
				SKDT = new SkillData
				{
					Attribute = reader.ReadInt32(),
					Specification = reader.ReadInt32(),
					UseValue = reader.ReadDoubleArray(4)
				};
			}
			else if (subRecordName == "DESC")
			{
				Description = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
		}
	}
}
