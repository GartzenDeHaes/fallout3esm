using System;
using System.Buffers;
using System.Collections.Generic;

using Assets.Fallout.NET.TES4.SubRecords;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public struct SkillBoost
	{
		public ActorValue Skill;
		public sbyte Boost;
	}

	[Flags]
	public enum RaceFlags
	{
		None = 0,
		Playable = 0x1,
		Unknown = 0x2,
		Child = 0x4,
	}

	public class RACERecord : Record
	{
		public STRSubRecord FULL_name = new();
		public STRSubRecord DESC = new();
		public List<XNAMSubRecord> XNAMRelations = new();
		public SkillBoost[] DATA_SkillBoosts = new SkillBoost[7];
		public float DATA_MaleHeight;
		public float DATA_FemaleHeight;
		public float DATA_MaleWeight;
		public float DATA_FemaleWeight;
		public RaceFlags DATA_Flags;
		public FormID ONAM_OlderRaceId = new();
		public FormID YNAM_YoungerRaceId = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//reader.ReadBytes((int)size);
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				//var end = stream.Length;

				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "FULL":
							FULL_name.Deserialize(stream, name);
							break;
						case "DESC":
							DESC.Deserialize(stream, name);
							break;
						case "XNAM":
							var xnam = new XNAMSubRecord();
							xnam.Deserialize(stream, name);
							XNAMRelations.Add(xnam);
							break;
						case "DATA":
							var datalen = stream.ReadUInt16();
							DATA_SkillBoosts[0] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[1] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[2] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[3] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[4] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[5] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							DATA_SkillBoosts[6] = new SkillBoost() { Skill = (ActorValue)(sbyte)stream.ReadByte(), Boost = (sbyte)stream.ReadByte() };
							stream.ReadByte();
							stream.ReadByte();
							DATA_MaleHeight = stream.ReadSingle();
							DATA_FemaleHeight = stream.ReadSingle();
							DATA_MaleWeight = stream.ReadSingle();
							DATA_FemaleWeight = stream.ReadSingle();
							DATA_Flags = (RaceFlags)stream.ReadUInt32();
							break;
						case "ONAM":
							ONAM_OlderRaceId.Deserialize(stream, name);
							break;
						case "YNAM":
							YNAM_YoungerRaceId.Deserialize(stream, name);
							break;
						default:
							//UnityEngine.Debug.Log($"Unknown RACE sub record {name}");
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