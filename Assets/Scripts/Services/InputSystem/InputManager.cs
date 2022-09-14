using UD.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

namespace UD.Services.InputSystem
{
    public class InputManager : PersistentService
    {
        private InputDevice inputDevice;
        private VirtualInput activeInput;
        private VirtualInput standaloneInput = new StandaloneInput();
        private VirtualInput joystickInput = new JoystickInput();
        private VirtualInput mobileInput = new MobileInput();

        private Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();

        public InputDevice InputDevice
        {
            get { return inputDevice; }
        }

        private void Awake()
        {
            SwitchDevice(InputDevice.MouseKeyboard);
            CollectInputMapping();
        }

        private void OnGUI()
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    if (IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.Joystick);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    break;
                case InputDevice.Joystick:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    break;
                case InputDevice.Mobile:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.Joystick);
                    }
                    break;
            }
        }

        private void CollectInputMapping()
        {
            var textAsset = Resources.Load<TextAsset>("InputMapping");
            if (!textAsset)
            {
                Debug.LogError("Resources dont exist InputMapping");
            }
            else
            {
                var lines = textAsset.text.Split(new[]
                {
                    "\r\n",
                    "\n"
                }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 3; i < lines.Length; i++)
                {
                    var column = lines[i].Split('\t');
                    var mapping = new InputMapping()
                    {
                        buttonName = column[0],
                        keyboard = column[1],
                        joystick = column[2],
                        mobile = column[3],
                        isAxis = byte.Parse(column[4]) > 0
                    };

                    inputMappings.Add(mapping.buttonName, mapping);
                }
            }
        }

        private bool IsMouseKeyboard()
        {
#if MOBLIE_INPUT
            return false;
#endif
            if (Event.current.isKey || Event.current.isMouse)
            {
                return true;
            }

            if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f)
            {
                return true;
            }

            return false;
        }

        private bool IsJoystickInput()
        {
            if (Input.GetKey(KeyCode.Joystick1Button0) ||
                Input.GetKey(KeyCode.Joystick1Button1) ||
                Input.GetKey(KeyCode.Joystick1Button2) ||
                Input.GetKey(KeyCode.Joystick1Button3) ||
                Input.GetKey(KeyCode.Joystick1Button4) ||
                Input.GetKey(KeyCode.Joystick1Button5) ||
                Input.GetKey(KeyCode.Joystick1Button6) ||
                Input.GetKey(KeyCode.Joystick1Button7) ||
                Input.GetKey(KeyCode.Joystick1Button8) ||
                Input.GetKey(KeyCode.Joystick1Button9) ||
                Input.GetKey(KeyCode.Joystick1Button10) ||
                Input.GetKey(KeyCode.Joystick1Button11) ||
                Input.GetKey(KeyCode.Joystick1Button12) ||
                Input.GetKey(KeyCode.Joystick1Button13) ||
                Input.GetKey(KeyCode.Joystick1Button14) ||
                Input.GetKey(KeyCode.Joystick1Button15) ||
                Input.GetKey(KeyCode.Joystick1Button16) ||
                Input.GetKey(KeyCode.Joystick1Button17) ||
                Input.GetKey(KeyCode.Joystick1Button18) ||
                Input.GetKey(KeyCode.Joystick1Button19))
            {
                return true;
            }

            if (Input.GetAxis("LeftAnalogHorizontal") != 0.0f ||
                Input.GetAxis("LeftAnalogVertical") != 0.0f ||
                Input.GetAxis("RightAnalogHorizontal") != 0.0f ||
                Input.GetAxis("RightAnalogVertical") != 0.0f ||
                Input.GetAxis("LT") != 0.0f ||
                Input.GetAxis("RT") != 0.0f ||
                Input.GetAxis("D-Pad Horizontal") != 0.0f ||
                Input.GetAxis("D-Pad Vertical") != 0.0f)
            {
                return true;
            }

            return false;
        }

        private bool IsMobileInput()
        {

#if UNITY_EDITOR && UNITY_MOBILE
            if (EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                return true;
            }

#elif MOBLE_INPUT
            if (EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0)
            {
                return true;
            }
#endif

            return false;
        }

        private InputMapping GetInputMapping(string name)
        {
            if (inputMappings.TryGetValue(name, out var input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {input}");
            return null;
        }

        public void SwitchDevice(InputDevice device)
        {
            if (device == inputDevice && activeInput != null)
            {
                return;
            }

            inputDevice = device;
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    activeInput = standaloneInput;
                    break;
                case InputDevice.Joystick:
                    activeInput = joystickInput;
                    break;
                case InputDevice.Mobile:
                    activeInput = mobileInput;
                    break;
            }
        }

        public float GetAxis(string name)
        {
            var input = GetInputMapping(name);
            if (input == null)
            {
                return 0.0f;
            }

            return activeInput.GetAxis(input);
        }

        public float GetAxisRaw(string name)
        {
            var input = GetInputMapping(name);
            if (input == null)
            {
                return 0.0f;
            }

            return activeInput.GetAxisRaw(input);
        }

        public bool GetButton(string name)
        {
            var input = GetInputMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButton(input);
        }

        public bool GetButtonDown(string name)
        {
            var input = GetInputMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButtonDown(input);
        }

        public bool GetButtonUp(string name)
        {
            var input = GetInputMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButtonUp(input);
        }

        public void SetAxisPositive(string name)
        {
            activeInput.SetAxis(name, 1);
        }

        public void SetAxisNegative(string name)
        {
            activeInput.SetAxis(name, -1);
        }

        public void SetAxisZero(string name)
        {
            activeInput.SetAxis(name, 0);
        }

        public void SetAxis(string name, float value)
        {
            activeInput.SetAxis(name, value);
        }

        public void SetButtonDown(string name)
        {
            activeInput.SetButtonDown(name);
        }

        public void SetButtonUp(string name)
        {
            activeInput.SetButtonUp(name);
        }
    }

    public enum InputDevice
    {
        MouseKeyboard,
        Joystick,
        Mobile,
    }

    public class InputMapping
    {
        public string buttonName;
        public string keyboard;
        public string joystick;
        public string mobile;
        public bool isAxis;
    }
}