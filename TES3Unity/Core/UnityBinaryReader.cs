using System;
using System.IO;
using System.Text;

using Fallout.NET;

using Portland;

using UnityEngine;

/// <summary>
/// An improved BinaryReader for Unity.
/// </summary>
public class UnityBinaryReader : IDisposable, IUnityBinaryReader, ITesReader
{
	private BinaryReader _reader;
	// A buffer for read bytes the size of a decimal variable. Created to minimize allocations. 
	private byte[] _readBuffer = new byte[16];
	//private byte[] _emptyArray = new byte[0];

	public Stream BaseStream => _reader.BaseStream;

	// For BetterBinaryReader
	public long Position
	{
		get { return _reader.BaseStream.Position; }
		set { _reader.BaseStream.Position = value; }
	}

	// Might need to use this for MemoryStream? (See BetterMemoryReader)
	//public long Position
	//{
	//	get { return _reader.BaseStream.Position; }
	//	set { _reader.BaseStream.Position = value; }
	//}

	public long Length
	{
		get { return _reader.BaseStream.Length; }
	}

	public UnityBinaryReader(Stream input)
	{
		_reader = new BinaryReader(input, Encoding.UTF8);
	}

	void IDisposable.Dispose()
	{
		Close();
	}

	~UnityBinaryReader()
	{
		Close();
	}

	public void Close()
	{
		if (_reader != null)
		{
			_reader.Close();
			_reader = null;
		}
	}

	public long Seek(long offset, SeekOrigin origin)
	{
		return _reader.BaseStream.Seek(offset, origin);
	}

	public byte ReadByte()
	{
		return _reader.ReadByte();
	}

	public char ReadChar()
	{
		return _reader.ReadChar();
	}

	public sbyte ReadSByte()
	{
		return _reader.ReadSByte();
	}

	public int Read(byte[] buffer, int index, int count)
	{
		return _reader.Read(buffer, index, count);
	}

	public byte[] ReadBytes(int count)
	{
		return _reader.ReadBytes(count);
	}

	public byte[] ReadRestOfBytes()
	{
		var remainingByteCount = _reader.BaseStream.Length - _reader.BaseStream.Position;

		Debug.Assert(remainingByteCount <= int.MaxValue);

		return _reader.ReadBytes((int)remainingByteCount);
	}

	public void ReadRestOfBytes(byte[] buffer, int startIndex)
	{
		var remainingByteCount = _reader.BaseStream.Length - _reader.BaseStream.Position;

		Debug.Assert((startIndex >= 0) && (remainingByteCount <= int.MaxValue) && ((startIndex + remainingByteCount) <= buffer.Length));

		_reader.Read(buffer, startIndex, (int)remainingByteCount);
	}

	public AsciiId4 ReadASCII4(int length)
	{
		AsciiId4 str = AsciiId4.Empty;
		str[0] = (char)ReadByte();
		str[1] = (char)ReadByte();
		str[2] = (char)ReadByte();
		str[3] = (char)ReadByte();
		return str;
	}

	public string ReadASCIIString(int length)
	{
		Debug.Assert(length >= 0);

		return Encoding.ASCII.GetString(_reader.ReadBytes(length));
	}

	public string ReadUnicodeString(int length)
	{
		return Encoding.Unicode.GetString(_reader.ReadBytes(length));
	}

	public string ReadUTF8String(int length)
	{
		return Encoding.UTF8.GetString(_reader.ReadBytes(length));
	}

	public string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator)
	{
		Debug.Assert(lengthIncludingPossibleNullTerminator > 0);

		var bytes = _reader.ReadBytes(lengthIncludingPossibleNullTerminator);
		var count = bytes.Length;

		for (var i = 0; i < bytes.Length; i++)
		{
			if (bytes[i] == 0)
			{
				count = i;
				break;
			}
		}

		// Ignore the null terminator.
		return Encoding.Default.GetString(bytes, 0, count);
	}

	// From BetterBinaryReader. Replace with ReadPossiblyNullTerminatedASCIIString?
	public string ReadNullTerminatedString(int length)
	{
		var bytes = _reader.ReadBytes(length);
		var size = bytes.Length;
		var charCount = (bytes[size - 1] != 0) ? size : size - 1;

		return Encoding.UTF8.GetString(bytes, 0, charCount);
	}

	// From BetterBinaryReader
	public string ReadString(int length)
	{
		return Encoding.ASCII.GetString(_reader.ReadBytes(length));
	}

	#region Little Endian

	public bool ReadBool32()
	{
		return ReadUInt32() != 0;
	}

	public ushort ReadUInt16()
	{
		_reader.Read(_readBuffer, 0, 2);

		return (ushort)((_readBuffer[1] << 8) | _readBuffer[0]);
	}
	public uint ReadUInt32()
	{
		_reader.Read(_readBuffer, 0, 4);

		return ((uint)_readBuffer[3] << 24) | ((uint)_readBuffer[2] << 16) | ((uint)_readBuffer[1] << 8) | _readBuffer[0];
	}
	public ulong ReadUInt64()
	{
		_reader.Read(_readBuffer, 0, 8);

		return ((ulong)_readBuffer[7] << 56) | ((ulong)_readBuffer[6] << 48) | ((ulong)_readBuffer[5] << 40) | ((ulong)_readBuffer[4] << 32) | ((ulong)_readBuffer[3] << 24) | ((ulong)_readBuffer[2] << 16) | ((ulong)_readBuffer[1] << 8) | _readBuffer[0];
	}

	public short ReadInt16()
	{
		_reader.Read(_readBuffer, 0, 2);

		return BitConverter.ToInt16(_readBuffer, 0);
	}

	public int ReadInt32()
	{
		_reader.Read(_readBuffer, 0, 4);

		return BitConverter.ToInt32(_readBuffer, 0);
	}

	public long ReadInt64()
	{
		_reader.Read(_readBuffer, 0, 8);

		return BitConverter.ToInt32(_readBuffer, 0);
	}

	public float ReadSingle()
	{
		_reader.Read(_readBuffer, 0, 4);

		return BitConverter.ToSingle(_readBuffer, 0);
	}

	public double ReadDouble()
	{
		_reader.Read(_readBuffer, 0, 8);

		return BitConverter.ToDouble(_readBuffer, 0);
	}

	public byte[] ReadLength32PrefixedBytes()
	{
		var length = ReadInt32();

		if (length <= 0)
		{
			return Array.Empty<byte>();
		}

		var count = length;
		return _reader.ReadBytes(count);
	}

	public string ReadLength32PrefixedASCIIString()
	{
		return Encoding.ASCII.GetString(ReadLength32PrefixedBytes());
	}

	public Vector2 ReadVector2()
	{
		var x = ReadSingle();
		var y = ReadSingle();

		return new Vector2(x, y);
	}

	public Vector3 ReadVector3()
	{
		var x = ReadSingle();
		var y = ReadSingle();
		var z = ReadSingle();

		return new Vector3(x, y, z);
	}

	public Vector4 ReadVector4()
	{
		var x = ReadSingle();
		var y = ReadSingle();
		var z = ReadSingle();
		var w = ReadSingle();

		return new Vector4(x, y, z, w);
	}

	/// <summary>
	/// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
	/// </summary>
	public Matrix4x4 ReadColumnMajorMatrix3x3()
	{
		var matrix = new Matrix4x4();

		for (int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for (int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
				if ((rowIndex <= 2) && (columnIndex <= 2))
				{
					matrix[rowIndex, columnIndex] = ReadSingle();
				}
				else
				{
					matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
				}
			}
		}

		return matrix;
	}

	/// <summary>
	/// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
	/// </summary>
	public Matrix4x4 ReadRowMajorMatrix3x3()
	{
		var matrix = new Matrix4x4();

		for (int rowIndex = 0; rowIndex < 4; rowIndex++)
		{
			for (int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				// If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
				if ((rowIndex <= 2) && (columnIndex <= 2))
				{
					matrix[rowIndex, columnIndex] = ReadSingle();
				}
				else
				{
					matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
				}
			}
		}

		return matrix;
	}

	public Matrix4x4 ReadColumnMajorMatrix4x4()
	{
		var matrix = new Matrix4x4();

		for (int columnIndex = 0; columnIndex < 4; columnIndex++)
		{
			for (int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				matrix[rowIndex, columnIndex] = ReadSingle();
			}
		}

		return matrix;
	}

	public Matrix4x4 ReadRowMajorMatrix4x4()
	{
		var matrix = new Matrix4x4();

		for (int rowIndex = 0; rowIndex < 4; rowIndex++)
		{
			for (int columnIndex = 0; columnIndex < 4; columnIndex++)
			{
				matrix[rowIndex, columnIndex] = ReadSingle();
			}
		}

		return matrix;
	}

	public Quaternion ReadQuaternionWFirst()
	{
		float w = ReadSingle();
		float x = ReadSingle();
		float y = ReadSingle();
		float z = ReadSingle();

		return new Quaternion(x, y, z, w);
	}

	public Quaternion ReadQuaternionWLast()
	{
		float x = ReadSingle();
		float y = ReadSingle();
		float z = ReadSingle();
		float w = ReadSingle();

		return new Quaternion(x, y, z, w);
	}

	#endregion

	#region Helpers

	public long ReadIntRecord(uint dataSize)
	{
		if (dataSize == 1)
		{
			return ReadByte();
		}
		else if (dataSize == 2)
		{
			return ReadInt16();
		}
		else if (dataSize == 4)
		{
			return ReadInt32();
		}
		else if (dataSize == 8)
		{
			return ReadInt64();
		}

		_reader.BaseStream.Position += dataSize;

		return 0;
	}

	public float[] ReadDoubleArray(int size)
	{
		var array = new float[size];
		for (var i = 0; i < 4; i++)
		{
			array[i] = ReadSingle();
		}
		return array;
	}

	public int[] ReadInt32Array(int size)
	{
		var array = new int[size];
		for (var i = 0; i < size; i++)
		{
			array[i] = ReadInt32();
		}
		return array;
	}

	public float[] ReadFloatArray(int size)
	{
		var array = new float[size];
		for (var i = 0; i < size; i++)
		{
			array[i] = ReadSingle();
		}
		return array;
	}

	public string ReadStringFromChar(int size)
	{
		var bytes = ReadBytes(size);
		var array = new char[size];

		for (var i = 0; i < size; i++)
		{
			array[i] = System.Convert.ToChar(bytes[i]);
		}

		return TES3Unity.Convert.CharToString(array);
	}

	public string ReadStringFromByte(int size)
	{
		var bytes = ReadBytes(size);
		var str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
		return TES3Unity.Convert.RemoveNullChar(str);
	}

	#endregion
}