using Fallout.NET;

using Portland;

using TES3Unity.ESM;

namespace TES3Unity.ESS.Records
{
	public class REFRRecord : Record
	{
		public float[] Position { get; private set; }
		public float[] Rotation { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "DATA")
			{
				Position = new float[]
				{
						  reader.ReadSingle(),
						  reader.ReadSingle(),
						  reader.ReadSingle()
				};

				Rotation = new float[]
				{
						  reader.ReadSingle(),
						  reader.ReadSingle(),
						  reader.ReadSingle()
				};
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
