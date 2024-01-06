using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Activator
	/// </summary>
	public sealed class ACTIRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord FULL_ActivatorName = new();
		public MODLSubRecord MODL = new();
		public MODLSubRecord MODT;
		public MODLSubRecord MODB;
		public FormID SCRI_ScriptFormId = new();
		public FormID SNAM_SoundLoopingFormId = new();
		public FormID VNAM_SoundActivationFormId = new();
		/// <summary>FormID of a TACT (talking activator) record</summary>
		public FormID RNAM_RadioStationFormId = new();
		public FormID WNAM_WaterTipe = new();

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
							FULL_ActivatorName.Deserialize(stream, name);
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
						case "SCRI":
							SCRI_ScriptFormId.Deserialize(stream, name);
							break;
						case "DEST": // destruction
						case "DSTD":
						case "DMDL":
						case "DMDT":
						case "DSTF":
							stream.SkipBytes(stream.ReadUInt16());
							break;
						case "SNAM":
							SNAM_SoundLoopingFormId.Deserialize(stream, name);
							break;
						case "VNAM":
							VNAM_SoundActivationFormId.Deserialize(stream, name);
							break;
						case "RNAM":
							RNAM_RadioStationFormId.Deserialize(stream, name);
							break;
						case "WNAM":
							WNAM_WaterTipe.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}