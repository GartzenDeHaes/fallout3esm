using Fallout.NET.Core;

using UnityEngine;

namespace Fallout.NET.TES4.Records
{
	public sealed class BytesSubRecord : SubRecord
	{
		public byte[] Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadBytes((int)Size);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public sealed class ByteSubRecord : SubRecord
	{
		public byte Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadByte();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public sealed class STRSubRecord : SubRecord
	{
		public string Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadNullTerminatedString((int)Size);
		}

		public override string ToString()
		{
			return Value;
		}
	}

	//public class UInt16SubRecord : SubRecord
	//{
	//	public ushort Value { get; protected set; }

	//	public override void Deserialize(BetterReader reader, string name)
	//	{
	//		base.Deserialize(reader, name);
	//		Value = reader.ReadUInt16();
	//	}

	//	public override string ToString()
	//	{
	//		return Value.ToString();
	//	}
	//}

	public sealed class UInt32SubRecord : SubRecord
	{
		public uint Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadUInt32();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}


	public sealed class UInt64SubRecord : SubRecord
	{
		public ulong Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadUInt64();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public sealed class FloatSubRecord : SubRecord
	{
		public float Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadSingle();
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public sealed class Vector2iSubRecord : SubRecord
	{
		public Vector2Int Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
		}

		public override string ToString()
		{
			return $"X:{Value.x.ToString()} Y:{Value.y.ToString()})";
		}
	}

	public sealed class Vector2fSubRecord : SubRecord
	{
		public float X;
		public float Y;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			X = reader.ReadSingle();
			Y = reader.ReadSingle();
		}

		public override string ToString()
		{
			return $"X:{X.ToString()} Y:{Y.ToString()})";
		}
	}
}
