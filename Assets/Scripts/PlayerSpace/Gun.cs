using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Events;
using Events.Implementations;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerSpace
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Transform m_barrel;
        [SerializeField] private Rigidbody m_rigidbody;
        
        private Transform[] m_subBarrels;

        private BulletFactory m_bulletFactory;
        private Tweener m_moveTween;

        private int m_bulletLevel;
        private int m_bulletSize;
        
        private int m_fireBurst;
        private float m_firePeriod;
        private float m_burstAngleCover;

        private float m_timer;
        
        private bool m_isFiring;


        private void Awake()
        {
            m_bulletFactory = DI.Resolve<BulletFactory>();
            
            var gameManager = DI.Resolve<GameManager>();
            var parameters = gameManager.GameParameters;

            m_burstAngleCover = parameters.BurstAngleCover;
            
            CreateSubBarrels(parameters.MaxGunBurst);
            SetFireRate(parameters.BaseFireRate);

            SetFireBurst(1);
            
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.AddListener<LevelFailedEvent>(OnLevelFinished);
        }
        

        private void Update()
        {
            if (!m_isFiring)
                return;
            
            if (!Input.GetMouseButton(0))
                return;

            m_timer += Time.deltaTime;
            
            if (m_timer > m_firePeriod)
            {
                m_timer = 0f;
                Fire();
            }
        }

        private void OnLevelFinished(object none)
        {
            EnableFiring(false);
        }

        public void EnableFiring(bool value)
        {
            m_isFiring = value;
        }
        
        private void Fire()
        {
            for (var i = 0; i < m_fireBurst; i++)
            {
                var bullet = m_bulletFactory.Get(m_bulletLevel, m_bulletSize);
                
                bullet.SetPosition(m_barrel.position);
                bullet.SetDirection(m_subBarrels[i].forward);
            }
        }
        
        public void SetFireRate(float fireRate)
        {
            m_firePeriod = 1f / fireRate;
        }
        
        public void SetFireBurst(int burst)
        {
            m_fireBurst = burst;
            SetSubBarrels();
        }
        
        public void SetBulletSize(int size) => m_bulletSize = size;
        
        private void SetSubBarrels()
        {
            if (m_fireBurst == 1)
            {
                m_subBarrels[0].localRotation = Quaternion.identity;
                return;
            }

            var sliceCount = m_fireBurst - 1;
            var sliceAngle = m_burstAngleCover / sliceCount;
            
            for (var i = 0; i < m_fireBurst; i++)
            {
                var subBarrel = m_subBarrels[i];
                
                subBarrel.localRotation = quaternion.identity;
                subBarrel.Rotate(m_barrel.up, -m_burstAngleCover / 2f + i * sliceAngle);
            }
        }
        
        private void CreateSubBarrels(int count)
        {
            m_subBarrels = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                var barrel = new GameObject($"SubBarrel_{i.ToString()}")
                {
                    transform =
                    {
                        parent = m_barrel,
                        localPosition = Vector3.zero,
                        localRotation = Quaternion.identity
                    }
                };

                m_subBarrels[i] = barrel.transform;
            }
        }

        public void DoLocalMove(Vector3 localPos, float duration)
        {
            m_moveTween?.Kill();
            m_moveTween = transform.DOLocalMove(localPos, duration).SetEase(Ease.InOutQuad);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponentInParent(out Bullet bullet))
            {
                m_bulletLevel = bullet.Level;
                
                var gunPlacer = DI.Resolve<GunPlacer>();
                gunPlacer.TakeGun(this);
                
                GameEventSystem.Invoke<BulletEnteredGunEvent>(bullet);
            }
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFinished);
        }
    }
}