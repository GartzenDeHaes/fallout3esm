using System.Collections.Generic;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.LAND;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Form List
	/// </summary>
	public sealed class FLSTRecord : Record
	{
		public List<FormID> FormIDs = new();

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
						case "LNAM":
							FormIDs.Add(new FormID());
							FormIDs[FormIDs.Count - 1].Deserialize(stream, name);
							break;
						default:
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}