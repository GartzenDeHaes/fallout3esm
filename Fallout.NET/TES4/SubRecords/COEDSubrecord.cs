using Fallout.NET.Core;

using System;

using UnityEngine;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class COEDSubRecord : SubRecord
	{
		public uint OwnerFormId;
		public uint GlobOrReqRank;
		public float ItemCondition;

		public override void Deserialize(BetterReader reader, String name)
		{
			base.Deserialize(reader, name);

			Debug.Assert(Size == 4 + 4 + 4);

			OwnerFormId = reader.ReadUInt32();
			GlobOrReqRank = reader.ReadUInt32();
			ItemCondition = reader.ReadSingle();
		}
	}
}
