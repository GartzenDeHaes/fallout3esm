using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class Weapon : RecordComponent
    {
        private bool _isVisible = true;
        private Transform _hand = null;
        private Transform _container = null;
        private Renderer[] _renderers = null;

        private void Start()
        {
            var WEAP = (WEAPRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = WEAP.Name;
            objData.weight = WEAP.Data.Weight.ToString();
            objData.value = WEAP.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            _renderers = GetComponentsInChildren<Renderer>();

            // Replace MeshCollider by Box. Will change in the future.
            var meshColliders = GetComponentsInChildren<MeshCollider>();
            for (int i = 0; i < meshColliders.Length; i++)
            {
                meshColliders[i].gameObject.AddComponent<BoxCollider>();
                Destroy(meshColliders[i]);
            }

            gameObject.isStatic = false;

            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.isStatic = false;
            }

            TryAddScript(WEAP.Script);
        }

        public void SetVisible(bool visible)
        {
            if (visible == _isVisible)
            {
                return;
            }

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = visible;
            }

            _isVisible = visible;
        }

        public void Equip(Transform hand, Transform container)
        {
            m_transform.parent = hand;
            m_transform.localPosition = Vector3.zero;
            m_transform.localRotation = Quaternion.identity;
            _hand = hand;
            _container = container;
        }

        public void Unequip(Transform disabledObjects)
        {
            m_transform.parent = disabledObjects;
            _hand = null;
            _container = null;
        }
    }
}
