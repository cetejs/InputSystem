using UD.Globals;
using UD.Services.InputSystem;
using UnityEngine;

namespace UD.Cameras
{
    public class FreeLookCamera : FollowTargetCamera
    {
        [SerializeField]
        private float moveSpeed = 5.0f;
        [SerializeField]
        private float turnSpeed = 3.0f;
        [SerializeField]
        private float turnSmoothing = 15.0f;
        [SerializeField]
        private float minTiltAngle = -45.0f;
        [SerializeField]
        private float maxTiltAngle = 75.0f;
        [SerializeField]
        private float joystickAxisSpeed = 0.1f;
        [SerializeField]
        private bool isLockScreen;

        private Transform pivot;
        private float lookAngle;
        private float tiltAngle;
        private Vector3 pivotEuler;
        private Quaternion lookTargetRot;
        private Quaternion tiltTargetRot;

        protected override void Awake()
        {
            base.Awake();
            pivot = cam.transform.parent;
            pivotEuler = pivot.eulerAngles;
            lookTargetRot = transform.localRotation;
            tiltTargetRot = pivot.localRotation;
        }

        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                isLockScreen = !isLockScreen;
            }

            HandleRotation();
        }

        protected override void FollowTarget()
        {
            if (moveSpeed > 0)
            {
                transform.position = Vector3.Lerp(transform.position, Target.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Target.position;
            }
        }

        public void ResetRotation()
        {
            lookAngle = target.eulerAngles.y;
        }

        private void HandleRotation()
        {
            if (!isLockScreen)
            {
                var input = Global.GetService<InputManager>();
                var axisSpeed = input.InputDevice == InputDevice.MouseKeyboard ? 1.0f : joystickAxisSpeed;
                var x = Global.GetService<InputManager>().GetAxis("Mouse X") * axisSpeed;
                var y = Global.GetService<InputManager>().GetAxis("Mouse Y") * axisSpeed;
                lookAngle += x * turnSpeed;
                tiltAngle -= y * turnSpeed;
            }

            tiltAngle = Mathf.Clamp(tiltAngle, minTiltAngle, maxTiltAngle);

            lookTargetRot = Quaternion.Euler(0, lookAngle, 0);
            tiltTargetRot = Quaternion.Euler(tiltAngle, pivotEuler.y, pivotEuler.z);

            if (turnSmoothing > 0)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, lookTargetRot, turnSmoothing * Time.deltaTime);
                pivot.localRotation = Quaternion.Slerp(pivot.localRotation, tiltTargetRot, turnSmoothing * Time.deltaTime);
            }
            else
            {
                transform.localRotation = lookTargetRot;
                pivot.localRotation = tiltTargetRot;
            }
        }
    }
}