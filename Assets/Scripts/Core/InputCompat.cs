using System;
using UnityEngine;

namespace Breach.Core
{
    public static class InputCompat
    {
        private const string KeyboardTypeName = "UnityEngine.InputSystem.Keyboard, Unity.InputSystem";
        private const string MouseTypeName = "UnityEngine.InputSystem.Mouse, Unity.InputSystem";

        public static bool GetKeyDown(KeyCode keyCode)
        {
            if (TryGetInputSystemKeyDown(keyCode, out var inputSystemPressed))
            {
                return inputSystemPressed;
            }
            return Input.GetKeyDown(keyCode);
        }

        public static bool IsSupportedKeyCode(KeyCode keyCode)
        {
            return true;
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (TryGetInputSystemMouseButtonDown(button, out var inputSystemPressed))
            {
                return inputSystemPressed;
            }
            return Input.GetMouseButtonDown(button);
        }

        public static bool GetMouseButton(int button)
        {
            if (TryGetInputSystemMouseButton(button, out var inputSystemPressed))
            {
                return inputSystemPressed;
            }
            return Input.GetMouseButton(button);
        }

        public static Vector2 MousePosition
        {
            get
            {
                if (TryGetInputSystemMousePosition(out var mousePosition))
                {
                    return mousePosition;
                }
                return Input.mousePosition;
            }
        }

        public static Vector2 MoveVector
        {
            get
            {
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
        }

        private static bool TryGetInputSystemKeyDown(KeyCode keyCode, out bool pressed)
        {
            pressed = false;
            var keyboardType = Type.GetType(KeyboardTypeName);
            if (keyboardType == null)
            {
                return false;
            }

            var currentKeyboard = keyboardType.GetProperty("current")?.GetValue(null);
            if (currentKeyboard == null)
            {
                return true;
            }

            var keyName = MapKeyControlName(keyCode);
            if (string.IsNullOrEmpty(keyName))
            {
                return true;
            }

            var keyControl = keyboardType.GetProperty(keyName)?.GetValue(currentKeyboard);
            if (keyControl == null)
            {
                return true;
            }

            var isPressed = keyControl.GetType().GetProperty("wasPressedThisFrame")?.GetValue(keyControl);
            pressed = isPressed is bool b && b;
            return true;
        }

        private static bool TryGetInputSystemMouseButtonDown(int button, out bool pressed)
        {
            pressed = false;
            if (!TryGetMouseControl(button, out var control))
            {
                return false;
            }

            var value = control.GetType().GetProperty("wasPressedThisFrame")?.GetValue(control);
            pressed = value is bool b && b;
            return true;
        }

        private static bool TryGetInputSystemMouseButton(int button, out bool pressed)
        {
            pressed = false;
            if (!TryGetMouseControl(button, out var control))
            {
                return false;
            }

            var value = control.GetType().GetProperty("isPressed")?.GetValue(control);
            pressed = value is bool b && b;
            return true;
        }

        private static bool TryGetInputSystemMousePosition(out Vector2 mousePosition)
        {
            mousePosition = Vector2.zero;
            var mouseType = Type.GetType(MouseTypeName);
            if (mouseType == null)
            {
                return false;
            }

            var currentMouse = mouseType.GetProperty("current")?.GetValue(null);
            if (currentMouse == null)
            {
                return true;
            }

            var positionControl = mouseType.GetProperty("position")?.GetValue(currentMouse);
            if (positionControl == null)
            {
                return true;
            }

            var readValueMethod = positionControl.GetType().GetMethod("ReadValue", Type.EmptyTypes);
            if (readValueMethod == null)
            {
                return true;
            }

            var value = readValueMethod.Invoke(positionControl, null);
            if (value is Vector2 position)
            {
                mousePosition = position;
            }

            return true;
        }

        private static bool TryGetMouseControl(int button, out object control)
        {
            control = null;
            var mouseType = Type.GetType(MouseTypeName);
            if (mouseType == null)
            {
                return false;
            }

            var currentMouse = mouseType.GetProperty("current")?.GetValue(null);
            if (currentMouse == null)
            {
                return true;
            }

            var controlName = button switch
            {
                0 => "leftButton",
                1 => "rightButton",
                _ => null
            };

            if (string.IsNullOrEmpty(controlName))
            {
                return true;
            }

            control = mouseType.GetProperty(controlName)?.GetValue(currentMouse);
            return true;
        }

        private static string MapKeyControlName(KeyCode keyCode)
        {
            return keyCode switch
            {
                KeyCode.Tab => "tabKey",
                KeyCode.H => "hKey",
                KeyCode.F => "fKey",
                KeyCode.M => "mKey",
                KeyCode.T => "tKey",
                KeyCode.E => "eKey",
                KeyCode.F1 => "f1Key",
                _ => null
            };
        }
    }
}
