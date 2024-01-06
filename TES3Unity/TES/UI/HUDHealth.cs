using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public sealed class HUDHealth : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Health = null;
        [SerializeField]
        private Slider m_Magic = null;
        [SerializeField]
        private Slider m_Stamina = null;
        [SerializeField]
        private Image m_WeaponBox = null;
        [SerializeField]
        private Image m_MagicBox = null;

        public int Health
        {
            set => m_Health.value = value;
        }

        public int Magic
        {
            set => m_Magic.value = value;
        }

        public int Stamina
        {
            set => m_Stamina.value = value;
        }

        public void EquipWeapon(Sprite icon)
        {
            m_WeaponBox.sprite = icon;
            m_WeaponBox.transform.parent.gameObject.SetActive(icon != null);
        }

        public void EquipMagic(Sprite icon)
        {
            m_MagicBox.sprite = icon;
            m_MagicBox.transform.parent.gameObject.SetActive(icon != null);
        }
    }
}
