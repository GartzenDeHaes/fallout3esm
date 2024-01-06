using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET;

using Portland;

namespace TES4Unity
{
	public struct RecordHeader
	{
		public AsciiId4 RecType; // 4 bytes
		public uint DataSize;
		//public uint unknown0;
		public uint Flags;
		public uint FormId;
		//public uint revision;
		//public uint version;

		/// <summary>
		/// TES3 records are not compressed
		/// </summary>
		public bool Compressed
		{
			get { return (Flags & 0x00040000) != 0; }
		}

		public bool Deleted
		{
			get { return (Flags & 0x20) != 0; }
		}

		public bool Ignored
		{
			get { return (Flags & 0x1000) != 0; }
		}

		public void Deserialize(ITesReader reader, in AsciiId4 recordType)
		{
			RecType = recordType;
			DataSize = reader.ReadUInt32();

			// unknown0
			_ = reader.ReadUInt32();
			Flags = reader.ReadUInt32();
		}

		//private static byte[] Decompress(byte[] data)
		//{
		//	using (var outMemoryStream = new MemoryStream())
		//	using (var outZStream = new ComponentAce.Compression.Libs.zlib.ZOutputStream(outMemoryStream))
		//	using (var inMemoryStream = new MemoryStream(data))
		//	{
		//		CopyStream(inMemoryStream, outZStream);
		//		outZStream.finish();
		//		return outMemoryStream.ToArray();
		//	}
		//}

		//public static void CopyStream(Stream input, Stream output)
		//{
		//	byte[] buffer = new byte[2000];
		//	int len;
		//	while ((len = input.Read(buffer, 0, 2000)) > 0)
		//	{
		//		output.Write(buffer, 0, len);
		//	}
		//	output.Flush();
		//}
	}
}
