using UD.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace UD.Services.InputSystem
{
    public class InputManager : PersistentService
    {
        private InputDevice inputDevice;
        private VirtualInput activeInput;
        private InputData inputData = new InputData();
        private Dictionary<int, VirtualInput> virtualInputs = new Dictionary<int, VirtualInput>();

        private readonly List<KeyCode> joystickButtons = new List<KeyCode>
        {
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

        private readonly List<string> joystickAxes = new List<string>
        {
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

        private bool isForce = true;
        private string inputName;
        private float inputAxis;

        public Action<InputDevice> onDeviceChanged;

        public InputDevice InputDevice
        {
            get { return inputDevice; }
        }

        public bool IsJoystickDevice
        {
            get
            {
                return inputDevice == InputDevice.XboxGamepad ||
                       inputDevice == InputDevice.Ps4Gamepad;
            }
        }

        public string InputName
        {
            get { return inputName; }
        }

        public float InputAxis
        {
            get { return inputAxis; }
        }

        private void Awake()
        {
            InitEventSystem();
            InitInputDevice();
            SelectDefaultDevice();
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
                        UpdateMouseKeyboard();
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

        private void Update()
        {
            switch (inputDevice)
            {
                case InputDevice.XboxGamepad:
                case InputDevice.Ps4Gamepad:
                    UpdateJoystickInput();
                    break;
                case InputDevice.Mobile:
                    inputName = null;
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
            activeInput = virtualInputs[(int) device];
            onDeviceChanged?.Invoke(device);
        }

        public float GetAxis(string name)
        {
            var input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0.0f;
            }

            return activeInput.GetAxis(input);
        }

        public float GetAxisRaw(string name)
        {
            var input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return 0.0f;
            }

            return activeInput.GetAxisRaw(input);
        }

        public bool GetButton(string name)
        {
            var input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButton(input);
        }

        public bool GetButtonDown(string name)
        {
            var input = inputData.GetBoundMapping(name);
            if (input == null)
            {
                return false;
            }

            return activeInput.GetButtonDown(input);
        }

        public bool GetButtonUp(string name)
        {
            var input = inputData.GetBoundMapping(name);
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
            return inputData.IsRebindConflict(bindName, inputDevice, out conflictName);
        }

        public void RebindButton(string name, string bindName)
        {
            inputData.RebindButton(name, inputDevice, bindName);
        }

        public void ResetButton(string name)
        {
            inputData.ResetButton(name, inputDevice);
        }

        public string GetDescription(string name)
        {
            var input = inputData.GetInputMapping(name);
            if (input != null)
            {
                var description = input.description;
                if (string.IsNullOrEmpty(description))
                {
                    description = input.buttonName;
                }

                return description;
            }

            return null;
        }

        public string GetActiveInputMapping(string name)
        {
            var input = inputData.GetInputMapping(name);
            if (input != null)
            {
                return inputData.GetInputDeviceMapping(input, inputDevice);
            }

            return null;
        }

        public string GetActiveBoundMapping(string name)
        {
            var input = inputData.GetBoundMapping(name);
            if (input != null)
            {
                return inputData.GetInputDeviceMapping(input, inputDevice);
            }

            return null;
        }

        public void SelectDefaultGo()
        {
            if (IsJoystickDevice)
            {
                if (!EventSystem.current.currentSelectedGameObject)
                {
                    var selectable = FindObjectOfType<Selectable>();
                    if (selectable)
                    {
                        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                    }
                }
            }
        }

        private void InitEventSystem()
        {
            if (EventSystem.current == null)
            {
                EventSystem.current = new GameObject("EventSystem").AddComponent<EventSystem>();
                EventSystem.current.gameObject.AddComponent<StandaloneInputModule>();
            }

            var inputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            var customInput = inputModule.gameObject.AddComponent<CustomInput>();
            customInput.onGetButtonDown = GetButtonDown;
            customInput.onGetAxisRaw = GetAxisRaw;
            inputModule.inputOverride = customInput;
            inputModule.horizontalAxis = "HorizontalNav";
            inputModule.verticalAxis = "VerticalNav";
            inputModule.submitButton = "Submit";
            inputModule.cancelButton = "Cancel";
        }

        private void InitInputDevice()
        {
            inputData.Init();
            virtualInputs.Add((int) InputDevice.MouseKeyboard, new StandaloneInput());
            virtualInputs.Add((int) InputDevice.XboxGamepad, new XboxInput(inputData));
            virtualInputs.Add((int) InputDevice.Ps4Gamepad, new Ps4Input(inputData));
            virtualInputs.Add((int) InputDevice.Mobile, new MobileInput());
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
            for (int i = 0; i < joystickButtons.Count; i++)
            {
                var button = joystickButtons[i];
                if (Input.GetKey(button))
                {
                    return true;
                }
            }

            for (int i = 0; i < joystickAxes.Count; i++)
            {
                var axis = joystickAxes[i];
                if ((i == 3 || i == 4) && IsPs4Device() && isForce)
                {
                    if (Input.GetAxis(axis) > -1.0f)
                    {
                        return true;
                    }
                }
                else if (Input.GetAxis(axis) != 0.0f)
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

        private void UpdateMouseKeyboard()
        {
            if (Event.current.isKey && Event.current.keyCode != KeyCode.None)
            {
                inputName = Event.current.keyCode.ToString();
                inputAxis = 0.0f;
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    inputName = string.Concat("Mouse ", i);
                    inputAxis = 0.0f;
                    return;
                }
            }

            inputName = null;
            inputAxis = 0.0f;
        }

        private void UpdateJoystickInput()
        {
            for (int i = 0; i < joystickButtons.Count; i++)
            {
                var button = joystickButtons[i];
                if (Input.GetKeyDown(button))
                {
                    var joystick = inputData.GetJoystickMapping(button.ToString());
                    if (joystick != null)
                    {
                        inputName = IsPs4Device() ? joystick.ps4 : joystick.xbox;
                        inputAxis = 0.0f;
                        return;
                    }
                }
            }

            for (int i = 0; i < joystickAxes.Count; i++)
            {
                var axis = joystickAxes[i];
                var axisValue = Input.GetAxis(axis);
                if ((i == 3 || i == 4) && IsPs4Device() && isForce)
                {
                    if (axisValue > -1.0f)
                    {
                        var joystick = inputData.GetJoystickMapping(axis);
                        if (joystick != null)
                        {
                            inputName = joystick.ps4;
                            inputAxis = 1.0f;
                            return;
                        }
                    }
                }
                else if (axisValue != 0.0f)
                {
                    var joystick = inputData.GetJoystickMapping(axis);
                    if (joystick != null)
                    {
                        inputName = IsPs4Device() ? joystick.ps4 : joystick.xbox;
                        inputAxis = axisValue;
                        return;
                    }
                }
            }

            inputName = null;
            inputAxis = 0.0f;
        }
    }

    public enum InputDevice
    {
        MouseKeyboard,
        XboxGamepad,
        Ps4Gamepad,
        Mobile,
    }
}