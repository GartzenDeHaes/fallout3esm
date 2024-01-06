using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity.Inputs
{
    public static class InputManager
    {
        private static InputActionAsset InputActions = null;

        static InputManager()
        {
            InputActions = Resources.Load<InputActionAsset>("Input/TESInputActions");
        }

        public static InputActionMap GetActionMap(string actionMap)
        {
            var map = InputActions.FindActionMap(actionMap);

            if (map == null)
            {
                Debug.LogError($"Can't find the action map {actionMap}");
            }

            return map;
        }

        public static InputAction GetAction(string actionMap, string actionName)
        {
            var map = GetActionMap(actionMap);
            return map?[actionName];
        }

        public static InputActionMap Enable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Enable();
            return map;
        }

        public static InputActionMap Disable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Disable();
            return map;
        }
    }
}
