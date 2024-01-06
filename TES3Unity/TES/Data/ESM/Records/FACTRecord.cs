using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class FACTRecord : Record
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public string Rank { get; private set; }
		public FactionData Data { get; private set; }
		public string FactionName { get; private set; }
		public int Reaction { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "RNAM")
			{
				Rank = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FADT")
			{
				var data = new FactionData();
				data.AttributeID1 = reader.ReadInt32();
				data.AttributeID2 = reader.ReadInt32();

				var rankData = new FactionRankData[10];
				for (var i = 0; i < rankData.Length; i++)
				{
					rankData[i] = new FactionRankData
					{
						Attribute1 = reader.ReadInt32(),
						Attribute2 = reader.ReadInt32(),
						FirstSkill = reader.ReadInt32(),
						SecondSkill = reader.ReadInt32(),
						Faction = reader.ReadInt32()
					};
				}

				data.Data = rankData;
				data.SkillID = reader.ReadInt32Array(6);
				data.Unknown1 = reader.ReadInt32();
				data.Flags = reader.ReadInt32();

				Data = data;
			}
			else if (subRecordName == "ANAM")
			{
				FactionName = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "INTV")
			{
				Reaction = (int)reader.ReadIntRecord(dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
