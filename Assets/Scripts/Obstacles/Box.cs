using PlayerSpace;
using UnityEngine;

namespace Obstacles
{
    public class Box : Obstacle
    {
        public override void OnBulletEnter(Bullet bullet)
        {
            bullet.GetDamage(health);
            gameObject.SetActive(false);
        }
    }
}