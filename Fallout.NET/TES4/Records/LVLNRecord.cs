using Fallout.NET.Core;

using Portland.AI;
using Portland.Text.Generate;
using UnityEditor.Experimental.GraphView;

namespace Fallout.NET.TES4.Records
{

	/// <summary>
	/// Leveled NPC
	/// 
	/// Anything leveled will spawn enemies or loot according to the player's 
	/// level. This means you can enter a dungeon at level 5 to see lower level 
	/// enemies and receive lower level loot. Entering the same dungeon at level 
	/// 50 will spawn different, harder enemies and better loot because of the 
	/// player level.
	///
	/// Loot on NPCs comes from 3 sources together:
	///
	/// 1) Inventory of NPC(a list of items and/or leveled lists)
	///
	/// 2) Outfit of NPC(a list of items and/or leveled lists)
	///
	/// 3) Death Item of NPC(a single leveled list)
	///
	/// Am I also right in assuming that the more lower level(albeit duplicate 
	/// NPC references) you have in a leveled list, the more likely that specific 
	/// NPC will spawn? So in a sense, one could still allow for the higher 
	/// level(60, 70, 80) spawns, but dot the list with many level 1 - 20 mobs?
	///
	/// </summary>
	public sealed class LVLNRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}
}