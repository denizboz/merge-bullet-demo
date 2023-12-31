﻿using Events;
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
        [SerializeField] private TextMeshPro m_healthUI;

        public virtual void OnBulletEnter(Bullet bullet)
        {
            GetDamage(bullet.Damage);
            GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
        }

        public virtual void OnPlayerEnter(Player player)
        {
            GameEventSystem.Invoke<LevelFailedEvent>();
        }

        private void GetDamage(int damage)
        {
            health -= damage;
            UpdateUI();

            if (health < 1)
                gameObject.SetActive(false);
        }
        
        protected virtual void UpdateUI()
        {
            if (!m_healthUI)
                return;

            m_healthUI.text = health.ToString();
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            UpdateUI();
        }
#endif
    }
}