using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.STAT;

using System;
using System.Buffers;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Static
	/// </summary>
	public class STATRecord : Record
	{
		public OBNDSubRecord OBND;

		// Model collection
		public STRSubRecord MODL;
		public BytesSubRecord MODB;
		public BytesSubRecord MODT;
		public MODSSubRecord MODS;
		public MODDSubRecord MODD;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;

				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);


					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "OBND":
							OBND = new OBNDSubRecord();
							OBND.Deserialize(stream, name);
							break;
						case "MODL":
							MODL = new STRSubRecord();
							MODL.Deserialize(stream, name);
							break;
						case "MODB":
							MODB = new BytesSubRecord();
							MODB.Deserialize(stream, name);
							break;
						case "MODT":
							MODT = new BytesSubRecord();
							MODT.Deserialize(stream, name);
							break;
						case "MODS":
							MODS = new MODSSubRecord();
							MODS.Deserialize(stream, name);
							break;
						case "MODD":
							MODD = new MODDSubRecord();
							MODD.Deserialize(stream, name);
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

		public override string ToString()
		{
			return EDID.ToString();
		}
	}
}