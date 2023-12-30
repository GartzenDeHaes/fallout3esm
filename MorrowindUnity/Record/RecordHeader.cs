using System.IO;

public sealed class RecordHeader
{
	public int DataOffset;
	public int DataSize;
	public bool IsDeleted;
	public RecordFlags Flags;
	public RecordType Type;
	public FormId FormId;

	public bool Compressed
	{
		get { return ((uint)Flags & 0x00040000) != 0; }
	}

	public bool Deleted
	{
		get { return IsDeleted || ((uint)Flags & 0x20) != 0; }
	}

	public bool Ignored
	{
		get { return ((uint)Flags & 0x1000) != 0; }
	}

	public RecordHeader(BinaryReader reader, in GameId gameId)
	{
		Type = (RecordType)reader.ReadInt32();
		DataSize = reader.ReadInt32();
		
		if (gameId == GameId.Morrowind)
		{
			IsDeleted = reader.ReadInt32() != 0;
		}

		Flags = (RecordFlags)reader.ReadInt32();

		if (gameId == GameId.Fallout3)
		{
			FormId = new FormId(reader.ReadUInt32());

			// revision
			_ = reader.ReadUInt32();
			// version
			_ = reader.ReadUInt16();
			// unknown0
			_ = reader.ReadUInt16();
		}

		DataOffset = (int)reader.BaseStream.Position;
	}

	public int DataEndPos => DataOffset + DataSize - 4;

	public override string ToString()
	{
		return $"Type: {Type}, Size: {DataSize}, Deleted: {IsDeleted}, Flags: {Flags}, RecordType: {Flags}";
	}
}