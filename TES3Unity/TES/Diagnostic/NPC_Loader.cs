using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using TES3Unity.World;

using UnityEngine;

namespace TES3Unity.Diagnostic
{
	public class NPC_Loader : MonoBehaviour
	{
		private NIFManager m_NifManager = null;

		[SerializeField]
		private RaceType m_Race = RaceType.Breton;
		[SerializeField]
		private bool m_Female = false;

		private void Awake()
		{
			var tes = GetComponent<TES3Loader>();
			tes.Initialized += Tes_Initialized;
		}

		private void Tes_Initialized(TES3Loader tes)
		{
			m_NifManager = tes.NifManager;

			if (TES3Engine.Inst.DataReader is TES3DataReader reader)
			{
				var npcs = reader.MorrowindESMFile.GetRecords<NPC_Record>();
				NPC_Record npcRecord = null;

				foreach (var npc in npcs)
				{
					if (npc.Race == m_Race.ToString().Replace("_", " "))
					{
						var female = Utils.ContainsBitFlags((uint)npc.Flags, (uint)NPCFlags.Female);

						if (female && m_Female || !female && !m_Female)
						{
							npcRecord = npc;
							break;
						}
					}
				}

				// Instanciate NPC
				var npcObj = NPCFactory.InstanciateNPC(m_NifManager, npcRecord);
				var component = npcObj.AddComponent<NPC>();
				component.record = npcRecord;
			}
			else
			{
				Debug.Log($"TES4 not supported");
			}
		}
	}
}