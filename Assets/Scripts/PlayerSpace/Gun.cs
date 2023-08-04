using CommonTools.Runtime.DependencyInjection;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace PlayerSpace
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private Transform m_barrel;
        private Transform[] m_subBarrels;

        private BulletFactory m_bulletFactory;

        private int m_bulletLevel;
        
        private int m_fireBurst;
        private float m_firePeriod;
        private float m_burstAngleCover;
        
        private float m_timer;
        
        private bool m_isFiring;
        private bool m_isLarger;

        private void Awake()
        {
            m_bulletFactory = DI.Resolve<BulletFactory>();
            var gameManager = DI.Resolve<GameManager>();

            m_burstAngleCover = gameManager.GameParameters.BurstAngleCover;

            CreateSubBarrels(gameManager.GameParameters.MaxGunBurst);
            SetFirePeriod(gameManager.GameParameters.BaseFireRate);

            SetFireBurst(1);
        }

        private void Update()
        {
            if (!m_isFiring)
                return;

            m_timer += Time.deltaTime;

            if (m_timer > m_firePeriod)
            {
                m_timer = 0f;
                Fire();
            }
        }

        private void Fire()
        {
            for (var i = 0; i < m_fireBurst; i++)
            {
                var bullet = m_bulletFactory.Get(m_bulletLevel, m_isLarger);
                
                bullet.SetPosition(m_barrel.position);
                bullet.SetDirection(m_subBarrels[i].forward);
            }
        }
        
        private void SetFirePeriod(float fireRate)
        {
            m_firePeriod = 1f / fireRate;
        }
        
        public void SetFireBurst(int burst)
        {
            m_fireBurst = burst;
            SetSubBarrels();
        }

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
    }
}