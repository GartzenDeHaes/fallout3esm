using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Talking Activator
	/// </summary>
	public sealed class TACTRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord FULL = new();
		public MODLSubRecord MODL = new();
		public MODLSubRecord MODT;
		public FormID SCRI = new();
		public FormID SNAM_Sound = new();
		public FormID VNAM_Voice = new();

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
						case "SCRI":
							SCRI.Deserialize(stream, name);
							break;
						case "SNAM":
							SNAM_Sound.Deserialize(stream, name);
							break;
						case "VNAM":
							VNAM_Voice.Deserialize(stream, name);
							break;
						case "DEST": // destruction
						case "DSTD":
						case "DSTF":
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