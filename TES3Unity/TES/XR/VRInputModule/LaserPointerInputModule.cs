using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wacki
{
    public class LaserPointerInputModule : BaseInputModule
    {
        private static LaserPointerInputModule _instance = null;

        public LayerMask layerMask = new LayerMask() { value = 1 << 5 };

        // storage class for controller specific data
        private class ControllerData
        {
            public LaserPointerEventData pointerEvent;
            public GameObject currentPoint;
            public GameObject currentPressed;
            public GameObject currentDragging;
        };

        private Camera UICamera;
        private PhysicsRaycaster raycaster;
        private HashSet<IUILaserPointer> _controllers;
        // controller data
        private Dictionary<IUILaserPointer, ControllerData> _controllerData = new Dictionary<IUILaserPointer, ControllerData>();

        public static bool TryGetInstance(out LaserPointerInputModule instance)
        {
            instance = null;

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<LaserPointerInputModule>();
            }

            if (_instance != null)
            {
                instance = _instance;
            }

            return _instance != null;
        }

        protected override void Awake()
        {
            base.Awake();

            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("Trying to instantiate multiple LaserPointerInputModule.");
                Debug.Log(_instance.name); enabled = false;
                return;
            }

            _instance = this;
        }

        protected override void Start()
        {
            base.Start();

            // Create a new camera that will be used for raycasts
            UICamera = new GameObject("UI Camera").AddComponent<Camera>();
            // Added PhysicsRaycaster so that pointer events are sent to 3d objects
            raycaster = UICamera.gameObject.AddComponent<PhysicsRaycaster>();
            UICamera.clearFlags = CameraClearFlags.Nothing;
            UICamera.stereoTargetEye = StereoTargetEyeMask.None;
            UICamera.enabled = false;
            UICamera.fieldOfView = 5;
            UICamera.nearClipPlane = 0.01f;

            // Find canvases in the scene and assign our custom
            // UICamera to them
            var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = UICamera;
            }
        }

        public void AddController(IUILaserPointer controller)
        {
            if (!_controllerData.ContainsKey(controller))
            {
                _controllerData.Add(controller, new ControllerData());
            }
        }

        public void RemoveController(IUILaserPointer controller)
        {
            if (_controllerData.ContainsKey(controller))
            {
                _controllerData.Remove(controller);
            }
        }

        protected void UpdateCameraPosition(IUILaserPointer controller)
        {
            UICamera.transform.position = controller.transform.position;
            UICamera.transform.rotation = controller.transform.rotation;
        }

        // clear the current selection
        public void ClearSelection()
        {
            if (base.eventSystem.currentSelectedGameObject)
            {
                base.eventSystem.SetSelectedGameObject(null);
            }
        }

        // select a game object
        private void Select(GameObject go)
        {
            ClearSelection();

            if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
            {
                base.eventSystem.SetSelectedGameObject(go);
            }
        }

        public override void Process()
        {
            raycaster.eventMask = layerMask;

            foreach (var pair in _controllerData)
            {
                var controller = pair.Key;
                var data = pair.Value;

                if (!controller.IsActive || !controller.enabled || !controller.gameObject.activeSelf || !controller.gameObject.activeInHierarchy)
                {
                    continue;
                }

                // Test if UICamera is looking at a GUI element
                UpdateCameraPosition(controller);

                if (data.pointerEvent == null)
                {
                    data.pointerEvent = new LaserPointerEventData(eventSystem);
                }
                else
                {
                    data.pointerEvent.Reset();
                }

                data.pointerEvent.controller = controller;
                data.pointerEvent.delta = Vector2.zero;
                data.pointerEvent.position = new Vector2(UICamera.pixelWidth * 0.5f, UICamera.pixelHeight * 0.5f);
                //data.pointerEvent.scrollDelta = Vector2.zero;

                // trigger a raycast
                eventSystem.RaycastAll(data.pointerEvent, m_RaycastResultCache);
                data.pointerEvent.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                m_RaycastResultCache.Clear();

                // make sure our controller knows about the raycast result
                // we add 0.01 because that is the near plane distance of our camera and we want the correct distance
                //if (data.pointerEvent.pointerCurrentRaycast.distance > 0.0f)
                //controller.LimitLaserDistance(data.pointerEvent.pointerCurrentRaycast.distance + 0.01f);

                // stop if no UI element was hit
                //if(pointerEvent.pointerCurrentRaycast.gameObject == null)
                //return;

                // Send control enter and exit events to our controller
                var hitControl = data.pointerEvent.pointerCurrentRaycast.gameObject;
                if (data.currentPoint != hitControl)
                {
                    if (data.currentPoint != null)
                    {
                        controller.OnExitControl(data.currentPoint);
                    }

                    if (hitControl != null)
                    {
                        controller.OnEnterControl(hitControl);
                    }
                }

                data.currentPoint = hitControl;

                // Handle enter and exit events on the GUI controlls that are hit
                base.HandlePointerExitAndEnter(data.pointerEvent, data.currentPoint);

                if (controller.ButtonDown())
                {
                    ClearSelection();

                    data.pointerEvent.pressPosition = data.pointerEvent.position;
                    data.pointerEvent.pointerPressRaycast = data.pointerEvent.pointerCurrentRaycast;
                    data.pointerEvent.pointerPress = null;

                    // update current pressed if the curser is over an element
                    if (data.currentPoint != null)
                    {
                        controller.RequestLockControls();
                        data.currentPressed = data.currentPoint;
                        data.pointerEvent.current = data.currentPressed;
                        GameObject newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerDownHandler);
                        ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.pointerDownHandler);
                        if (newPressed == null)
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                            ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                            if (newPressed != null)
                            {
                                data.currentPressed = newPressed;
                            }
                        }
                        else
                        {
                            data.currentPressed = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute(newPressed, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                            ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.pointerClickHandler);

                        }

                        if (newPressed != null)
                        {
                            data.pointerEvent.pointerPress = newPressed;
                            data.currentPressed = newPressed;
                            Select(data.currentPressed);
                        }

                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.beginDragHandler);
                        ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.beginDragHandler);

                        data.pointerEvent.pointerDrag = data.currentPressed;
                        data.currentDragging = data.currentPressed;
                    }
                }// button down end

                if (controller.ButtonUp())
                {
                    if (data.currentDragging != null)
                    {
                        data.pointerEvent.current = data.currentDragging;
                        ExecuteEvents.Execute(data.currentDragging, data.pointerEvent, ExecuteEvents.endDragHandler);
                        ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.endDragHandler);
                        if (data.currentPoint != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(data.currentPoint, data.pointerEvent, ExecuteEvents.dropHandler);
                        }
                        data.pointerEvent.pointerDrag = null;
                        data.currentDragging = null;
                        controller.RequestLockControls();
                    }
                    if (data.currentPressed)
                    {
                        data.pointerEvent.current = data.currentPressed;
                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        data.pointerEvent.rawPointerPress = null;
                        data.pointerEvent.pointerPress = null;
                        data.currentPressed = null;
                        controller.RequestLockControls();
                    }
                }

                // drag handling
                if (data.currentDragging != null)
                {
                    data.pointerEvent.current = data.currentPressed;
                    ExecuteEvents.Execute(data.currentDragging, data.pointerEvent, ExecuteEvents.dragHandler);
                    ExecuteEvents.Execute(controller.gameObject, data.pointerEvent, ExecuteEvents.dragHandler);
                }

                // update selected element for keyboard focus
                if (base.eventSystem.currentSelectedGameObject != null)
                {
                    data.pointerEvent.current = eventSystem.currentSelectedGameObject;
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                    //ExecuteEvents.Execute(controller.gameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                }
            }
        }
    }
}