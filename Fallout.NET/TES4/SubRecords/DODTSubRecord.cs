using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	[Flags]
	public enum DecalFlags
	{
		None = 0,
		Parallax = 0x01,
		AlphaBlending = 0x02,
		AlphaTesting = 0x04,
	}

	/// <summary>
	/// Decal Data
	/// </summary>
	public sealed class DODTSubRecord : SubRecord
	{
		public float MinWidth;
		public float MaxWidth;
		public float MinHeight;
		public float MaxHeight;
		public float Depth;
		public float Shininess;
		public float ParallaxScale;
		public byte ParallaxPasses;
		public DecalFlags Flags;
		public uint ColorRGBA;

		public override void Deserialize(BetterReader reader, String name)
		{
			base.Deserialize(reader, name);

			MinWidth = reader.ReadSingle();
			MaxWidth = reader.ReadSingle();
			MinHeight = reader.ReadSingle();
			MaxHeight = reader.ReadSingle();
			Depth = reader.ReadSingle();
			Shininess = reader.ReadSingle();
			ParallaxScale = reader.ReadSingle();
			ParallaxPasses = reader.ReadByte();
			Flags = (DecalFlags)reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			ColorRGBA = reader.ReadUInt32();
		}
	}
}
