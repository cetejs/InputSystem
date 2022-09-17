using UD.Globals;
using UD.Services.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UD.UI
{
    public class RebindElement : MonoBehaviour
    {
        [SerializeField]
        private Text buttonNameText;
        [SerializeField]
        private Text keyCodeText;
        [SerializeField]
        private Image joystickIcon;
        [SerializeField]
        private Button rebindButton;
        [SerializeField]
        private Button resetButton;

        private RebindControl control;
        private InputManager input;
        private string buttonName;
        private string boundName;
        private string inputName;
        private float inputAxis;
        private bool isCheckRebind;
        private bool isConflict;
        private bool isKeyShow;
        private int frameCount;

        public bool IsConflict
        {
            get { return isConflict; }
        }

        public string ButtonName
        {
            get { return buttonName; }
        }

        public string BoundName
        {
            get { return boundName; }
        }

        private void Update()
        {
            if (isCheckRebind && Time.frameCount > frameCount && !string.IsNullOrEmpty(input.InputName))
            {
                if (input.InputAxis != 0.0f)
                {
                    inputName = boundName;
                }
                else
                {
                    inputName = input.InputName;
                    inputAxis = input.InputAxis;
                }

                isCheckRebind = false;
                OnListenInput();
            }
        }

        public void Init(RebindControl control, string buttonName)
        {
            this.control = control;
            this.buttonName = buttonName;
            input = Global.GetService<InputManager>();
            buttonNameText.text = input.GetDescription(buttonName);
            SetButtonContent(buttonName);
        }

        public void SetConflict(bool isConflict)
        {
            if (isConflict)
            {
                rebindButton.image.color = Color.red;
            }
            else
            {
                rebindButton.image.color = Color.white;
            }

            this.isConflict = isConflict;
        }

        public void OnRebindButtonClicked()
        {
            if (isCheckRebind)
            {
                return;
            }

            frameCount = Time.frameCount;
            isCheckRebind = true;
            rebindButton.image.color = Color.yellow;
            rebindButton.image.raycastTarget = false;
            resetButton.image.raycastTarget = false;
            keyCodeText.gameObject.SetActive(false);
            joystickIcon.gameObject.SetActive(false);
        }

        public void OnResetButtonClicked()
        {
            isCheckRebind = false;
            inputName = input.GetActiveInputMapping(buttonName);
            inputAxis = 0.0f;
            OnListenInput();
        }

        private void SetText(string name)
        {
            isKeyShow = true;
            keyCodeText.text = name;
            keyCodeText.gameObject.SetActive(true);
            joystickIcon.gameObject.SetActive(false);
        }

        private void SetIcon(Sprite icon, string name)
        {
            if (icon)
            {
                isKeyShow = false;
                joystickIcon.sprite = icon;
                keyCodeText.gameObject.SetActive(false);
                joystickIcon.gameObject.SetActive(true);
            }
            else
            {
                SetText(name);
            }
        }

        public void SetButtonContent(string name)
        {
            boundName = input.GetActiveBoundMapping(buttonName);
            switch (input.InputDevice)
            {
                case InputDevice.MouseKeyboard:
                    SetText(boundName);
                    break;
                case InputDevice.Mobile:
                    EnableElement(false);
                    break;
                case InputDevice.XboxGamepad:
                    SetIcon(control.LoadXboxIcon(boundName), boundName);
                    break;
                case InputDevice.Ps4Gamepad:
                    SetIcon(control.LoadPs4Icon(boundName), boundName);
                    break;
            }
        }

        private void EnableElement(bool isEnable)
        {
            gameObject.SetActive(isEnable);
        }

        private void OnListenInput()
        {
            rebindButton.image.color = Color.white;
            rebindButton.image.raycastTarget = true;
            resetButton.image.raycastTarget = true;
            if (inputName == boundName)
            {
                if (isKeyShow)
                {
                    keyCodeText.gameObject.SetActive(true);
                }
                else
                {
                    joystickIcon.gameObject.SetActive(true);
                }

                return;
            }

            var oldBindName = boundName;
            boundName = inputName;
            input.RebindButton(buttonName, inputName);
            SetButtonContent(buttonName);
            control.OnButtonRebind(oldBindName, inputName);
        }
    }
}