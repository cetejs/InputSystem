using UnityEngine;

namespace UD.Cameras
{
    public class CursorControlRig : MonoBehaviour
    {
        [SerializeField]
        private bool isLockCursor;

        private void OnEnable()
        {
            Cursor.lockState = isLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLockCursor;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            if (isLockCursor && Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}