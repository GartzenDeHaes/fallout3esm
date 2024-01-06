using Fallout.NET.Core;

using System;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class FormID : SubRecord
	{
		public int Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			UnityEngine.Debug.Assert(Size == 4);
			Value = reader.ReadInt32();
		}

		public override string ToString()
		{
			return Value.ToString("X");
		}

		public static uint Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			UnityEngine.Debug.Assert(datasize == 4);
			return reader.ReadUInt32();
		}
	}
}
