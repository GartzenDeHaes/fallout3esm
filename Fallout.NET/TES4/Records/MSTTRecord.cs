using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Movable Static
	/// </summary>
	public sealed class MSTTRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord FULL = new();
		public MODLSubRecord MODL = new();
		public MODLSubRecord MODT;
		public MODLSubRecord MODB;
		public FormID SNAM_Sound = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

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
						case "OBND":
							OBND.Deserialize(stream, name);
							break;
						case "FULL":
							FULL.Deserialize(stream, name);
							break;
						case "MODL":
							MODL.Deserialize(stream, name);
							break;
						case "MODT":
							MODT = new();
							MODT.Deserialize(stream, name);
							break;
						case "MODB":
							MODB = new();
							MODB.Deserialize(stream, name);
							break;
						case "SNAM":
							SNAM_Sound.Deserialize(stream, name);
							break;
						case "DATA":
						case "DEST":
						case "DSTD":
						case "DSTF":
						case "DMDL":
						case "DMDT":
							stream.SkipBytes(stream.ReadUInt16());
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
}