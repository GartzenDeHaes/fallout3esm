using System.Linq;
using System.Runtime.ConstrainedExecution;

using Fallout.NET.Core;

using Portland.CodeDom.Operators;

using UnityEditor;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	///  	Actor Values/Perk Tree Graphics
	/// </summary>
	/// <remarks>
	/// The AVIF record is Actor Value information.
	///
	///	Most records are simple but the player skills have perk tree graphical 
	///	information in them as well in field groups(below thick line repeats 
	///	for each index).
	///
	/// Note: Smithing and Pickpocket are different because the initial CNAM does 
	/// not match up(Pickpocket would start at Cutpurse) and the AVSK has different 
	/// data throughout.It's possible the record-level CNAM has a different meaning 
	/// than assumed.
	///
	/// Perk tree information:
	///
	///  - This does not change prerequisites. Individual PERK records have CTDA 
	///    for perk requirements that allow you to skip perks if removed.
	///    
	///  - coordinates are given for each point along with any lines to other points.
	/// </remarks>
	public sealed class AVIFRecord : Record
	{
		public STRSubRecord EDID;
		public STRSubRecord FULL;
		public STRSubRecord DESC;
		public STRSubRecord ICON;
		public STRSubRecord MICO;
		public STRSubRecord ANAM;

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
							EDID = new STRSubRecord();
							EDID.Deserialize(stream, name);
							break;
						case "FULL":
							FULL = new STRSubRecord();
							FULL.Deserialize(stream, name);
							break;
						case "DESC":
							DESC = new STRSubRecord();
							DESC.Deserialize(stream, name);
							break;
						case "ICON":
							ICON = new STRSubRecord();
							ICON.Deserialize(stream, name);
							break;
						case "ANAM":
							ANAM = new STRSubRecord();
							ANAM.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown AVIF subrecord {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}