using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.STAT;

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Fallout.NET.TES4.Records
{
	public sealed class NPC_Record : Record
	{
		public STRSubRecord EDID;
		public STRSubRecord FULL;
		public STRSubRecord MODL;
		public OBNDSubRecord OBND_Bounds;
		public ACBSSubRecord ACBS_MoreStats;
		public FormID INAM_DeathItem;
		public FormID VTCK_Voice;
		public FormID TPLT_TemplateNpcOrLvln;
		public FormID RNAM_Race;
		public ImpactMaterialType ImpactMaterial;
		public FormID EITM_UnarmedAttackEffect;
		public FormID SCRI;
		public FormID CNAM_Class;
		public FormID HNAM_Head;
		public FormID ENAM_Eyes;
		public FloatSubRecord NAM6_Height;
		public FloatSubRecord NAM7_Weight;
		public FormID ZNAM_CombatStyle;
		public List<FormID> PKID_AiPackages = new List<FormID>();
		public List<FormID> PNAM = new List<FormID>();
		public List<SNAMSubRecord> SNAM_Factions = new List<SNAMSubRecord>();
		public DATASubRecord DATA_Stats;
		public DNAMSubRecord DNAM_Skills;
		public COEDSubRecord COED_Ownership;
		public List<FormID> SPLO_Spells = new();

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
							//Debug.Log($"NPC {FULL.Value}");
							break;
						case "OBND":
							OBND_Bounds = new OBNDSubRecord();
							OBND_Bounds.Deserialize(stream, name);
							break;
						case "MODL":
							MODL = new STRSubRecord();
							MODL.Deserialize(stream, name);
							break;
						case "ACBS":
							ACBS_MoreStats = new ACBSSubRecord();
							ACBS_MoreStats.Deserialize(stream, name);
							break;
						case "SNAM":
							var snam = new SNAMSubRecord();
							snam.Deserialize(stream, name);
							SNAM_Factions.Add(snam);
							break;
						case "INAM":
							INAM_DeathItem = new FormID();
							INAM_DeathItem.Deserialize(stream, name);
							break;
						case "VTCK":
							VTCK_Voice = new FormID();
							VTCK_Voice.Deserialize(stream, name);
							break;
						case "TPLT":
							TPLT_TemplateNpcOrLvln = new FormID();
							TPLT_TemplateNpcOrLvln.Deserialize(stream, name);
							break;
						case "RNAM":
							RNAM_Race = new FormID();
							RNAM_Race.Deserialize(stream, name);
							break;
						case "EITM":
							EITM_UnarmedAttackEffect = new FormID();
							EITM_UnarmedAttackEffect.Deserialize(stream, name);
							break;
						case "SCRI":
							SCRI = new FormID();
							SCRI.Deserialize(stream, name);
							break;
						case "PKID":
							var pkid = new FormID();
							pkid.Deserialize(stream, name);
							PKID_AiPackages.Add(pkid);
							break;
						case "CNAM":
							CNAM_Class = new FormID();
							CNAM_Class.Deserialize(stream, name);
							break;
						case "PNAM":
							var pnam = new FormID();
							pnam.Deserialize(stream, name);
							PNAM.Add(pnam);
							break;
						case "HNAM":
							HNAM_Head = new FormID();
							HNAM_Head.Deserialize(stream, name);
							break;
						case "ENAM":
							ENAM_Eyes = new FormID();
							ENAM_Eyes.Deserialize(stream, name);
							break;
						case "ZNAM":
							ZNAM_CombatStyle = new FormID();
							ZNAM_CombatStyle.Deserialize(stream, name);
							break;
						case "DNAM":
							DNAM_Skills = new DNAMSubRecord();
							DNAM_Skills.Deserialize(stream, name);
							break;
						case "DATA":   // Zero length
							DATA_Stats = new DATASubRecord();
							DATA_Stats.Deserialize(stream, name);
							break;
						case "NAM6":
							NAM6_Height = new FloatSubRecord();
							NAM6_Height.Deserialize(stream, name);
							break;
						case "NAM7":
							NAM7_Weight = new FloatSubRecord();
							NAM7_Weight.Deserialize(stream, name);
							break;
						case "COED":
							COED_Ownership = new COEDSubRecord();
							COED_Ownership.Deserialize(stream, name);
							break;
						case "NAM4":
							var size2 = stream.ReadInt16();
							ImpactMaterial = (ImpactMaterialType)stream.ReadUInt32();
							break;
						case "SPLO":
							SPLO_Spells.Add(new FormID());
							SPLO_Spells[SPLO_Spells.Count -1].Deserialize(stream, name);
							break;
						case "NAM5":	// Unknown
						case "EAMT":   // Attack Animations
						case "CNTO":   // Inventory Container
						case "AIDT":   // AI Data
						case "HCLR":   // Hair color
						case "LNAM":	// Hair length
						case "FGGS":   // FaceGen Geometry-Symmetric
						case "FGGA":   // FaceGen Geometry-Asymmetric
						case "FGTS":   // FaceGen Texture-Symmetric
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown NPC subrecord {name}");
							var rest2 = stream.ReadUInt16();
							stream.ReadBytes(rest2);
							break;
					}
				}
			}
		}

		public override string ToString()
		{
			return EDID.ToString();
		}

		public class DATASubRecord
		{
			public int BaseHealth;
			public byte Strength;
			public byte Perception;
			public byte Endurance;
			public byte Charisma;
			public byte Intelligence;
			public byte Agility;
			public byte Luck;
			//Unused   uint8[] Only appears in older record versions.

			public void Deserialize(BetterReader reader, string name)
			{
				//base.Deserialize(reader, name);

				var rest = reader.ReadUInt16();

				using (var stream = new BetterMemoryReader(reader.ReadBytes(rest)))
				{
					BaseHealth = stream.ReadInt32();
					Strength = stream.ReadByte();
					Perception = stream.ReadByte();
					Endurance = stream.ReadByte();
					Charisma = stream.ReadByte();
					Intelligence = stream.ReadByte();
					Agility = stream.ReadByte();
					Luck = stream.ReadByte();
				}
			}
		}
	}
}
