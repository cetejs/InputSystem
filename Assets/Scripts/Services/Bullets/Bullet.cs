using System;
using System.Collections;
using UD.Globals;
using UnityEngine;

namespace UD.Services.Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime = 1.0f;
        private Rigidbody rd;

        private void Awake()
        {
            rd = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            StartCoroutine(AutoRelease());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator AutoRelease()
        {
            if (lifeTime <= 0)
            {
                yield break;
            }

            yield return new WaitForSeconds(lifeTime);
            rd.velocity = Vector3.zero;
            Global.GetService<BulletManager>().ReleaseBullet(this);
        }

        public void AddForce(Vector3 force)
        {
            rd.AddForce(force);
        }
    }
}