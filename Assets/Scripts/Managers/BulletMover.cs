using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using PlayerSpace;
using UnityEngine;

namespace Managers
{
    public class BulletMover : MonoBehaviour, IDependency
    {
        private readonly List<Bullet> m_activeBullets = new List<Bullet>();

        private float m_bulletSpeed;
        private bool m_isOn;

        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            var gameManager = DI.Resolve<GameManager>();
            m_bulletSpeed = gameManager.GameParameters.BulletSpeed;
            
            GameEventSystem.AddListener<BulletActivatedEvent>(OnBulletActivated);
            GameEventSystem.AddListener<BulletDestroyedEvent>(OnBulletDestroyed);
            
            GameEventSystem.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelStarted);
            GameEventSystem.AddListener<LevelFailedEvent>(OnLevelFinished);
        }

        private void Update()
        {
            if (!m_isOn)
                return;
            
            var distance = m_bulletSpeed * Time.deltaTime;
            
            foreach (var bullet in m_activeBullets)
            {
                bullet.MoveForward(distance);
            }
        }
        
        private void OnBulletActivated(object bullet)
        {
            m_activeBullets.Add((Bullet)bullet);
        }

        private void OnBulletDestroyed(object bullet)
        {
            m_activeBullets.Remove((Bullet)bullet);
        }

        private void OnLevelStarted(object none)
        {
            m_isOn = true;
        }

        private void OnLevelFinished(object none)
        {
            m_isOn = false;
            DestroyActiveBullets();
        }

        private void DestroyActiveBullets()
        {
            var cloneList = new List<Bullet>(m_activeBullets);
            
            foreach (var bullet in cloneList)
            {
                GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
            }
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<BulletActivatedEvent>(OnBulletActivated);
            GameEventSystem.RemoveListener<BulletDestroyedEvent>(OnBulletDestroyed);
            
            GameEventSystem.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelStarted);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFinished);
        }
    }
}