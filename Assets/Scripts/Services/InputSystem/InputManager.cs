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
        private Dictionary<string, InputMapping> bindedMappings = new Dictionary<string, InputMapping>();

        private readonly List<KeyCode> joystickButtons = new List<KeyCode> {
            KeyCode.Joystick1Button0,
            KeyCode.Joystick1Button1,
            KeyCode.Joystick1Button2,
            KeyCode.Joystick1Button3,
            KeyCode.Joystick1Button4,
            KeyCode.Joystick1Button5,
            KeyCode.Joystick1Button6,
            KeyCode.Joystick1Button7,
            KeyCode.Joystick1Button8,
            KeyCode.Joystick1Button9,
            KeyCode.Joystick1Button10,
            KeyCode.Joystick1Button11,
            KeyCode.Joystick1Button12,
            KeyCode.Joystick1Button13,
            KeyCode.Joystick1Button14,
            KeyCode.Joystick1Button15,
            KeyCode.Joystick1Button16,
            KeyCode.Joystick1Button17,
            KeyCode.Joystick1Button18,
            KeyCode.Joystick1Button19,
        };

        private readonly List<string> joystickAxes = new List<string> {
            "x axis",
            "y axis",
            "3rd axis",
            "4th axis",
            "5th axis",
            "6th axis",
            "7th axis",
            "8th axis",
            "9th axis",
            "10th axis"
        };

        private bool isForce;
        private string inputKey;

        public Action<InputDevice> onDeviceChanged;

        public InputDevice InputDevice
        {
            get { return inputDevice; }
        }

        private void Awake()
        {
            SelectDefaultDevice();
            CollectInputMapping();
            CollectBindedMapping();
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
                            SwitchDevice(InputDevice.Ps4Gamepad);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxGamepad);
                        }
                    }
                    else if (IsMobileInput())
                    {
                        SwitchDevice(InputDevice.Mobile);
                    }
                    else 
                    {
                    
                    }

                    break;
                case InputDevice.XboxGamepad:
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
                        SwitchDevice(InputDevice.Ps4Gamepad);
                    }

                    break;
                case InputDevice.Ps4Gamepad:
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
                        SwitchDevice(InputDevice.XboxGamepad);
                    }

                    break;
                case InputDevice.Mobile:
                    if (IsJoystickInput())
                    {
                        if (IsPs4Device())
                        {
                            SwitchDevice(InputDevice.Ps4Gamepad);
                        }
                        else
                        {
                            SwitchDevice(InputDevice.XboxGamepad);
                        }
                    }
                    else if (IsMouseKeyboard())
                    {
                        SwitchDevice(InputDevice.MouseKeyboard);
                    }

                    break;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            isForce = hasFocus;
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
                case InputDevice.XboxGamepad:
                    activeInput = xboxInput;
                    break;
                case InputDevice.Ps4Gamepad:
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

        public bool IsRebindConflict(string bindName, out string conflictName)
        {
            conflictName = null;
            foreach (var name in inputMappings.Keys)
            {
                var input = GetInputMapping(name);
                var keyName = GetInputDeviceMapping(input, inputDevice);
                if (keyName == bindName)
                {
                    conflictName = name;
                    return true;
                }
            }

            return false;
        }

        public void RebindButton(string name, string bindName)
        {
            if (bindedMappings.TryGetValue(name, out var input))
            {
                SetInputDeviceMapping(input, inputDevice, bindName);
                PlayerPrefs.SetString(string.Concat(name, "_", inputDevice), bindName);
            }
            else
            {
                Debug.LogError($"InputMapping is not exist key {name}");
            }
        }

        public void ResetButton(string name)
        {
            if (inputMappings.TryGetValue(name, out var input))
            {
                var binded = bindedMappings[name];
                SetInputDeviceMapping(binded, inputDevice, input);
                PlayerPrefs.DeleteKey(string.Concat(name, "_", inputDevice));
            }
        }

        private void SelectDefaultDevice()
        {
            if (IsXboxDevice())
            {
                SwitchDevice(InputDevice.XboxGamepad);
            }
            else if (IsPs4Device())
            {
                SwitchDevice(InputDevice.Ps4Gamepad);
            }
            else if (IsMobileInput())
            {
                SwitchDevice(InputDevice.Mobile);
            }
            else
            {
                SwitchDevice(InputDevice.MouseKeyboard);
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
                        xbox = column[2],
                        ps4 = column[3],
                        mobile = column[4],
                    };

                    inputMappings.Add(mapping.buttonName, mapping);
                }
            }
        }

        private void CollectBindedMapping()
        {
            foreach (var name in inputMappings.Keys)
            {
                var rebind = new InputMapping()
                {
                    buttonName = name,
                    keyboard = PlayerPrefs.GetString(string.Concat(name, "_", InputDevice.MouseKeyboard), inputMappings[name].keyboard),
                    xbox = PlayerPrefs.GetString(string.Concat(name, "_", InputDevice.XboxGamepad), inputMappings[name].xbox),
                    ps4 = PlayerPrefs.GetString(string.Concat(name, "_", InputDevice.Ps4Gamepad), inputMappings[name].ps4),
                    mobile = PlayerPrefs.GetString(string.Concat(name, "_", InputDevice.Mobile), inputMappings[name].mobile)
                };

                bindedMappings.Add(name, rebind);
            }
        }

        private InputMapping GetInputMapping(string name)
        {
            if (bindedMappings.TryGetValue(name, out var input))
            {
                return input;
            }

            if (inputMappings.TryGetValue(name, out input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        private string GetInputDeviceMapping(InputMapping input, InputDevice device)
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    return input.keyboard;
                case InputDevice.XboxGamepad:
                    return input.xbox;
                case InputDevice.Ps4Gamepad:
                    return input.ps4;
                case InputDevice.Mobile:
                    return input.mobile;
                default:
                    return input.keyboard;
            }
        }

        private void SetInputDeviceMapping(InputMapping input, InputDevice device, string value)
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    input.keyboard = value;
                    break;
                case InputDevice.XboxGamepad:
                    input.xbox = value;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.ps4 = value;
                    break;
                case InputDevice.Mobile:
                    input.mobile = value;
                    break;
            }
        }

        private void SetInputDeviceMapping(InputMapping input, InputDevice device, InputMapping value)
        {
            switch (inputDevice)
            {
                case InputDevice.MouseKeyboard:
                    input.keyboard = value.keyboard;
                    break;
                case InputDevice.XboxGamepad:
                    input.xbox = value.xbox;
                    break;
                case InputDevice.Ps4Gamepad:
                    input.ps4 = value.ps4;
                    break;
                case InputDevice.Mobile:
                    input.mobile = value.mobile;
                    break;
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

        private string GetMouseKeyboard()
        {
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                inputKey = Event.current.ToString();
            }

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    inputKey = string.Concat("Mouse ", i);
                }
            }

            return null;
        }

        //private string GetJoystickInput()
        //{
        //    if (Input.GetKey(KeyCode.Joystick1Button0) ||
        //        Input.GetKey(KeyCode.Joystick1Button1) ||
        //        Input.GetKey(KeyCode.Joystick1Button2) ||
        //        Input.GetKey(KeyCode.Joystick1Button3) ||
        //        Input.GetKey(KeyCode.Joystick1Button4) ||
        //        Input.GetKey(KeyCode.Joystick1Button5) ||
        //        Input.GetKey(KeyCode.Joystick1Button6) ||
        //        Input.GetKey(KeyCode.Joystick1Button7) ||
        //        Input.GetKey(KeyCode.Joystick1Button8) ||
        //        Input.GetKey(KeyCode.Joystick1Button9) ||
        //        Input.GetKey(KeyCode.Joystick1Button10) ||
        //        Input.GetKey(KeyCode.Joystick1Button11) ||
        //        Input.GetKey(KeyCode.Joystick1Button12) ||
        //        Input.GetKey(KeyCode.Joystick1Button13) ||
        //        Input.GetKey(KeyCode.Joystick1Button14) ||
        //        Input.GetKey(KeyCode.Joystick1Button15) ||
        //        Input.GetKey(KeyCode.Joystick1Button16) ||
        //        Input.GetKey(KeyCode.Joystick1Button17) ||
        //        Input.GetKey(KeyCode.Joystick1Button18) ||
        //        Input.GetKey(KeyCode.Joystick1Button19))
        //    {
        //        return true;
        //    }

        //    if (Input.GetAxis("x axis") != 0.0f ||
        //        Input.GetAxis("y axis") != 0.0f ||
        //        Input.GetAxis("3rd axis") != 0.0f ||
        //        Input.GetAxis("6th axis") != 0.0f ||
        //        Input.GetAxis("7th axis") != 0.0f ||
        //        Input.GetAxis("8th axis") != 0.0f ||
        //        Input.GetAxis("9th axis") != 0.0f ||
        //        Input.GetAxis("10th axis") != 0.0f)
        //    {
        //        return true;
        //    }

        //    return null;
        //}
    }

    public enum InputDevice
    {
        MouseKeyboard,
        XboxGamepad,
        Ps4Gamepad,
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