using System.IO;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	public class GLOBRecord : Record
	{
		public STRSubRecord EDID = new();
		public BytesSubRecord FNAM_VarType = new();
		public FloatSubRecord FLTV_Value = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//reader.ReadBytes((int)size);
			EDID.Deserialize(reader, reader.ReadString(4));
			FNAM_VarType.Deserialize(reader, reader.ReadString(4));
			FLTV_Value.Deserialize(reader, reader.ReadString(4));
		}
	}
}