using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using PlayerSpace;
using UnityEngine;
using Utility;

namespace Managers
{
    public class BulletFactory : MonoBehaviour, IDependency
    {
        [SerializeField] private Bullet m_bulletPrefab;
        [SerializeField] private BulletMaterialsSO m_materialContainer;

        private Material[] m_materials;

        private Queue<Bullet> m_bulletPool;
        private int m_poolSize = 256;

        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            GameEventSystem.AddListener<BulletDestroyedEvent>(OnBulletDestroyed);

            m_materials = m_materialContainer.Materials;
            CreatePool();
        }

        public Bullet Get(int level, int size = 1)
        {
            EnsurePoolCapacity();
            
            var bullet = m_bulletPool.Dequeue();
            
            bullet.SetActive(true);
            bullet.SetLevel(level);
            bullet.SetSize(size);
            bullet.ResetRotation();
            bullet.SetMaterial(m_materials[level - 1]);
            
            // set bullet mesh instead of material here in the fbx case:
            // bullet.SetMesh(m_meshes[level]);

            GameEventSystem.Invoke<BulletActivatedEvent>(bullet);
            
            return bullet;
        }

        private void OnBulletDestroyed(object usedBullet)
        {
            var bullet = (Bullet)usedBullet;
            bullet.SetActive(false);
            m_bulletPool.Enqueue(bullet);
        }
        
        private void CreatePool()
        {
            m_bulletPool = new Queue<Bullet>(m_poolSize);

            for (int i = 0; i < m_poolSize; i++)
            {
                CreateAndAddBullet();
            }
        }

        private void CreateAndAddBullet()
        {
            var bullet = Instantiate(m_bulletPrefab, transform);
            bullet.SetActive(false);
            
            m_bulletPool.Enqueue(bullet);
        }
        
        private void EnsurePoolCapacity()
        {
            if (m_bulletPool.Count > 0)
                return;

            for (int i = 0; i < m_poolSize; i++)
            {
                CreateAndAddBullet();
            }

            m_poolSize *= 2;
            
            // Debug.LogWarning($"Bullet pool capacity increased from {(m_poolSize / 2).ToString()} to {m_poolSize.ToString()}");
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<BulletDestroyedEvent>(OnBulletDestroyed);
        }
    }
}