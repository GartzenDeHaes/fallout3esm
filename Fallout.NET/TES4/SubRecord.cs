using Fallout.NET.Core;

namespace Fallout.NET.TES4
{
	public class SubRecord
	{
		//public string Name;
		public uint Size;

		public virtual void Deserialize(BetterReader reader, string name)
		{
			//Name = name;
			Size = reader.ReadUInt16();
		}
	}
}
