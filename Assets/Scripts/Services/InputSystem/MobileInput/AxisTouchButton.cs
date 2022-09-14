﻿using UD.Globals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UD.Services.InputSystem
{
    public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private AxisOption axisType = AxisOption.Negative;
        [SerializeField]
        private string axisName = "Horizontal";
        [SerializeField]
        private float responseSpeed = 1f;

        private bool isPointerDown;
        private float axisValue;

        private void Update()
        {
            if(isPointerDown)
            {
                if(axisType == AxisOption.Negative)
                {
                    UpdateAxis(1);
                }
                else if(axisType == AxisOption.Positive)
                {
                    UpdateAxis(-1);
                }
            }
        }

        private void UpdateAxis(float value)
        {
            if(!Mathf.Approximately(axisValue, value))
            {
                axisValue = Mathf.MoveTowards(axisValue, value, responseSpeed * Time.deltaTime);
                Global.GetService<InputManager>().SetAxis(axisName, axisValue);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            axisValue = 0;
            Global.GetService<InputManager>().SetAxisZero(axisName);
        }

        private enum AxisOption : byte
        {
            Positive,
            Negative
        }
    }
}