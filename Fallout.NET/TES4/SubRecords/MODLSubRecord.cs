using Fallout.NET.Core;

using System;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class MODLSubRecord : SubRecord
	{
		public string Model;

		public override void Deserialize(BetterReader reader, String name)
		{
			base.Deserialize(reader, name);
			Model = reader.ReadNullTerminatedString((int)Size);
		}
	}
}
