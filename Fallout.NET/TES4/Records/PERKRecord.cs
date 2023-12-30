using System.Collections.Generic;
using System.Xml.Linq;

using Fallout.NET.Core;

using UnityEngine;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Perk
	/// </summary>
	public sealed class PERKRecord : Record
	{
		public STRSubRecord EDID;
		public STRSubRecord FULL;
		public STRSubRecord DESC;
		public STRSubRecord ICON_LargeIcon;
		public STRSubRecord MICO_SmallIcon;
		//public CTDASubrecord; // https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/Subrecords/CTDA.md
		public List<DATA_SubRec> DATA = new List<DATA_SubRec>();
		public List<EffectSubRecord> Effect = new List<EffectSubRecord>();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}

		public struct DATA_SubRec
		{
			public Trait Trait;
			public byte MinLevel;
			public byte Ranks;
			public YesNo Playable;
			public YesNo Hidden;
		}

		public enum Trait : byte
		{ }

		public enum YesNo : byte
		{ }

		// https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Records/PERK.md
		public struct EffectSubRecord
		{

		}
	}
}
