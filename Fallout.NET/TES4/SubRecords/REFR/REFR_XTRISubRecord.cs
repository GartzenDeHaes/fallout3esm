using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public class REFR_XTRISubRecord : SubRecord
	{
		public REFR_XTRITypes Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (REFR_XTRITypes)reader.ReadUInt32();
		}
	}

	public enum REFR_XTRITypes
	{
		Unidentified = 0,
		Static,
		AnimStatic,
		Transparent,
		Clutter,
		Weapon,
		Projectile,
		Spell,
		Biped,
		Trees,
		Props,
		Water,
		Trigger,
		Terrain,
		Trap,
		NonCollidable,
		CloudTrap,
		Ground,
		Portal,
		DebrisSmall,
		DebrisLarge,
		AcusticSpace,
		ActorZone,
		ProjectileZone,
		GasTrap,
		ShellCasing,
		TransparentSmall,
		InvisibleWall,
		TransparentSmallAnim,
		DeadBip,
		CharController,
		AvoidBox,
		CollisionBox,
		CameraSphere,
		DoorDetection,
		CameraPick,
		ItemPick,
		LineOfSight,
		PathPick,
		CustomPick1,
		CustomPick2,
		SpellExplosion,
		DroppingPick
	}
}
