using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wacki
{
    public sealed class IUILaserPointer : MonoBehaviour
    {
        private GameObject m_HitPoint = null;
        private GameObject m_Pointer = null;
        private float m_DistanceLimit;
        private bool m_Enabled = true;
        private bool m_Initialized = false;
        private bool m_Locked = false;
        private bool m_InputPressed = false;
        private bool m_Ready = false;

        [SerializeField]
        private float m_LaserThickness = 0.002f;
        [SerializeField]
        private float m_LaserHitScale = 0.02f;
        [SerializeField]
        private Color m_Color = Color.blue;
        [SerializeField]
        private Material m_LaserMaterial = null;
        [SerializeField]
        private bool m_AutoInitialize = false;
        //[SerializeField]
        //private bool m_Left = false;

        public InputAction PressAction = null;

        public bool HapicEnabled { get; set; } = true;

        public bool IsActive
        {
            get => m_Enabled;
            set
            {
                if (!m_Initialized)
                {
                    Initialize();
                }

                m_Enabled = value;
                m_Locked = false;
                m_HitPoint.SetActive(value);
                m_Pointer.SetActive(value);

                if (value)
                {
                    PressAction?.Enable();
                }
                else
                {
                    PressAction?.Disable();
                }
            }
        }

        public bool LineVisible
        {
            get => m_Pointer.activeSelf;
            set
            {
                if (!m_Initialized)
                {
                    Initialize();
                }

                m_Pointer.GetComponent<MeshRenderer>().enabled = value;
            }
        }

        public Material SharedMaterial
        {
            get => m_HitPoint.GetComponent<MeshRenderer>().sharedMaterial;
            set
            {
                m_Pointer.GetComponent<MeshRenderer>().sharedMaterial = value;
                m_HitPoint.GetComponent<MeshRenderer>().sharedMaterial = value;
            }
        }

        private void Awake()
        {
            if (m_AutoInitialize)
            {
                Initialize(m_LaserMaterial);

                if (PressAction != null && !PressAction.enabled)
                {
                    PressAction.Enable();
                }
            }
        }

        private void OnEnable()
        {
            PressAction?.Enable();
            m_Locked = false;
        }

        private void OnDisable()
        {
            PressAction?.Disable();
            m_Locked = false;
        }

        private void OnApplicationPause(bool pause)
        {
            m_Locked = false;
        }

        public void InitializeInput()
        {
            if (PressAction == null)
            {
                return;
            }

            PressAction.started += c => m_InputPressed = true;
            PressAction.canceled += c => m_InputPressed = false;
        }

        public void Initialize(Material newMaterial = null)
        {
            if (m_Initialized)
            {
                return;
            }

            m_Pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_Pointer.transform.SetParent(transform, false);
            m_Pointer.transform.localScale = new Vector3(m_LaserThickness, m_LaserThickness, 100.0f);
            m_Pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

            m_HitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_HitPoint.transform.SetParent(transform, false);
            m_HitPoint.transform.localScale = new Vector3(m_LaserHitScale, m_LaserHitScale, m_LaserHitScale);
            m_HitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

            m_HitPoint.SetActive(false);

            // remove the colliders on our primitives
            Destroy(m_HitPoint.GetComponent<SphereCollider>());
            Destroy(m_Pointer.GetComponent<BoxCollider>());

            if (newMaterial == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null)
                {
                    Debug.LogError("The Shader Unlit was not found. using Lit instead.");
                    shader = Shader.Find("Universal Render Pipeline/Lit");
                }

                newMaterial = new Material(shader);
                newMaterial.SetColor("_BaseColor", m_Color);
            }

            m_Pointer.GetComponent<MeshRenderer>().sharedMaterial = newMaterial;
            m_HitPoint.GetComponent<MeshRenderer>().sharedMaterial = newMaterial;

            InitializeInput();

            m_Initialized = true;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            IsActive = isActive;
        }

        private void Update()
        {
            if (!m_Enabled || m_Pointer == null)
            {
                return;
            }

            if (!m_Ready)
            {
                if (LaserPointerInputModule.TryGetInstance(out LaserPointerInputModule module))
                {
                    module.AddController(this);
                    m_Ready = true;
                }
            }

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            bool bHit = Physics.Raycast(ray, out hitInfo);

            float distance = 100.0f;

            if (bHit)
            {
                distance = hitInfo.distance;
            }

            // ugly, but has to do for now
            if (m_DistanceLimit > 0.0f)
            {
                distance = Mathf.Min(distance, m_DistanceLimit);
                bHit = true;
            }

            m_Pointer.transform.localScale = new Vector3(m_LaserThickness, m_LaserThickness, distance);
            m_Pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

            if (bHit)
            {
                m_HitPoint.SetActive(true);
                m_HitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }
            else
            {
                m_HitPoint.SetActive(false);
            }

            // reset the previous distance limit
            m_DistanceLimit = -1.0f;
        }

        public void LimitLaserDistance(float distance)
        {
            if (distance < 0.0f)
            {
                return;
            }

            if (m_DistanceLimit < 0.0f)
            {
                m_DistanceLimit = distance;
            }
            else
            {
                m_DistanceLimit = Mathf.Min(m_DistanceLimit, distance);
            }
        }

        public void OnEnterControl(GameObject control)
        {
        }

        public void OnExitControl(GameObject control)
        {
        }

        public bool ButtonDown()
        {
            return m_InputPressed && !m_Locked;
        }

        public bool ButtonUp()
        {
            return !m_InputPressed && !m_Locked;
        }

        public void RequestLockControls()
        {
            if (!m_Locked)
            {
                StartCoroutine(LockControls());
            }
        }

        private IEnumerator LockControls()
        {
            m_Locked = true;
            yield return new WaitForSeconds(0.5f);
            m_Locked = false;
        }
    }
}