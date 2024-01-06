using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum EnchantType
	{
		CastOne = 0,
		CastStrikes = 1,
		CastWhenUsed = 2,
		ConstantEffect = 3
	}

	public enum EnchantRangeType
	{
		Self = 0,
		Touch = 1,
		Target = 2
	}

	public class ENCHRecord : Record
	{
		public string Id { get; private set; }
		public EnchantData Data { get; private set; }
		public SingleEnchantData SingleData { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "ENDT")
			{
				Data = new EnchantData
				{
					Type = (EnchantType)reader.ReadInt32(),
					EnchantCost = reader.ReadInt32(),
					Charge = reader.ReadInt32(),
					AutoCalc = reader.ReadInt32(),
				};
			}
			else if (subRecordName == "ENAM")
			{
				SingleData = new SingleEnchantData
				{
					EffectID = reader.ReadInt16(),
					SkillID = reader.ReadByte(),
					AttributeID = reader.ReadByte(),
					RangeType = (EnchantRangeType)reader.ReadInt32(),
					Area = reader.ReadInt32(),
					Duration = reader.ReadInt32(),
					MagMin = reader.ReadInt32(),
					MagMax = reader.ReadInt32()
				};
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
