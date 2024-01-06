using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.STAT;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Fallout.NET.TES4.Records
{
	public sealed class NPC_Record : Record
	{
		public STRSubRecord FULL = new();
		public STRSubRecord MODL = new();
		public OBNDSubRecord OBND_Bounds = new();
		public ACBSSubRecord ACBS_MoreStats = new();
		public FormID INAM_DeathItem = new();
		public FormID VTCK_Voice = new();
		public FormID TPLT_TemplateNpcOrLvln = new();
		public FormID RNAM_Race = new();
		public ImpactMaterialType ImpactMaterial;
		public FormID EITM_UnarmedAttackEffect = new();
		public FormID SCRI = new();
		public FormID CNAM_Class = new();
		public FormID HNAM_Head = new();
		public FormID ENAM_Eyes = new();
		public FloatSubRecord NAM6_Height = new();
		public FloatSubRecord NAM7_Weight = new();
		public FormID ZNAM_CombatStyle = new();
		public List<FormID> PKID_AiPackages = new List<FormID>();
		public List<FormID> PNAM_HeadPart = new List<FormID>();
		public List<SNAMSubRecord> SNAM_Factions = new List<SNAMSubRecord>();
		public DATASubRecord DATA_Stats = new();
		public DNAMSubRecord DNAM_Skills = new();
		public COEDSubRecord COED_Ownership = new();
		public List<FormID> SPLO_Spells = new();

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
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "FULL":
							FULL.Deserialize(stream, name);
							//Debug.Log($"NPC {FULL.Value}");
							break;
						case "OBND":
							OBND_Bounds.Deserialize(stream, name);
							break;
						case "MODL":
							MODL.Deserialize(stream, name);
							break;
						case "ACBS":
							ACBS_MoreStats.Deserialize(stream, name);
							break;
						case "SNAM":
							var snam = new SNAMSubRecord();
							snam.Deserialize(stream, name);
							SNAM_Factions.Add(snam);
							break;
						case "INAM":
							INAM_DeathItem.Deserialize(stream, name);
							break;
						case "VTCK":
							VTCK_Voice.Deserialize(stream, name);
							break;
						case "TPLT":
							TPLT_TemplateNpcOrLvln.Deserialize(stream, name);
							break;
						case "RNAM":
							RNAM_Race.Deserialize(stream, name);
							break;
						case "EITM":
							EITM_UnarmedAttackEffect.Deserialize(stream, name);
							break;
						case "SCRI":
							SCRI.Deserialize(stream, name);
							break;
						case "PKID":
							var pkid = new FormID();
							pkid.Deserialize(stream, name);
							PKID_AiPackages.Add(pkid);
							break;
						case "CNAM":
							CNAM_Class.Deserialize(stream, name);
							break;
						case "PNAM":
							var pnam = new FormID();
							pnam.Deserialize(stream, name);
							PNAM_HeadPart.Add(pnam);
							break;
						case "HNAM":
							HNAM_Head.Deserialize(stream, name);
							break;
						case "ENAM":
							ENAM_Eyes.Deserialize(stream, name);
							break;
						case "ZNAM":
							ZNAM_CombatStyle.Deserialize(stream, name);
							break;
						case "DNAM":
							DNAM_Skills.Deserialize(stream, name);
							break;
						case "DATA":   // Zero length
							DATA_Stats.Deserialize(stream, name);
							break;
						case "NAM6":
							NAM6_Height.Deserialize(stream, name);
							break;
						case "NAM7":
							NAM7_Weight.Deserialize(stream, name);
							break;
						case "COED":
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

			ArrayPool<byte>.Shared.Return(bytes);
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
