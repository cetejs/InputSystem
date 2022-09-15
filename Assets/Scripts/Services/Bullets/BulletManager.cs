using System.Collections.Generic;
using UD.Generic;
using UnityEngine;

namespace UD.Services.Bullets
{
    public class BulletManager : Service
    {
        private Stack<Bullet> bullets = new Stack<Bullet>();
        private Bullet bulletPrefab;

        private void Awake()
        {
            bulletPrefab = Resources.Load<GameObject>("Bullet").GetComponent<Bullet>();
        }

        public Bullet GetBullet()
        {
            Bullet bullet;
            if (bullets.Count > 0)
            {
                bullet = bullets.Pop();
                bullet.gameObject.SetActive(true);
            }
            else
            {
                bullet = Instantiate(bulletPrefab);
            }

            bullet.transform.SetParent(null);
            return bullet;
        }

        public void ReleaseBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            bullet.transform.SetParent(transform);
            bullets.Push(bullet);
        }
    }
}