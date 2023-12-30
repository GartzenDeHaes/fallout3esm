using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_TNAMSubRecord : SubRecord
	{
		public TNAMTypes Value;
		public byte Unused;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (TNAMTypes)reader.ReadByte();
			Unused = reader.ReadByte();
		}
	}

	public enum TNAMTypes
	{
		None = 0,
		City,
		Settlement,
		Encampment,
		NaturalLandmark,
		Cave,
		Factory,
		Monument,
		Military,
		Office,
		TownRuins,
		UrbanRuins,
		SewerRuins,
		Metro,
		Vault
	}
}
