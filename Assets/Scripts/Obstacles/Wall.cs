﻿using PlayerSpace;

namespace Obstacles
{
    public class Wall : Obstacle
    {
        public override void OnBulletEnter(Bullet bullet)
        {
            bullet.GetDamage(health);
            gameObject.SetActive(false);
        }
    }
}