using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.DIAL;

using System;
using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// A DIAL record defines a dialog topic. There is a QSTI subrecord for each quest that contains responses for the topic. The DIAL record is followed by a group containing the responses (INFO subrecords).
	/// </summary>
	public class DIALRecord : Record
	{
		public STRSubRecord EDID;
		public List<FormID> QSTI_QuestAdded = new List<FormID>();
		public List<FormID> QSTR_QuestRemoved  = new List<FormID>();
		public STRSubRecord FULL_TopicName;
		public FloatSubRecord PNAM_Priority;
		public DIAL_DATASubRecord DATA;

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
						case "QSTI":
							var qsti = new FormID();
							qsti.Deserialize(stream, name);
							QSTI_QuestAdded.Add(qsti);
							break;
						case "QSTR":
							var qstr = new FormID();
							qstr.Deserialize(stream, name);
							QSTR_QuestRemoved.Add(qstr);
							break;
						case "FULL":
							FULL_TopicName = new STRSubRecord();
							FULL_TopicName.Deserialize(stream, name);
							break;
						case "PNAM":
							PNAM_Priority = new FloatSubRecord();
							PNAM_Priority.Deserialize(stream, name);
							break;
						case "DATA":
							DATA = new DIAL_DATASubRecord();
							DATA.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown DIAL sub record {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}