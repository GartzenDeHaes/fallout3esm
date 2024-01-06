using System;
using System.IO;
using System.Text;

using Portland;

using UnityEngine;

namespace Fallout.NET
{
	public interface ITesReader : IDisposable
	{
		long Length { get; }
		long Position { get; set; }

		Stream BaseStream { get; }

		int Read(byte[] buffer, int offset, int count);
		bool ReadBool32();
		byte ReadByte();
		byte[] ReadBytes(int count);
		sbyte ReadSByte();
		char ReadChar();
		double ReadDouble();
		short ReadInt16();
		int ReadInt32();
		long ReadInt64();
		string ReadNullTerminatedString(int length);
		string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator);
		string ReadASCIIString(int length);
		AsciiId4 ReadASCII4(int length);
		float ReadSingle();
		string ReadString(int length);
		ushort ReadUInt16();
		uint ReadUInt32();
		ulong ReadUInt64();
		long Seek(long offset, SeekOrigin origin);

		Vector2 ReadVector2();

		Vector3 ReadVector3();

		Vector4 ReadVector4();

		long ReadIntRecord(uint dataSize);

		float[] ReadDoubleArray(int size);

		string ReadStringFromChar(int size);

		string ReadStringFromByte(int size);

		int[] ReadInt32Array(int size);
	}
}