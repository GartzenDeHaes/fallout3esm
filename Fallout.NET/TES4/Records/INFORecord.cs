using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.DIAL;
using Fallout.NET.TES4.SubRecords.INFO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fallout.NET.TES4.Records
{
	public class INFORecord : Record
	{
		public FormID ANAM_Speaker;
		public List<FormID> NAME_AddTopic = new();
		public INFO_DATASubRecord DATA;
		public FormID QSTI;
		public List<INFO_ResponseSubRecord> Responses = new();
		public List<FormID> TCLT = new List<FormID>();
		public List<FormID> TCLF = new List<FormID>();
		public SCHRSubRecord SCHR_ResultScriptData;
		public STRSubRecord SCTX_ResultScriptSrc;
		public List<FormID> SCRO_GlobalVars = new();
		public List<CTDASubRecord> CTDA_Conditions = new();
		public STRSubRecord RNAM_ResponseOverride;
		public SpeechChallange DNAME_SpeechCheck;
		public FormID KNAM_ActorValuePerk;
		public FormID LNAM_ListenAnimation;
		public FormID SNDD_SoundUnused;

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
						case "NAME":
							NAME_AddTopic.Add(new FormID());
							NAME_AddTopic[NAME_AddTopic.Count - 1].Deserialize(stream, name);
							break;
						case "DATA":
							DATA = new INFO_DATASubRecord();
							DATA.Deserialize(stream, name);
							break;
						case "QSTI":
							QSTI = new FormID();
							QSTI.Deserialize(stream, name);
							break;
						case "ANAM":
							ANAM_Speaker = new FormID();
							ANAM_Speaker.Deserialize(stream, name);
							break;
						case "TRDT":
							Responses.Add(new INFO_ResponseSubRecord());

							Responses[Responses.Count - 1].TRDT = new INFO_TRDTSubrecord();
							Responses[Responses.Count - 1].TRDT.Deserialize(stream, name);

							break;
						case "NAM1":
							var nam1 = new STRSubRecord();
							nam1.Deserialize(stream, name);
							Responses[Responses.Count - 1].NAM1_ResponseText = nam1;
							break;
						case "NAM2":
							var nam2 = new STRSubRecord();
							nam2.Deserialize(stream, name);
							Responses[Responses.Count - 1].NAM2_ScriptNotes = nam2;
							break;
						case "NAM3":
							var nam3 = new STRSubRecord();
							nam3.Deserialize(stream, name);
							Responses[Responses.Count - 1].NAM3_Edits = nam3;
							break;
						case "RNAM":
							Debug.Assert(RNAM_ResponseOverride == null);
							RNAM_ResponseOverride = new STRSubRecord();
							RNAM_ResponseOverride.Deserialize(stream, name);
							break;
						case "SNAM":
							Responses[Responses.Count - 1].SNAM_SpeakerAnim = new();
							Responses[Responses.Count - 1].SNAM_SpeakerAnim.Deserialize(stream, name);
							break;
						case "TCLT":
							var tclt = new FormID();
							tclt.Deserialize(stream, name);
							TCLT.Add(tclt);
							break;
						case "TCLF":
							var tclf = new FormID();
							tclf.Deserialize(stream, name);
							TCLF.Add(tclf);
							break;
						case "SCHR":
							SCHR_ResultScriptData = new();
							SCHR_ResultScriptData.Deserialize(stream, name);
							break;
						case "NEXT":
							// Null, marker Marker to separate SCHR fields.
							stream.ReadBytes(stream.ReadUInt16());
							break;
						case "SCTX":
							SCTX_ResultScriptSrc = new();
							SCTX_ResultScriptSrc.Deserialize(stream, name);
							break;
						case "SCRO":
							SCRO_GlobalVars.Add(new FormID());
							SCRO_GlobalVars[SCRO_GlobalVars.Count - 1].Deserialize(stream, name);
							break;
						case "CTDA":
							CTDA_Conditions.Add(new CTDASubRecord());
							CTDA_Conditions[CTDA_Conditions.Count - 1].Deserialize(stream, name);
							break;
						case "DNAM":
							{
								var ssize = stream.ReadUInt16();
								DNAME_SpeechCheck = (SpeechChallange)stream.ReadInt32();
							}
							break;
						case "KNAM":
							KNAM_ActorValuePerk = new();
							KNAM_ActorValuePerk.Deserialize(stream, name);
							break;
						case "LNAM":
							LNAM_ListenAnimation = new();
							LNAM_ListenAnimation.Deserialize(stream, name);
							break;
						case "SCDA":   // Compiled result script
							stream.ReadBytes(stream.ReadUInt16());
							break;
						case "SNDD":
							SNDD_SoundUnused = new();
							SNDD_SoundUnused.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown INFO sub record {name}");
							var rest = stream.ReadUInt16();
							if (rest == 0)
							{
								UnityEngine.Debug.LogError("Invalid INFO");
								return;
							}
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}

		public enum SpeechChallange
		{
			//Speech Challenge Values
			None = 0,// 	---
			VeryEasy = 1, 	
			Easy = 2,
			Average = 3,
			Hard = 4,
			VeryHard = 5,
		}
	}
}