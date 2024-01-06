using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.DIAL;
using Fallout.NET.TES4.SubRecords.INFO;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fallout.NET.TES4.Records
{
	public class INFORecord : Record
	{
		public FormID ANAM_Speaker = new();
		public List<FormID> NAME_AddTopic = new();
		public INFO_DATASubRecord DATA = new();
		public FormID QSTI = new();
		public List<INFO_ResponseSubRecord> Responses = new();
		public List<FormID> TCLT = new List<FormID>();
		public List<FormID> TCLF = new List<FormID>();
		public ScriptSubRecordCollection ScriptCollection = new();
		public List<CTDASubRecord> CTDA_Conditions = new();
		public STRSubRecord RNAM_ResponseOverride = new();
		public SpeechChallange DNAME_SpeechCheck;
		public FormID KNAM_ActorValuePerk = new();
		public FormID LNAM_ListenAnimation = new();
		public FormID SNDD_SoundUnused = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//var bytes = reader.ReadBytes((int)size);
			string name;

			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));

			using (var stream = new BetterMemoryReader(bytes))
			{
				//var end = stream.Length;

				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "NAME":
							NAME_AddTopic.Add(new FormID());
							NAME_AddTopic[NAME_AddTopic.Count - 1].Deserialize(stream, name);
							break;
						case "DATA":
							DATA.Deserialize(stream, name);
							break;
						case "QSTI":
							QSTI.Deserialize(stream, name);
							break;
						case "ANAM":
							ANAM_Speaker.Deserialize(stream, name);
							break;
						case "TRDT":
							Responses.Add(new INFO_ResponseSubRecord());
							Responses[Responses.Count - 1].TRDT.Deserialize(stream, name);
							break;
						case "NAM1":
							Responses[Responses.Count - 1].NAM1_ResponseText.Deserialize(stream, name);
							break;
						case "NAM2":
							Responses[Responses.Count - 1].NAM2_ScriptNotes.Deserialize(stream, name);
							break;
						case "NAM3":
							Responses[Responses.Count - 1].NAM3_Edits.Deserialize(stream, name);
							break;
						case "RNAM":
							Debug.Assert(RNAM_ResponseOverride == null);
							RNAM_ResponseOverride.Deserialize(stream, name);
							break;
						case "SNAM":
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
							ScriptCollection.Deserialize(stream, name);
							break;
						case "NEXT":
							// Null, marker Marker to separate SCHR fields.
							stream.ReadBytes(stream.ReadUInt16());
							break;
						//case "SCTX":
						//	ScriptCollection.SCTX_ScriptSrc.Deserialize(stream, name);
						//	break;
						//case "SCRO":
						//	var globFormId = new FormID();
						//	globFormId.Deserialize(stream, name);
						//	ScriptCollection.SCRO_GlobalVars.Add(globFormId);
						//	break;
						case "CTDA":
							CTDA_Conditions.Add(new CTDASubRecord());
							CTDA_Conditions[CTDA_Conditions.Count - 1].Deserialize(stream, name);
							break;
						case "DNAM":
							{
								var scsize = stream.ReadUInt16();
								DNAME_SpeechCheck = (SpeechChallange)stream.ReadInt32();
							}
							break;
						case "KNAM":
							KNAM_ActorValuePerk.Deserialize(stream, name);
							break;
						case "LNAM":
							LNAM_ListenAnimation.Deserialize(stream, name);
							break;
						//case "SCDA":   // Compiled result script
						//	stream.ReadBytes(stream.ReadUInt16());
						//	break;
						case "SNDD":
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

			ArrayPool<byte>.Shared.Return(bytes);
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