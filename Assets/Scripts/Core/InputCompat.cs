using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Breach.Core
{
    public static class InputCompat
    {
        public static bool GetKeyDown(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
            {
                return false;
            }

            var key = keyCode switch
            {
                KeyCode.Tab => Key.Tab,
                KeyCode.H => Key.H,
                KeyCode.F => Key.F,
                KeyCode.M => Key.M,
                KeyCode.T => Key.T,
                KeyCode.E => Key.E,
                _ => Key.None
            };

            return key != Key.None && Keyboard.current[key].wasPressedThisFrame;
#else
            return Input.GetKeyDown(keyCode);
#endif
        }

        public static bool GetMouseButtonDown(int button)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null)
            {
                return false;
            }

            return button switch
            {
                0 => Mouse.current.leftButton.wasPressedThisFrame,
                1 => Mouse.current.rightButton.wasPressedThisFrame,
                _ => false
            };
#else
            return Input.GetMouseButtonDown(button);
#endif
        }

        public static Vector2 MousePosition
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
                return Input.mousePosition;
#endif
            }
        }

        public static Vector2 MoveVector
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current == null)
                {
                    return Vector2.zero;
                }

                var x = 0f;
                var y = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    x -= 1f;
                }
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    x += 1f;
                }
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                {
                    y -= 1f;
                }
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                {
                    y += 1f;
                }
                return new Vector2(x, y);
#else
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
            }
        }
    }
}
