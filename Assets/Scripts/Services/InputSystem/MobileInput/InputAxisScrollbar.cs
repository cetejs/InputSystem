using UD.Globals;
using UnityEngine;
using UnityEngine.UI;

namespace UD.Services.InputSystem
{
    [RequireComponent(typeof(Scrollbar))]
    public class InputAxisScrollbar : MonoBehaviour
    {
        [SerializeField]
        private string axisName = "Horizontal";

        private Scrollbar scrollbar;

        private void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
            scrollbar.value = (Global.GetService<InputManager>().GetAxis(axisName) + 1.0f) / 2.0f;
            scrollbar.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            Global.GetService<InputManager>().SetAxis(axisName, value * 2.0f - 1.0f);
        }
    }
}