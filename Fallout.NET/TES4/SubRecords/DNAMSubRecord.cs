using Fallout.NET.Core;
using System;

namespace Fallout.NET.TES4.SubRecords.STAT
{
	public class DNAMSubRecord : SubRecord
	{
		public byte SkillValue_Barter;
		public byte SkillValue_BigGuns;
		public byte SkillValue_EnergyWeapons;
		public byte SkillValue_Explosives;
		public byte SkillValue_Lockpick;
		public byte SkillValue_Medicine;
		public byte SkillValue_MeleeWeapons;
		public byte SkillValue_Repair;
		public byte SkillValue_Science;
		public byte SkillValue_SmallGuns;
		public byte SkillValue_Sneak;
		public byte SkillValue_Speech;
		public byte SkillValue_Throwing;// Unused
		public byte SkillValue_Unarmed;
		public byte SkillOffset_Barter;
		public byte SkillOffset_BigGuns;
		public byte SkillOffset_EnergyWeapons;
		public byte SkillOffset_Explosives;
		public byte SkillOffset_Lockpick;
		public byte SkillOffset_Medicine;
		public byte SkillOffset_MeleeWeapons;
		public byte SkillOffset_Repair;
		public byte SkillOffset_Science;
		public byte SkillOffset_SmallGuns;
		public byte SkillOffset_Sneak;
		public byte SkillOffset_Speech;
		public byte SkillOffset_Throwing;// Unused
		public byte SkillOffset_Unarmed;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			SkillValue_Barter = reader.ReadByte();
			SkillValue_BigGuns = reader.ReadByte();
			SkillValue_EnergyWeapons = reader.ReadByte();
			SkillValue_Explosives = reader.ReadByte();
			SkillValue_Lockpick = reader.ReadByte();
			SkillValue_Medicine = reader.ReadByte();
			SkillValue_MeleeWeapons = reader.ReadByte();
			SkillValue_Repair = reader.ReadByte();
			SkillValue_Science = reader.ReadByte();
			SkillValue_Speech = reader.ReadByte();
			SkillValue_Unarmed = reader.ReadByte();
			SkillValue_Throwing = reader.ReadByte();// Unused
			SkillValue_Sneak = reader.ReadByte();
			SkillOffset_Barter = reader.ReadByte();
			SkillOffset_BigGuns = reader.ReadByte();
			SkillOffset_EnergyWeapons = reader.ReadByte();
			SkillOffset_Explosives = reader.ReadByte();
			SkillOffset_Lockpick = reader.ReadByte();
			SkillOffset_Medicine = reader.ReadByte();
			SkillOffset_MeleeWeapons = reader.ReadByte();
			SkillOffset_Repair = reader.ReadByte();
			SkillOffset_Science = reader.ReadByte();
			SkillOffset_SmallGuns = reader.ReadByte();
			SkillOffset_Sneak = reader.ReadByte();
			SkillOffset_Speech = reader.ReadByte();
			SkillOffset_Throwing = reader.ReadByte();// Unused
			SkillOffset_Unarmed = reader.ReadByte();
			SkillValue_SmallGuns = reader.ReadByte();

		}
	}
}
