using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public enum PlacedWaterFlags : uint
	{
		Reflects = 0x00000001,
		ReflectsActors = 0x00000002,
		ReflectsLand = 0x00000004,
		ReflectsLODLand = 0x00000008,
		ReflectsLODBuildings = 0x00000010,
		ReflectsTrees = 0x00000020,
		ReflectsSky = 0x00000040,
		ReflectsDynamicObjects = 0x00000080,
		ReflectsDeadBodies = 0x00000100,
		Refracts = 0x00000200,
		RefractsActors = 0x00000400,
		RefractsLand = 0x00000800,
		Unknown1 = 0x00001000,
		Unknown2 = 0x00002000,
		Unknown3 = 0x00004000,
		Unknown4 = 0x00008000,
		RefractsDynamicObjects = 0x00010000,
		RefractsDeadBodies = 0x00020000,
		SilhouetteReflections = 0x00040000,
		Unknown5 = 0x00080000,
		Unknown6 = 0x00100000,
		Unknown7 = 0x00200000,
		Unknown8 = 0x00400000,
		Unknown9 = 0x00800000,
		Unknown10 = 0x01000000,
		Unknown11 = 0x02000000,
		Unknown12 = 0x03000000,
		Unknonw13 = 0x08000000,
		Depth = 0x10000000,
		ObjectTextureCoordinates = 0x20000000,
		Unknown14 = 0x40000000,
		NoUnderwaterFog = 0x80000000,
	}

	/// <summary>
	/// Placed Water
	/// </summary>
	public sealed class PWATRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public MODLSubRecord MODL = new();
		public PlacedWaterFlags DNAM_Flags;
		public uint DNAM_WaterFormId;

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
						case "MODL":
							MODL.Deserialize(stream, name);
							break;
						case "DNAM":
							var datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 8, datasize);
							DNAM_Flags = (PlacedWaterFlags)stream.ReadUInt32();
							DNAM_WaterFormId = stream.ReadUInt32();
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