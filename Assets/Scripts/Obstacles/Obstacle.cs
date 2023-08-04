using Gates;
using Interfaces;
using PlayerSpace;
using UnityEngine;

namespace Obstacles
{
    public abstract class Obstacle : MonoBehaviour, IInteractable
    {
        public virtual void OnBulletEnter(Bullet bullet) { }
        
        public virtual void OnPlayerEnter(Player player) { }
    }
}