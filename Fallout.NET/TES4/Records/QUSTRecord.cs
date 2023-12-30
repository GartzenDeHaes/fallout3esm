using System;
using System.Collections.Generic;
using System.IO;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.DIAL;
using Fallout.NET.TES4.SubRecords.INFO;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum QuestFlags
	{
		StartGameEnabled = 0x01,
		UnknownBit2 = 0x02,// 	??
		AllowRepeatedConversationTopics = 0x04,
		AllowRepeatedStages = 0x08,
		UnknownBit5 = 0x10,// 	??
	}

	/// <summary>
	/// Note, flag #4 is probably barks or run globally outside of conversation
	/// </summary>
	public class QUSTRecord : Record
	{
		public STRSubRecord EDID = new();
		public FormID SCRI_Script = new();
		public STRSubRecord FULL = new();
		public FormID SCRI = new();
		public QuestFlags DATA_Flags;
		public byte DATA_Priority;
		public float DATA_QuestDelay;
		public List<CTDASubRecord> CTDA_Conditions = new();
		public List<StageSubRecord> Stages = new();
		public List<ObjectiveSubRecord> Objectives = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//reader.ReadBytes((int)size);
			var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;
				int srsize;

				while (stream.Position < end)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID.Deserialize(stream, name);
							continue;
						case "FULL":
							FULL.Deserialize(stream, name);
							continue;
						case "SCRI":
							SCRI.Deserialize(stream, name);
							continue;
						case "DATA":
							srsize = stream.ReadUInt16();
							DATA_Flags = (QuestFlags)stream.ReadByte();
							DATA_Priority = stream.ReadByte();
							if (srsize == 8)
							{
								stream.ReadByte();
								stream.ReadByte();
								DATA_QuestDelay = stream.ReadSingle();
							}
							else
							{
								UnityEngine.Debug.Assert(srsize == 2, srsize);
							}
							continue;
						case "CTDA":
							CTDA_Conditions.Add(new CTDASubRecord());
							CTDA_Conditions[CTDA_Conditions.Count - 1].Deserialize(stream, name);
							continue;
						default:
							break;
					}
					while (name == "INDX")
					{
						Stages.Add(new());
						srsize = stream.ReadUInt16();
						UnityEngine.Debug.Assert(srsize == 2);
						Stages[Stages.Count - 1].INDX_StateIndex = stream.ReadInt16();

						name = stream.ReadString(4);

						while (name == "QSDT")
						{
							var log = new LogEntrySubRecord();
							Stages[Stages.Count - 1].LogEntries.Add(log);

							srsize = stream.ReadInt16();
							UnityEngine.Debug.Assert(srsize == 1);
							log.Flags = (StageLogFlags)stream.ReadByte();

							name = stream.ReadString(4);

							while (name == "CTDA")
							{
								var ctda = new CTDASubRecord();
								ctda.Deserialize(stream, name);
								log.Conditions.Add(ctda);
								name = stream.ReadString(4);
							}

							if (name == "CNAM")
							{
								log.CNAM_LogText = new();
								log.CNAM_LogText.Deserialize(stream, name);
								name = stream.ReadString(4);
							}
							if (name == "NAM0")
							{
								log.NAM0_NextQuest = new FormID();
								log.NAM0_NextQuest.Deserialize(stream, name);
								name = stream.ReadString(4);
							}
							if (name == "SCHR")
							{
								log.SCHR_NextQuest = new();
								log.SCHR_NextQuest.Deserialize(stream, name);
								name = stream.ReadString(4);
							}
							if (name == "SCDA")
							{
								// Compiled result script
								stream.ReadBytes(stream.ReadUInt16());
								name = stream.ReadString(4);
							}
							if (name == "SCTX")
							{
								log.SCTX_ResultScriptSrc = new();
								log.SCTX_ResultScriptSrc.Deserialize(stream, name);
								name = stream.ReadString(4);
							}
							while (name == "SCRO")
							{
								log.SCRO_GlobalVars.Add(new FormID());
								log.SCRO_GlobalVars[log.SCRO_GlobalVars.Count - 1].Deserialize(stream, name);
								name = stream.ReadString(4);
							}
						}
					}
					while (name == "QOBJ")
					{
						var objtv = new ObjectiveSubRecord();
						Objectives.Add(objtv);

						srsize = stream.ReadInt16();
						UnityEngine.Debug.Assert(srsize == 4);
						objtv.INDX_ObjectiveIndex = stream.ReadInt32();
						name = stream.ReadString(4);

						UnityEngine.Debug.Assert(name == "NNAM", name);
						objtv.NNAM_Description = new();
						objtv.NNAM_Description.Deserialize(stream, name);
						name = stream.ReadString(4);

						while (name == "QSTA")
						{
							var target = new TargetSubRecord();
							target.Deserialize(stream, name);

							name = stream.ReadString(4);
							while (name == "CTDA")
							{
								var cond = new CTDASubRecord();
								cond.Deserialize(stream, name);
								target.Conditions.Add(cond);
								name = stream.ReadString(4);
							}
						}
					}
				}
				if (stream.Position < end)
				{
					UnityEngine.Debug.Log($"Should be EOF {name}");
				}

				//while (stream.Position < end)
				//{
				//	name = stream.ReadString(4);

				//	switch (name)
				//	{
				//		case "EDID":
				//			EDID.Deserialize(stream, name);
				//			break;
				//		case "FULL":
				//			FULL.Deserialize(stream, name);
				//			break;
				//		case "SCRI":
				//			SCRI.Deserialize(stream, name);
				//			break;
				//		case "ICON":
				//			ICON.Deserialize(stream, name);
				//			break;
				//		case "MICO":
				//			MICO.Deserialize(stream, name);
				//			break;
				//		case "DATA":
				//			srsize = stream.ReadUInt16();
				//			UnityEngine.Debug.Assert(srsize == 8);
				//			DATA_Flags = (QuestFlags)stream.ReadByte();
				//			DATA_Priority = stream.ReadByte();
				//			stream.ReadByte();
				//			stream.ReadByte();
				//			DATA_QuestDelay = stream.ReadSingle();
				//			break;
				//		case "CTDA":
				//			CTDA_Conditions.Add(new CTDASubRecord());
				//			CTDA_Conditions[CTDA_Conditions.Count - 1].Deserialize(stream, name);
				//			break;
				//		case "INDX":
				//			Stages.Add(new());
				//			Stages[Stages.Count - 1].Deserialize(stream, name);
				//			break;
				//		case "QSDT":
				//			Stages[Stages.Count - 1].LogEntries.Add(new());
				//			Stages[Stages.Count - 1].LogEntries[Stages[Stages.Count - 1].LogEntries.Count - 1].Deserialize(stream, name);
				//			break;
				//		case "QOBJ":
				//			Objectives.Add(new());
				//			Objectives[Objectives.Count - 1].Deserialize(stream, name);
				//			break;
				//		case "QSTA":
				//			Objectives[Objectives.Count - 1].Targets.Add(new());
				//			Objectives[Objectives.Count - 1].Targets[Objectives[Objectives.Count - 1].Targets.Count - 1].Deserialize(stream, name);
				//			break;
				//		default:
				//			UnityEngine.Debug.Log($"Unknown QUST sub record {name}");
				//			var rest = stream.ReadUInt16();
				//			stream.ReadBytes(rest);
				//			break;
				//	}
				//}
			}
		}
	}

	[Flags]
	public enum StageFlags
	{
		None = 0,
		StartUpStage = 0x02,
		ShutDownStage = 0x04,
		KeepInstanceDataFromHereOn = 0x08,
	}

	public sealed class StageSubRecord
	{
		public short INDX_StateIndex;

		public List<LogEntrySubRecord> LogEntries = new();
	}

	public enum StageLogFlags
	{
		CompleteQuest = 0x01,
		FailQuest = 0x02,
	}

	public sealed class LogEntrySubRecord
	{
		public StageLogFlags Flags;
		public List<CTDASubRecord> Conditions = new();
		public STRSubRecord CNAM_LogText;
		public FormID NAM0_NextQuest;
		public SCHRSubRecord SCHR_NextQuest;
		public STRSubRecord SCTX_ResultScriptSrc;
		public List<FormID> SCRO_GlobalVars = new();
	}

	public sealed class ObjectiveSubRecord : SubRecord
	{
		public int INDX_ObjectiveIndex;
		public STRSubRecord NNAM_Description;

		public List<TargetSubRecord> Targets = new();
	}

	public enum TargetFlags
	{
		None = 0,
		CompassMarkerIgnoresLocks = 0x01,
	}

	public sealed class TargetSubRecord : SubRecord
	{
		public uint TargetFormId;
		public TargetFlags Flags;
		public List<CTDASubRecord> Conditions = new();

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			UnityEngine.Debug.Assert(Size == 8);
			TargetFormId = reader.ReadUInt32();
			Flags = (TargetFlags)reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
		}
	}
}
