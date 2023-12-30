using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class MODSSubRecord : SubRecord
	{
		public uint Count;
		public List<AlternateTexture> AlternateTexture = new List<AlternateTexture>();

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Count = reader.ReadUInt32();

			for (var i = 0; i < Count; i++)
			{
				var alternateTextureLength = reader.ReadUInt32();
				AlternateTexture.Add(new AlternateTexture(alternateTextureLength, Encoding.ASCII.GetString(reader.ReadBytes(Convert.ToInt32(alternateTextureLength))), reader.ReadUInt32(), reader.ReadInt32()));
			}
		}
	}

	public sealed class AlternateTexture
	{
		public uint NameLength;
		public string Name3D;
		public uint FormID;
		public int Index3D;

		public AlternateTexture(uint nameLength, string name3d, uint formId, int index3d)
		{
			NameLength = nameLength;
			Name3D = name3d;
			FormID = formId;
			Index3D = index3d;
		}
	}
}
