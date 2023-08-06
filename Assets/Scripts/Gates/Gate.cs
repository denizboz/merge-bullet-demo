using Interfaces;
using PlayerSpace;
using TMPro;
using UnityEngine;

namespace Gates
{
    public abstract class Gate : MonoBehaviour, IInteractable
    {
        [SerializeField] protected int effectPower;
        [SerializeField] protected TextMeshPro effectUI;
        
        protected const char positiveSign = '+';
        
        public virtual void OnBulletEnter(Bullet bullet) { }
        
        public virtual void OnPlayerEnter(Player player) { }

        protected virtual void UpdateUI() { }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateUI();
        }
#endif
    }
}