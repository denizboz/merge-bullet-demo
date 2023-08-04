using Interfaces;
using PlayerSpace;
using UnityEngine;

namespace Gates
{
    public abstract class Gate : MonoBehaviour, IInteractable
    {
        public virtual void OnBulletEnter(Bullet bullet) { }
        
        public virtual void OnPlayerEnter(Player player) { }
    }
}