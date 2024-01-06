using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demonixis.Toolbox.XR
{
    public class Teleporter : MonoBehaviour
    {
        private InputAction m_TeleportAction = null;
        private Transform m_Transform = null;
        private GameObject m_GroundMarker = null;
        private Transform m_GroundMarkerTransform = null;
        private GameObject m_TeleporterLine = null;
        private LineRenderer m_TeleporterLineRenderer = null;
        private Vector3? m_TargetPosition;
        private Transform m_RootTransform = null;
        private Transform m_RayPoint = null;
        private float m_InitialPositionY;
        private Vector3 m_LastGoodTarget;
        private bool m_Pressed = false;

        [SerializeField]
        private GameObject m_GroundMarkerPrefab = null;
        [SerializeField]
        private GameObject m_TeleporterLinePrefab = null;
        [SerializeField]
        private float m_MaxDistance = 15.0f;

        private void Start()
        {
            m_Transform = transform;

            m_GroundMarker = Instantiate(m_GroundMarkerPrefab);
            m_GroundMarker.SetActive(false);
            m_GroundMarkerTransform = m_GroundMarker.transform;

            m_TeleporterLine = Instantiate(m_TeleporterLinePrefab);
            m_TeleporterLine.SetActive(false);
            m_TeleporterLineRenderer = m_TeleporterLine.GetComponent<LineRenderer>();

            m_RayPoint = new GameObject("TeleporterRayPoint").transform;
            m_RayPoint.parent = transform;
            m_RayPoint.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            m_RayPoint.localRotation = Quaternion.Euler(-35.0f, 0.0f, 0.0f);

            m_RootTransform = m_Transform.root;
            m_InitialPositionY = m_RootTransform.position.y;
        }

        public void SetHand(bool left)
        {
            m_TeleportAction = InputManager.GetAction("XR", $"Teleport-{(left ? "Left" : "Right")}");
            m_TeleportAction.started += (c) => m_Pressed = true;
            m_TeleportAction.canceled += (c) =>
            {
                m_Pressed = false;
                InputWasJustReleased();
            };
        }

        public void InputIsPressed()
        {
            m_Pressed = true;

            Debug.DrawRay(m_RayPoint.transform.position, -m_RayPoint.transform.up * 10);

            RaycastHit hit;
            if (Physics.Raycast(m_RayPoint.transform.position, -m_RayPoint.transform.up, out hit))
            {
                var point = hit.point;
                var target = point;
                //target.y += m_InitialPositionY;

                m_TeleporterLineRenderer.SetPosition(0, m_Transform.position);

                // Limit the distance.
                if (Vector3.Distance(m_RootTransform.position, target) <= m_MaxDistance)
                {
                    m_TargetPosition = target;
                    m_GroundMarkerTransform.position = point;
                    m_TeleporterLineRenderer.SetPosition(1, point);
                    m_LastGoodTarget = point;
                }
                else
                {
                    m_TargetPosition = null;
                }
            }
            else
            {
                m_TargetPosition = null;
            }

            SetMarkerActive(m_TargetPosition != null);
        }

        private void SetMarkerActive(bool active)
        {
            if (m_GroundMarker.activeSelf != active)
            {
                m_GroundMarker.SetActive(active);
            }

            if (m_TeleporterLine.activeSelf != active)
            {
                m_TeleporterLine.SetActive(active);
            }
        }

        public void InputWasJustReleased()
        {
            m_Pressed = false;

            if (m_TargetPosition.HasValue)
            {
                transform.root.position = m_TargetPosition.Value;
                m_TargetPosition = null;
            }

            SetMarkerActive(false);
        }

        private void Update()
        {
            if (m_Pressed)
            {
                InputIsPressed();
            }
        }
    }
}
