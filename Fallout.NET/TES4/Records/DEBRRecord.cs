using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public class DEBRRecord : Record
	{
		public List<DebrisModel> Models = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;
			DebrisModel current = null;

			using (var stream = new BetterMemoryReader(bytes))
			{
				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "DATA":
							current = new DebrisModel();
							Models.Add(current);
							var datasize = stream.ReadUInt16();
							current.DATA_Percentage = stream.ReadByte();
							current.DATA_ModelFilename = stream.ReadNullTerminatedString(datasize - 2);
							current.DATA_Flags = (DebrisFlags)stream.ReadByte();
							break;
						case "MODT":
							current.MODT_TextureFileHashes.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.SkipBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}

	[Flags]
	public enum DebrisFlags
	{
		None = 0,
		HasCollisonData = 0x01,
	}

	public class DebrisModel
	{
		public byte DATA_Percentage;
		public string DATA_ModelFilename;
		public DebrisFlags DATA_Flags;
		public BytesSubRecord MODT_TextureFileHashes = new();
	}
}
