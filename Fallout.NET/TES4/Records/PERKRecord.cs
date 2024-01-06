using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.INFO;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Perk
	/// </summary>
	public sealed class PERKRecord : Record
	{
		public STRSubRecord FULL = new();
		public STRSubRecord DESC = new();
		public STRSubRecord ICON_LargeIcon = new();
		public STRSubRecord MICO_SmallIcon = new();
		public List<CTDASubRecord> CTDA_Conditions = new(); // https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/Subrecords/CTDA.md
		public YesNo DATA_Trait;
		public byte DATA_MinLevel;
		public byte DATA_Ranks;
		public YesNo DATA_Playable;
		public YesNo DATA_Hidden;
		public List<EffectSubRecord> Effects = new List<EffectSubRecord>();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "FULL":
							FULL.Deserialize(stream, name);
							break;
						case "DESC":
							DESC.Deserialize(stream, name);
							break;
						case "ICON":
							ICON_LargeIcon.Deserialize(stream, name);
							break;
						case "MICO":
							MICO_SmallIcon.Deserialize(stream, name);
							break;
						case "CTDA":
							CTDA_Conditions.Add(new CTDASubRecord());
							CTDA_Conditions[CTDA_Conditions.Count - 1].Deserialize(stream, name);
							break;
						case "DATA":
							var datalen = stream.ReadUInt16();
							DATA_Trait = (YesNo)stream.ReadByte();
							DATA_MinLevel = stream.ReadByte();
							DATA_Ranks = stream.ReadByte();
							DATA_Playable = (YesNo)stream.ReadByte();
							if (datalen > 4)
							{
								DATA_Hidden = (YesNo)stream.ReadByte();
							}
							break;
						case "PRKE":
							Effects.Add(new EffectSubRecord(stream));
							break;
						default:
							UnityEngine.Debug.Log($"Unknown PERK {EDID} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}

		public enum YesNo
		{
			No = 0,
			Yes = 1
		}

		public enum PerkeType
		{
			QuestStage = 0,
			Ability = 1,
			EntryPoint = 2,
		}

		public enum EPF3Flags
		{
			None = 0,
			RunImmediately = 0x01
		}

		// https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/PERK.md
		// https://tes5edit.github.io/fopdoc/Fallout3/Records/PERK.html
		public class EffectSubRecord
		{
			public struct DATA_Quest
			{
				public uint QuestFormId;
				public sbyte QuestStage;
			}

			public struct DATA_Ability
			{
				public uint SpelFormId;
			}

			public struct DATA_EntryPoint
			{
				public EntryPointType EntryPoint;
				public byte Function;
				public byte PerkConditionTabCount;
			}

			public class PRKC_SubRecord
			{
				public RunOnType RunOn;
				public List<CTDASubRecord> Conditions = new();
			}

			public PerkeType PRKE_Type;
			public byte PRKE_Rank;
			public byte PRKE_Priority;
			public DATA_Quest DATA_TypeQuest;
			public DATA_Ability DATA_TypeAbility;
			public DATA_EntryPoint DATA_TypeEntryPoint;
			public List<CTDASubRecord> Conditions = new();
			public byte EPFT_EntryPointFuctionType;
			public byte[] EPFD_EntryPointFunctionData;
			public STRSubRecord EPF2_ButtonLabel = new();
			public EPF3Flags EPF3_ScriptFlags;
			public ScriptSubRecordCollection EmbeddedScripts = new();
			public List<PRKC_SubRecord> PRKC_Conditions = new();

			public EffectSubRecord(BetterMemoryReader reader)
			{
				var recsize = reader.ReadUInt16();

				PRKE_Type = (PerkeType)reader.ReadByte();
				PRKE_Rank = reader.ReadByte();
				PRKE_Priority = reader.ReadByte();

				string name = reader.ReadString(4);
				UnityEngine.Debug.Assert(name == "DATA");
				var datalen = reader.ReadUInt16();

				if (PRKE_Type == PerkeType.QuestStage)
				{
					DATA_TypeQuest = new();
					DATA_TypeQuest.QuestFormId = reader.ReadUInt32();
					DATA_TypeQuest.QuestStage = (sbyte)reader.ReadByte();
					reader.ReadByte();
					reader.ReadByte();
					reader.ReadByte();
				}
				else if (PRKE_Type == PerkeType.Ability)
				{
					DATA_TypeAbility = new();
					DATA_TypeAbility.SpelFormId = reader.ReadUInt32();
				}
				else if (PRKE_Type == PerkeType.EntryPoint)
				{
					DATA_TypeEntryPoint = new();
					DATA_TypeEntryPoint.EntryPoint = (EntryPointType)reader.ReadByte();
					DATA_TypeEntryPoint.Function = reader.ReadByte();
					DATA_TypeEntryPoint.PerkConditionTabCount = reader.ReadByte();
				}
				else
				{
					throw new Exception($"Invalid PERK effect type of {(int)PRKE_Type}");
				}

				name = reader.ReadString(4);
				while (name == "PRKC")
				{
					datalen = reader.ReadUInt16();

					PRKC_SubRecord prkc = new();
					prkc.RunOn = (RunOnType)(sbyte)reader.ReadByte();

					PRKC_Conditions.Add(prkc);

					name = reader.ReadString(4);

					while (name == "CTDA")
					{
						var cond = new CTDASubRecord();
						cond.Deserialize(reader, name);
						prkc.Conditions.Add(cond);

						name = reader.ReadString(4);
					}
				}

				while (name != "PRKF")
				{
					switch (name)
					{
						case "EPFT":
							datalen = reader.ReadUInt16();
							UnityEngine.Debug.Assert(datalen == 1);
							EPFT_EntryPointFuctionType = reader.ReadByte();
							break;
						case "EPFD":
							datalen = reader.ReadUInt16();
							//UnityEngine.Debug.Assert(datalen == 1);
							EPFD_EntryPointFunctionData = reader.ReadBytes(datalen);
							break;
						case "EPF2":
							EPF2_ButtonLabel.Deserialize(reader, name);
							break;
						case "EPF3":
							datalen = reader.ReadUInt16();
							UnityEngine.Debug.Assert(datalen == 2);
							EPF3_ScriptFlags = (EPF3Flags)reader.ReadUInt16();
							break;
						case "SCHR":
							EmbeddedScripts.Deserialize(reader, name);
							break;
						//case "SCTX":
						//	EmbeddedScripts.SCTX_ScriptSrc.Deserialize(reader, name);
						//	break;
						//case "SCRO":
						//	var globFormId = new FormID();
						//	globFormId.Deserialize(reader, name);
						//	EmbeddedScripts.SCRO_GlobalVars.Add(globFormId);
						//	break;
						case "NEXT":
							// Null, marker Marker to separate SCHR fields.
							reader.ReadBytes(reader.ReadUInt16());
							break;
						case "SCDA":
							// Compiled script
							reader.ReadBytes(reader.ReadUInt16());
							break;
						default:
							throw new Exception($"Unknown rec {name}");
					}

					name = reader.ReadString(4);
				}

				UnityEngine.Debug.Assert(name == "PRKF");
				var extra = reader.ReadInt16();
			}
		}
	}

	public enum EntryPointType
	{
		CalculateWeaponDamage = 0,
		CalculateMyCriticalHitChance = 1,
		CalculateMyCriticalHitDamage = 2,
		CalculateWeaponAttackAPCost = 3,
		CalculateMineExplodeChance = 4,
		AdjustRangePenalty = 5,
		AdjustLimbDamage = 6,
		CalculateWeaponRange = 7,
		CalculateToHitChance = 8,
		AdjustExperiencePoints = 9,
		AdjustGainedSkillPoints = 10,
		AdjustBookSkillPoints = 11,
		ModifyRecoveredHealth = 12,
		CalculateInventoryAPCost = 13,
		GetDisposition = 14,
		GetShouldAttack = 15,
		GetShouldAssist = 16,
		CalculateBuyPrice = 17,
		GetBadKarma = 18,
		GetGoodKarma = 19,
		IgnoreLockedTerminal = 20,
		AddLeveledListOnDeath = 21,
		GetMaxCarryWeight = 22,
		ModifyAddictionChance = 23,
		ModifyAddictionDuration = 24,
		ModifyPositiveChemDuration = 25,
		AdjustDrinkingRadiation = 26,
		Activate = 27,
		MysteriousStranger = 28,
		HasParalyzingPalm = 29,
		HackingScienceBonus = 30,
		IgnoreRunningDuringDetection = 31,
		IgnoreBrokenLock = 32,
		HasConcentratedFire = 33,
		CalculateGunSpread = 34,
		PlayerKillAPReward = 35,
		ModifyEnemyCriticalHitChance = 36,
	}
}
