using System;
using System.Collections.Generic;
using UnityEngine;

namespace UD.Services.InputSystem
{
    public class InputData
    {
        private Dictionary<string, InputMapping> inputMappings = new Dictionary<string, InputMapping>();
        private Dictionary<string, InputMapping> boundMappings = new Dictionary<string, InputMapping>();
        private Dictionary<string, JoystickMapping> joystickMappings = new Dictionary<string, JoystickMapping>();

        public void Init()
        {
            CollectInputMapping();
            CollectBoundMapping();
            CollectJoystickMapping();
        }

        public InputMapping GetInputMapping(string name)
        {
            if (inputMappings.TryGetValue(name, out var input))
            {
                return input;
            }

            Debug.LogError($"InputMapping is not exist key {name}");
            return null;
        }

        public InputMapping GetBoundMapping(string name)
        {
            if (boundMappings.TryGetValue(name, out var input))
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

        public JoystickMapping GetJoystickMapping(string name)
        {
            if (joystickMappings.TryGetValue(name, out var joystick))
            {
                return joystick;
            }

            Debug.LogError($"JoystickMapping is not exist key {name}");
            return null;
        }

        public string GetInputDeviceMapping(InputMapping input, InputDevice device)
        {
            switch (device)
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

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, string value)
        {
            switch (device)
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

        public void SetInputDeviceMapping(InputMapping input, InputDevice device, InputMapping value)
        {
            switch (device)
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

        public bool IsRebindConflict(string bindName, InputDevice device, out string conflictName)
        {
            conflictName = null;
            foreach (var name in inputMappings.Keys)
            {
                var input = GetBoundMapping(name);
                var keyName = GetInputDeviceMapping(input, device);
                if (keyName == bindName)
                {
                    conflictName = name;
                    return true;
                }
            }

            return false;
        }

        public void RebindButton(string name, InputDevice device, string bindName)
        {
            if (boundMappings.TryGetValue(name, out var input))
            {
                SetInputDeviceMapping(input, device, bindName);
                PlayerPrefs.SetString(string.Concat(name, "_", device), bindName);
            }
            else
            {
                Debug.LogError($"InputMapping is not exist key {name}");
            }
        }

        public void ResetButton(string name, InputDevice device)
        {
            var inputMapping = GetInputMapping(name);
            var boundMapping = GetInputMapping(name);
            if (inputMapping == null || boundMapping == null)
            {
                return;
            }

            if (GetInputDeviceMapping(inputMapping, device) == GetInputDeviceMapping(boundMapping, device))
            {
                return;
            }
            
            SetInputDeviceMapping(boundMapping, device, inputMapping);
            PlayerPrefs.DeleteKey(string.Concat(name, "_", device));
        }

        public void ForeachJoystickMappings(Action<JoystickMapping> onForeach)
        {
            foreach (var joystick in joystickMappings.Values)
            {
                onForeach?.Invoke(joystick);
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
                        description = column[1],
                        keyboard = column[2],
                        xbox = column[3],
                        ps4 = column[4],
                        mobile = column[5],
                    };

                    inputMappings.Add(mapping.buttonName, mapping);
                }
            }
        }

        private void CollectBoundMapping()
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

                boundMappings.Add(name, rebind);
            }
        }

        private void CollectJoystickMapping()
        {
            if (joystickMappings.Count > 0)
            {
                return;
            }

            var textAsset = Resources.Load<TextAsset>("JoystickMapping");
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
                    var mapping = new JoystickMapping()
                    {
                        joystick = column[0],
                        xbox = column[1],
                        ps4 = column[2],
                        isAxis = byte.Parse(column[3]) > 0,
                        isInvert = byte.Parse(column[4]) > 0
                    };

                    joystickMappings.Add(mapping.joystick, mapping);
                }
            }
        }
    }

    public class InputMapping
    {
        public string buttonName;
        public string description;
        public string keyboard;
        public string xbox;
        public string ps4;
        public string mobile;
    }

    public class JoystickMapping
    {
        public string joystick;
        public string xbox;
        public string ps4;
        public bool isAxis;
        public bool isInvert;
    }
}