using TES3Unity.ESM.Records;
using TES3Unity.World;

using UnityEngine;

namespace TES3Unity.Components.Records
{
	public class NPC : RecordComponent
	{
		private void Start()
		{
			var NPC_ = (NPC_Record)record;

			usable = true;
			pickable = false;
			objData.name = NPC_.Name;
			objData.interactionPrefix = "Talk ";

			// Collider
			var collider = gameObject.AddComponent<CapsuleCollider>();
			collider.radius = 0.3f;
			collider.height = 1.8f;
			collider.center = new Vector3(0, 0.5f, 0);

			var rb = gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			rb.isKinematic = true; // FIXME

			// Add items
			//var db = TES3Engine.Inst.DataReader.MorrowindESMFile.ObjectsByIDString;
			var nifManager = NIFManager.Instance;

			foreach (var npcItem in NPC_.Items)
			{
				if (!TES3Engine.Inst.DataReader.TryGetObjectById(npcItem.Name, out var item)) 
				{ 
					continue; 
				}

				//if (!db.ContainsKey(npcItem.Name))
				//{
				//	continue;
				//}

				//var item = db[npcItem.Name];
				//Debug.Log($"Item {npcItem.Name} of type {item.header.name} found.");

				if (item is WEAPRecord)
				{
					var weapon = (WEAPRecord)item;
					var weaponModel = nifManager.InstantiateNIF($"meshes\\{weapon.Model}", false);
					weaponModel.transform.SetParent(transform.FindChildRecursiveExact("Right Hand"));
					weaponModel.transform.localPosition = Vector3.zero;
					weaponModel.transform.localRotation = Quaternion.Euler(180, 0, 180);
				}

				if (item is CLOTRecord)
				{
					var cloth = (CLOTRecord)item;

					foreach (var part in cloth.BodyPartGroup)
					{
						var boneName = Convert.NormalizeFromEnum(part.Index.ToString());

						if (part.Index == BodyPartIndex.Cuirass)
						{
							//boneName = "Chest";
						}
						else if (part.Index == BodyPartIndex.Skirt)
						{
							//boneName = "Groin";
						}

						var bone = transform.FindChildRecursiveExact(boneName);
						if (bone == null)
						{
							//Debug.Log($"Can't find bone {boneName}");
							continue;
						}

						var modelName = part.MalePartName;
						if (NPC_.IsFemale && !string.IsNullOrEmpty(part.FemalePartName))
						{
							modelName = part.FemalePartName;
						}

						if (string.IsNullOrEmpty(modelName))
						{
							continue;
						}

						PatchModelPath(ref modelName, "c");

						// Disable skin renderers to prevent z-fighting.
						var renderers = bone.GetComponentsInChildren<Renderer>();
						foreach (var renderer in renderers)
						{
							renderer.enabled = false;
						}

						NPCFactory.AddBodyPart(nifManager, $"{modelName}", bone, bone.name.ToLower().Contains("left"));
					}
				}

				if (item is ARMORecord)
				{
					var armor = (ARMORecord)item;

					continue;
					foreach (var part in armor.BodyPartGroup)
					{
						var boneName = Convert.NormalizeFromEnum(part.Index.ToString());
						var bone = transform.FindChildRecursiveExact(boneName);
						if (bone == null)
						{
							Debug.Log($"Can't find bone {boneName}");
							continue;
						}

						var modelName = part.MalePartName;

						if (NPC_.IsFemale && !string.IsNullOrEmpty(part.FemalePartName))
						{
							modelName = part.FemalePartName;
						}

						if (string.IsNullOrEmpty(modelName))
						{
							continue;
						}

						PatchModelPath(ref modelName, "a");

						NPCFactory.AddBodyPart(nifManager, $"{modelName}", bone, bone.name.ToLower().Contains("left"));
					}
				}
			}

			var topLevelLod = GetComponent<LODGroup>();
			var lods = GetComponentsInChildren<LODGroup>();

			foreach (var lod in lods)
			{
				if (lod == topLevelLod)
				{
					continue;
				}

				Destroy(lod);
			}
		}

		/// <summary>
		/// Patch model path. Code from philjord0 from the esmj3dtes3 repository
		/// https://github.com/philjord/esmj3dtes3/blob/master/esmj3dtes3/src/esmj3dtes3/j3d/j3drecords/type/J3dNPC_.java
		/// </summary>
		/// <param name="modelName"></param>
		/// <param name="folder"></param>
		public static void PatchModelPath(ref string modelName, string folder)
		{
			if (modelName == "imperial skirt")
			{
				modelName = "a\\a_imperial_skirt.nif";
			}
			else if (modelName == "imperial cuirass" || modelName == "a_imperial_gauntlet")
			{
				modelName = "a\\a_imperial_skins.nif";
			}
			else if (modelName == "imperial boot foot")
			{
				// f for foot, not female
				modelName = "a\\a_imperial_f_shoe.nif";
			}
			else if (modelName == "imperial boot ankle")
			{
				modelName = "a\\a_imperial_a_boot.nif";
			}
			else if (modelName == "imperial helmet")
			{
				modelName = "a\\a_imperial_m_helmet.nif";
			}
			else if (modelName == "imperial ua")
			{
				modelName = "a\\a_imperial_ua_pauldron.nif";
			}
			else if (modelName == "imperial cl pauldron")
			{
				modelName = "a\\a_imperial_cl_pauldron.nif";
			}
			else if (modelName == "a_iron_cuirass")
			{
				modelName = "a\\a_iron_skinned.nif";
			}
			else if (modelName == "a_nordicfur_cuirass" || modelName == "a_nordicfur_gauntlet")
			{
				modelName = "a\\a_nordicfur_skinned.nif";
			}
			else if (modelName == "a_steel_cuirass")
			{
				modelName = "a\\a_steel_skin.nif";
			}
			else if (modelName == "a_m_chitin_chest")
			{
				modelName = "a\\a_m_chitin_skinned.nif";
			}
			else if (modelName == "a_netch_m_chest")
			{
				modelName = "a\\a_netch_m_cuirass2.nif";
			}
			else if (modelName == "a_netch_m_gauntlet")
			{
				modelName = "a\\a_netch_m_skinned.nif";
			}
			else if (modelName == "a_orcish_boot_f")
			{
				modelName = "a\\a_orcish_boots_f.nif";
			}
			else if (modelName == "a_orcish_boot_a")
			{
				modelName = "a\\a_orcish_boots_a.nif";
			}
			else if (modelName == "a_orcish_cuirass")
			{
				modelName = "a\\a_orcish_cuirass_c.nif";
			}
			else if (modelName == "c_slave_bracer")
			{
				modelName = "c\\c_slave_bracer.nif";
			}
			else if (modelName == "c_m_bracer_w_leather01")
			{
				modelName = "c\\c_m_bracer_w_leather01.nif";
			}
			else if (modelName == "c_m_bracer_w_clothwrap02")
			{
				modelName = "c\\c_m_bracer_w_clothwrap02.nif";
			}
			else if (modelName == "indoril hlelmet")
			{
				modelName = "a\\a_indoril_m_helmet.nif";
			}
			else if (modelName == "a_indoril_m_gauntlet")
			{
				modelName = "a\\a_indoril_m_skins.nif";
			}
			else if (modelName == "a_indoril_m_chest")
			{
				modelName = "a\\a_indoril_m_skins.nif";
			}
			else
			{
				modelName = $"{folder}\\{modelName}.nif";
			}
		}
	}
}
