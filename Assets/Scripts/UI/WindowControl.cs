using UD.Cameras;
using UD.Controllers;
using UD.Globals;
using UD.Services.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UD.UI
{
    public class WindowControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject rebindWindow;
        [SerializeField]
        private SimpleController simpleController;
        [SerializeField]
        private FreeLookCamera freeLookCamera;
        
        public void EnableRebindWindow()
        {
            if (rebindWindow.activeSelf)
            {
                rebindWindow.SetActive(false);
                simpleController.enabled = true;
                freeLookCamera.enabled = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                rebindWindow.SetActive(true); 
                simpleController.enabled = false;
                freeLookCamera.enabled = false;
                Global.GetService<InputManager>().SelectDefaultGo();
            }
        }

        private void Update()
        {
            var input = Global.GetService<InputManager>();
            if (input.GetButtonDown("Button7"))
            {
                EnableRebindWindow();
            }
        }
    }
}

