using System.Xml.Linq;
using System;

using Fallout.NET.Core;
using System.Buffers;
using Fallout.NET.TES4.SubRecords;
using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	public enum MaterialType
	{
		Stone = 0,
		Cloth = 1,
		Dirt = 2,
		Glass = 3,
		Grass = 4,
		Metal = 5,
		Organic = 6,
		Skin = 7,
		Water = 8,
		Wood = 9,
		HeavyStone = 10,
		HeavyMetal = 11,
		HeavyWood = 12,
		Chain = 13,
		Snow = 14,
		Elevator = 15,
		HollowMetal = 16,
		SheetMetal = 17,
		Sand = 18,
		BrokenConcrete = 19,
		VehicleBody = 20,
		VehiclePartSolid = 21,
		VehiclePartHollow = 22,
		Barrel = 23,
		Bottle = 24,
		SodaCan = 25,
		Pistol = 26,
		Rifle = 27,
		ShoppingCart = 28,
		Lunchbox = 29,
		BabyRattle = 30,
	}

	/// <summary>
	/// Landscape Texture
	/// </summary>
	public class LTEXRecord : Record
	{
		public STRSubRecord ICON = new();
		public STRSubRecord MICO = new();
		public FormID TNAM_TextureSetId = new();
		public MaterialType HNAM_MaterialType;
		public byte HNAM_Friction;
		public byte HNAM_Restitution;
		public byte SNAM_TextureSpecularExponent;
		public List<FormID> GNAM_Grass = new();

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
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MICO":
							MICO.Deserialize(stream, name);
							break;
						case "TNAM":
							TNAM_TextureSetId.Deserialize(stream, name);
							break;
						case "HNAM":
							var datasize = stream.ReadUInt16();
							HNAM_MaterialType = (MaterialType)stream.ReadByte();
							HNAM_Friction = stream.ReadByte();
							HNAM_Restitution = stream.ReadByte();
							break;
						case "SNAM":
							datasize = stream.ReadUInt16();
							SNAM_TextureSpecularExponent = stream.ReadByte();
							break;
						case "GNAM":
							GNAM_Grass.Add(new FormID());
							GNAM_Grass[GNAM_Grass.Count - 1].Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown LTEX sub record {name}");
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