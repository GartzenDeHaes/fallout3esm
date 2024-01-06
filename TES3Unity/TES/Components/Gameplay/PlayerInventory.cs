using System;
using System.Collections.Generic;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        private List<RecordComponent> m_ItemStore = new List<RecordComponent>();
        private Transform m_DisabledObjects = null;
        private PlayerCharacter m_Player = null;

        public event Action<RecordComponent, bool> ItemAddedChanged = null;

        private void Start()
        {
            var disabledObjectGO = new GameObject("DisabledObjects");
            disabledObjectGO.SetActive(false);

            m_DisabledObjects = disabledObjectGO.GetComponent<Transform>();
            m_Player = GetComponent<PlayerCharacter>();
        }

        public void Equip(RecordComponent item, BodyPartIndex part)
        {

        }

        public void Unequip(RecordComponent item)
        {

        }

        public void AddItem(RecordComponent item)
        {
            // For now.
            var weapon = item as Weapon;
            if (weapon != null)
            {
                var rightHand = m_Player.RightHandSocket;
                if (rightHand.childCount > 0)
                {
                    rightHand.GetChild(0).parent = m_DisabledObjects;
                }
                weapon.Equip(rightHand, m_Player.RightHandContainer);
                return;
            }

            item.transform.parent = m_DisabledObjects.transform;
            ItemAddedChanged?.Invoke(item, true);
        }

        public void DropItem(RecordComponent item, Transform parent, Vector3 position, Quaternion rotation)
        {
            item.transform.parent = parent;
            item.transform.position = position;
            item.transform.rotation = rotation;

            ItemAddedChanged?.Invoke(item, false);
        }
    }
}