using UD.Globals;
using UD.Services.Bullets;
using UD.Services.InputSystem;
using UnityEngine;

namespace UD.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 1.0f;
        [SerializeField]
        private float turnSpeed = 30.0f;
        [SerializeField]
        private float jumpSpeed = 3.0f;
        [SerializeField]
        private float jumpTime = 0.3f;
        [SerializeField]
        private float gravity = 9.81f;
        [SerializeField]
        private float springMulti = 1.5f;
        [SerializeField]
        private float bulletForce = 1f;
        [SerializeField]
        private float shootInterval = 1f;
        [SerializeField]
        private Transform muzzle;
        private CharacterController cc;
        private Transform cam;
        private Vector3 camForward;
        private Vector3 freeMoveInput;

        private float jumpCounter;
        private float shootCounter;
        private bool isJumpDown;
        private bool isSpringing;
        private bool isShooting;

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            cam = Camera.main.transform;
        }

        private void Update()
        {
            UpdateInput();
            UpdateMovement();
            UpdateRotation();
            UpdateShoot();
        }

        private void UpdateInput()
        {
            var input = Global.GetService<InputManager>();
            var h = input.GetAxis("Horizontal");
            var v = input.GetAxis("Vertical");

            if (cam != null)
            {
                camForward = cam.forward;
                camForward.y = 0.0f;
                var forward = camForward.normalized;
                freeMoveInput = forward * v + cam.right * h;
            }
            else
            {
                freeMoveInput = Vector3.forward * v + Vector3.right * h;
            }

            if (input.GetButtonDown("Button0"))
            {
                isJumpDown = true;
            }

            if (input.GetButtonDown("Button1"))
            {
                isSpringing = true;
            }
            else if (input.GetButtonUp("Button1"))
            {
                isSpringing = false;
            }

            if (input.GetButtonDown("LeftStickClick"))
            {
                isSpringing = !isSpringing;
            }

            if (input.GetButtonDown("Button2"))
            {
                isShooting = true;
                shootCounter = 0;
            }
            else if (input.GetButtonUp("Button2"))
            {
                isShooting = false;
            }
        }

        private void UpdateMovement()
        {
            var moveInput = freeMoveInput;
            if (moveInput.sqrMagnitude > 1)
            {
                moveInput.Normalize();
            }

            if (isJumpDown && cc.isGrounded)
            {
                jumpCounter = jumpTime;
            }

            isJumpDown = false;

            float verticalSpeed = 0.0f;
            if (jumpCounter > 0)
            {
                jumpCounter -= Time.deltaTime;
                verticalSpeed = jumpSpeed;
            }
            else if (cc.isGrounded)
            {
                verticalSpeed = -gravity;
            }
            else
            {
                verticalSpeed -= gravity;
            }

            var forwardSpeed = isSpringing ? moveSpeed * springMulti : moveSpeed;
            cc.Move((moveInput * forwardSpeed + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        private void UpdateRotation()
        {
            var moveInput = freeMoveInput;
            if (moveInput.sqrMagnitude > 0)
            {
                var targetEuler = transform.eulerAngles;
                var freeRot = Quaternion.LookRotation(moveInput, transform.up);
                targetEuler.y = freeRot.eulerAngles.y;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetEuler), turnSpeed * Time.deltaTime);
            }
        }

        private void UpdateShoot()
        {
            if (isShooting)
            {
                if (shootCounter > 0)
                {
                    shootCounter -= Time.deltaTime;
                    return;
                }

                shootCounter = shootInterval;
                var bullet = Global.GetService<BulletManager>().GetBullet();
                bullet.transform.position = muzzle.transform.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.AddForce(transform.forward * bulletForce);
            }
        }
    }
}