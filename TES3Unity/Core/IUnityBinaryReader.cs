using System.IO;

using UnityEngine;

public interface IUnityBinaryReader
{
	Stream BaseStream { get; }

	void Close();
	int Read(byte[] buffer, int index, int count);
	string ReadASCIIString(int length);
	byte ReadByte();
	byte[] ReadBytes(int count);
	float[] ReadDoubleArray(int size);
	float[] ReadFloatArray(int size);
	int[] ReadInt32Array(int size);
	long ReadIntRecord(uint dataSize);
	bool ReadBool32();
	Matrix4x4 ReadColumnMajorMatrix3x3();
	Matrix4x4 ReadColumnMajorMatrix4x4();
	double ReadDouble();
	short ReadInt16();
	int ReadInt32();
	long ReadInt64();
	string ReadLength32PrefixedASCIIString();
	byte[] ReadLength32PrefixedBytes();
	Quaternion ReadQuaternionWFirst();
	Quaternion ReadQuaternionWLast();
	Matrix4x4 ReadRowMajorMatrix3x3();
	Matrix4x4 ReadRowMajorMatrix4x4();
	float ReadSingle();
	ushort ReadUInt16();
	uint ReadUInt32();
	ulong ReadUInt64();
	Vector2 ReadVector2();
	Vector3 ReadVector3();
	Vector4 ReadVector4();
	string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator);
	byte[] ReadRestOfBytes();
	void ReadRestOfBytes(byte[] buffer, int startIndex);
	sbyte ReadSByte();
	string ReadStringFromByte(int size);
	string ReadStringFromChar(int size);
	string ReadUnicodeString(int length);
	string ReadUTF8String(int length);
}