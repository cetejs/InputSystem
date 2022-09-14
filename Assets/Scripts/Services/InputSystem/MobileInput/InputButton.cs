using UD.Globals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UD.Services.InputSystem
{
    public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string buttonName = "Button";

        public void OnPointerDown(PointerEventData eventData)
        {
            Global.GetService<InputManager>().SetButtonDown(buttonName);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Global.GetService<InputManager>().SetButtonUp(buttonName);
        }
    }
}