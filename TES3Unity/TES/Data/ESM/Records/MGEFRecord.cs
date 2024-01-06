using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum MagicSpellSchool
	{
		Alteration,
		Conjuration,
		Destruction,
		Illusion,
		Mysticism,
		Restoration
	}

	public enum MagicEffectFlags
	{
		SpellMaking = 0x0200,
		Enchanting = 0x0400,
		Negative = 0x0800
	}

	public class MGEFRecord : Record
	{
		public string Id { get; private set; }
		public MagicEffectData Data { get; private set; }
		public string Icon { get; private set; }
		public string ParticleTexture { get; private set; }
		public string CastingVisual { get; private set; }
		public string BoltVisual { get; private set; }
		public string HitVisual { get; private set; }
		public string AreaVisual { get; private set; }
		public string Description { get; private set; }
		public string CastSound { get; private set; }
		public string BoltSound { get; private set; }
		public string HitSound { get; private set; }
		public string AreaSound { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "INDX")
			{
				Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "MEDT")
			{
				Data = new MagicEffectData
				{
					SpellSchool = (MagicSpellSchool)reader.ReadInt32(),
					BaseCost = reader.ReadSingle(),
					Flags = reader.ReadInt32(),
					Red = reader.ReadInt32(),
					Blue = reader.ReadInt32(),
					Green = reader.ReadInt32(),
					SpeedX = reader.ReadSingle(),
					SizeX = reader.ReadSingle(),
					SizeCap = reader.ReadSingle(),
				};
			}
			else if (subRecordName == "ITEX")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "PTEX")
			{
				ParticleTexture = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "CVFX")
			{
				CastingVisual = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "BVFX")
			{
				BoltVisual = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "HVFX")
			{
				HitVisual = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "AVFX")
			{
				AreaVisual = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "DESC")
			{
				Description = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "CSND")
			{
				CastSound = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "BSND")
			{
				BoltSound = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "HSND")
			{
				HitSound = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "ASND")
			{
				AreaSound = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
