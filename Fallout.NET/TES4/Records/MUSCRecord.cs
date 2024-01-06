using System;
using System.IO;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Music Type
	/// </summary>
	public sealed class MUSCRecord : Record
	{
		public STRSubRecord FNAM_Filename = new();
		//public FloatSubRecord ANAM_PositiveValuesCauseMusicToLoop = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var startPos = reader.Position;
			var endPos = startPos + size;

			var name = reader.ReadString(4);
			UnityEngine.Debug.Assert(name == "EDID");
			EDID = STRSubRecord.Read(reader, name);

			if (reader.Position < endPos)
			{
				name = reader.ReadString(4);
				//if (name == "ANAM")
				//{
				//	ANAM_PositiveValuesCauseMusicToLoop.Deserialize(reader, name);
				//	name = reader.ReadString(4);
				//}
				UnityEngine.Debug.Assert(name == "FNAM");
				FNAM_Filename.Deserialize(reader, name);
			}

			UnityEngine.Debug.Assert(startPos + size == reader.Position);
		}
	}
}