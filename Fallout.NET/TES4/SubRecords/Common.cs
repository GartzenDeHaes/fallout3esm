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


		public static byte[] Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			return reader.ReadBytes(datasize);
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

		public static byte Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 1);
			return reader.ReadByte();
		}
	}

	public sealed class SByteSubRecord
	{
		public static sbyte Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 1);
			return (sbyte)reader.ReadByte();
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

		public static string Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			return reader.ReadNullTerminatedString(datasize);
		}
	}

	public class UInt16SubRecord : SubRecord
	{
		public ushort Value { get; protected set; }

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Value = reader.ReadUInt16();
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public static ushort Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 2);
			return reader.ReadUInt16();
		}
	}

	public class Int16SubRecord : SubRecord
	{
		public static short Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 2);
			return reader.ReadInt16();
		}
	}

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

		public static uint Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 4);
			return reader.ReadUInt32();
		}
	}

	public sealed class Int32SubRecord : SubRecord
	{
		public static int Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 4);
			return reader.ReadInt32();
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

		public static ulong Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 8);
			return reader.ReadUInt64();
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

		public static float Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 4);
			return reader.ReadSingle();
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

		public static Vector2Int Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 8);
			return new Vector2Int(reader.ReadInt32(), reader.ReadInt32());
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

		public static Vector2 Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 8);
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}
	}

	public static class ColorSubRecord
	{
		public static Color Read(BetterReader reader, string name)
		{
			var datasize = reader.ReadUInt16();
			Debug.Assert(datasize == 4);
			return new Color(reader.ReadByte()/255f, reader.ReadByte()/255f, reader.ReadByte()/255f, reader.ReadByte()/255f);
		}
	}
}
