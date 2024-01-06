using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fallout.NET.TES4.SubRecords
{
	[Flags]
	public enum MerchantServiceFlags
	{
		None = 0,
		Weapons = 0x0001,
		Armor = 0x0002,
		Alcohol = 0x0004,
		Books = 0x0008,
		Food = 0x0010,
		Chems = 0x0020,
		Stimpacks = 0x0040,
		Lights = 0x0080,
		Unknown1 = 0x0100,
		Apparatus = 0x0200,
		Miscellaneous = 0x0400,
		Spells = 0x0800,
		MagicItems = 0x1000,
		Potions = 0x2000,
		Training = 0x4000,
		Unknown5 = 0x8000,
		Recharge = 0x10000,
		Repair = 0x20000,
	}
}
