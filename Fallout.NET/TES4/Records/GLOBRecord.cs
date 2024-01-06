using System.IO;
using System.Xml.Linq;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	public class GLOBRecord : Record
	{
		public byte[] FNAM_VarType;
		public float FLTV_Value;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//reader.ReadBytes((int)size);
			EDID = STRSubRecord.Read(reader, reader.ReadString(4));
			FNAM_VarType = BytesSubRecord.Read(reader, reader.ReadString(4));
			FLTV_Value = FloatSubRecord.Read(reader, reader.ReadString(4));
		}
	}
}