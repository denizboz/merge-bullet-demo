﻿using System.Collections.Generic;
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

        private Player m_player;
        
        private float m_bulletSpeed;
        private bool m_isOn;

        private float m_timer;
        private const float bulletRangeCheckPeriod = 1f;
        
        
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
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelFinished);
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

            m_timer += Time.deltaTime;

            if (m_timer > bulletRangeCheckPeriod)
            {
                m_timer = 0f;
                CheckForOutOfRangeBullets();
            }
        }

        private void CheckForOutOfRangeBullets()
        {
            var activeBullets = m_activeBullets.ToArray();
            
            foreach (var bullet in activeBullets)
            {
                if (m_player.IsBulletOutOfRange(bullet.Position))
                    GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
            }
        }
        
        private void OnBulletActivated(object bullet)
        {
            if (!m_isOn)
                return;
            
            m_activeBullets.Add((Bullet)bullet);
        }

        private void OnBulletDestroyed(object bullet)
        {
            if (!m_isOn)
                return;
            
            m_activeBullets.Remove((Bullet)bullet);
        }

        private void OnLevelStarted(object none)
        {
            m_player = DI.Resolve<Player>();
            m_isOn = true;
        }

        private void OnLevelFinished(object none)
        {
            DestroyActiveBullets();
            m_isOn = false;
        }

        private void DestroyActiveBullets()
        {
            var cloneList = new List<Bullet>(m_activeBullets);
            
            foreach (var bullet in cloneList)
            {
                GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
                // this will automatically call OnBulletDestroyed through event channel
            }
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<BulletActivatedEvent>(OnBulletActivated);
            GameEventSystem.RemoveListener<BulletDestroyedEvent>(OnBulletDestroyed);
            
            GameEventSystem.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFinished);
        }
    }
}