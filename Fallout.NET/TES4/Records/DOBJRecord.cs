using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Portland.Mathmatics;
using static System.Collections.Specialized.BitVector32;
using static UnityEditorInternal.ReorderableList;

using UnityEditor.Playables;

using UnityEngine;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// This is a lookup array of variable names to the record they reference. 
	/// </summary>
	public sealed class DOBJRecord : Record
	{
		public STRSubRecord EDID;
		public FormID Stimpack = new();
		public FormID SuperStimpack = new();
		public FormID RadX = new();
		public FormID RadAway = new();
		public FormID Morphine = new();
		public FormID PerkParalysis = new();
		public FormID PlayerFaction = new();
		public FormID MysteriousStrangerNPC = new();
		public FormID MysteriousStrangerFaction = new();
		public FormID DefaultMusic = new();
		public FormID BattleMusic = new();
		public FormID DeathMusic = new();
		public FormID SuccessMusic = new();
		public FormID LevelUpMusic = new();
		public FormID PlayerVoice_Male = new();
		public FormID PlayerVoice_MaleChild = new();
		public FormID PlayerVoice_Female = new();
		public FormID PlayerVoice_FemaleChild = new();
		public FormID EatPackageDefaultFood = new();
		public FormID EveryActorAbility = new();
		public FormID DrugWearsOffImageSpace = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;

				while (stream.Position < end)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = new STRSubRecord();
							EDID.Deserialize(stream, name);
							break;
						case "DATA":
							var dataLen = stream.ReadUInt16();

							//string data = stream.ReadString(dataLen);
							Stimpack.Value = stream.ReadInt32();
							SuperStimpack.Value = stream.ReadInt32();
							RadX.Value = stream.ReadInt32();
							RadAway.Value = stream.ReadInt32();
							Morphine.Value = stream.ReadInt32();
							PerkParalysis.Value = stream.ReadInt32();
							PlayerFaction.Value = stream.ReadInt32();
							MysteriousStrangerNPC.Value = stream.ReadInt32();
							MysteriousStrangerFaction.Value = stream.ReadInt32();
							DefaultMusic.Value = stream.ReadInt32();
							BattleMusic.Value = stream.ReadInt32();
							DeathMusic.Value = stream.ReadInt32();
							SuccessMusic.Value = stream.ReadInt32();
							LevelUpMusic.Value = stream.ReadInt32();
							PlayerVoice_Male.Value = stream.ReadInt32();
							PlayerVoice_MaleChild.Value = stream.ReadInt32();
							PlayerVoice_Female.Value = stream.ReadInt32();
							PlayerVoice_FemaleChild.Value = stream.ReadInt32();
							EatPackageDefaultFood.Value = stream.ReadInt32();
							EveryActorAbility.Value = stream.ReadInt32();
							DrugWearsOffImageSpace.Value = stream.ReadInt32();
							break;
						default:
							UnityEngine.Debug.Log($"Unknown DOBJ subrecord {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}



