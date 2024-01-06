using Fallout.NET.Core;

using System;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Game Setting
	/// </summary>
	public class GMSTRecord : Record
	{
		protected STRSubRecord _stringData;
		protected UInt32SubRecord _intData;
		protected FloatSubRecord _floatData;

		public SubRecord DATA;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;

				while (stream.Position < end)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "DATA":
							switch (EDID[0])
							{
								case 's':
									DATA = new STRSubRecord();
									((STRSubRecord)DATA).Deserialize(stream, name);
									break;
								case 'f':
									DATA = new FloatSubRecord();
									((FloatSubRecord)DATA).Deserialize(stream, name);
									break;
								default:
									DATA = new UInt32SubRecord();
									((UInt32SubRecord)DATA).Deserialize(stream, name);
									break;
							}
							break;
					}
				}
			}
		}
	}
}