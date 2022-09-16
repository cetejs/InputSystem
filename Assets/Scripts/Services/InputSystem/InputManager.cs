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
        private VirtualInput xboxInput = new XboxInput();
        private VirtualInput ps4Input = new Ps4Input();
        private VirtualInput mobileInput = new MobileInput();
        private Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();

        private bool isForce = true;

        public Action<InputDevice> onDeviceChanged;

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
                        if (IsPs4Device())
                        {
                            SwitchDevice(InputDevice.Ps4Joystick);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxJoystick);
                        }
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }

                    break;
                case InputDevice.XboxJoystick:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else if (IsPs4Device() && IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.Ps4Joystick);
                    }

                    break;
                case InputDevice.Ps4Joystick:
                    if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else if (!IsPs4Device() && IsJoystickInput())
                    {
                        SwitchDevice(InputDevice.XboxJoystick);
                    }

                    break;
                case InputDevice.Mobile:
                    if (IsJoystickInput())
                    {
                        if (IsPs4Device())
                        {
                            SwitchDevice(InputDevice.Ps4Joystick);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxJoystick);
                        }
                    }
                    else if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }

                    break;
            }
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
                case InputDevice.XboxJoystick:
                    activeInput = xboxInput;
                    break;
                case InputDevice.Ps4Joystick:
                    activeInput = ps4Input;
                    break;
                case InputDevice.Mobile:
                    activeInput = mobileInput;
                    break;
            }

            onDeviceChanged?.Invoke(device);
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

        private InputMapping GetInputMapping(string name)
        {
            if (inputMappings.TryGetValue(name, out var input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
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
                        xbox = column[2],
                        ps4 = column[3],
                        mobile = column[4],
                    };

                    inputMappings.Add(mapping.buttonName, mapping);
                }
            }
        }

        private bool IsMouseKeyboard()
        {
#if MOBILE_INPUT
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

            if (Input.GetAxis("x axis") != 0.0f ||
                Input.GetAxis("y axis") != 0.0f ||
                Input.GetAxis("3rd axis") != 0.0f ||
                Input.GetAxis("6th axis") != 0.0f ||
                Input.GetAxis("7th axis") != 0.0f ||
                Input.GetAxis("8th axis") != 0.0f ||
                Input.GetAxis("9th axis") != 0.0f ||
                Input.GetAxis("10th axis") != 0.0f)
            {
                return true;
            }

            if (IsPs4Device() && isForce)
            {
                if (Input.GetAxis("4th axis") > -1.0f ||
                    Input.GetAxis("5th axis") > -1.0f)
                {
                    return true;
                }
            }
            else
            {
                if (Input.GetAxis("4th axis") != 0.0f ||
                    Input.GetAxis("5th axis") != 0.0f)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMobileInput()
        {
#if MOBILE_INPUT
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject() && Event.current.isMouse || Input.touchCount > 0)
            {
                return true;
            }
#endif

            return false;
        }

        private bool IsXboxDevice()
        {
            var names = Input.GetJoystickNames();
            if (names.Length > 0)
            {
                return names[0].Length == 33;
            }

            return false;
        }

        private bool IsPs4Device()
        {
            var names = Input.GetJoystickNames();
            if (names.Length > 0)
            {
                return names[0].Length == 19;
            }

            return false;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            isForce = hasFocus;
        }
    }

    public enum InputDevice
    {
        MouseKeyboard,
        XboxJoystick,
        Ps4Joystick,
        Mobile,
    }

    public class InputMapping
    {
        public string buttonName;
        public string keyboard;
        public string xbox;
        public string ps4;
        public string mobile;
    }
}