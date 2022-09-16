using UD.Globals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UD.Services.InputSystem
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private AxisOption axisType = AxisOption.Both;
        [SerializeField]
        private string horizontalAxisName = "Mouse X";
        [SerializeField]
        private string verticalAxisName = "Mouse Y";
        [SerializeField]
        private float responseSpeed = 1f;

        private bool isUseHorizontal;
        private bool isUseVertical;
        private Vector2 previousPos;

        private void UpdateAxis(Vector2 value)
        {
            isUseHorizontal = axisType == AxisOption.Both || axisType == AxisOption.OnlyHorizontal;
            isUseVertical = axisType == AxisOption.Both || axisType == AxisOption.OnlyVertical;

            var delta = value.normalized;
            if (isUseHorizontal)
            {
                Global.GetService<InputManager>().SetAxis(horizontalAxisName, delta.x * responseSpeed);
            }

            if (isUseVertical)
            {
                Global.GetService<InputManager>().SetAxis(verticalAxisName, delta.y * responseSpeed);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            var delta = eventData.position - previousPos;
            previousPos = eventData.position;
            UpdateAxis(delta);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            previousPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateAxis(Vector2.zero);
        }

        private enum AxisOption : byte
        {
            Both,
            OnlyHorizontal,
            OnlyVertical
        }
    }
}