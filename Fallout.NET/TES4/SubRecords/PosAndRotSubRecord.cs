using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class PosAndRotSubRecord : SubRecord
	{
		public float X;
		public float Y;
		public float Z;
		public float rX;
		public float rY;
		public float rZ;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			X = reader.ReadSingle();
			Y = reader.ReadSingle();
			Z = reader.ReadSingle();
			rX = reader.ReadSingle();
			rY = reader.ReadSingle();
			rZ = reader.ReadSingle();
		}
	}
}
