using Events;
using Events.Implementations;
using Interfaces;
using PlayerSpace;
using TMPro;
using UnityEngine;

namespace Obstacles
{
    public abstract class Obstacle : MonoBehaviour, IInteractable
    {
        [SerializeField] protected int health;
        [SerializeField] private TextMeshPro healthUI;

        public void OnBulletEnter(Bullet bullet)
        {
            GetDamage(bullet.Damage);
            GameEventSystem.Invoke<BulletDestroyedEvent>();
        }

        public void OnPlayerEnter(Player player)
        {
            GameEventSystem.Invoke<LevelFailedEvent>();
        }

        private void GetDamage(int damage)
        {
            health -= damage;

            if (health < 1)
                gameObject.SetActive(false);
        }
        
        protected virtual void UpdateUI()
        {
            if (!healthUI)
                Debug.LogWarning("Assign 'healthUI' for auto-fill OnValidate.");
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateUI();
        }
#endif
    }
}