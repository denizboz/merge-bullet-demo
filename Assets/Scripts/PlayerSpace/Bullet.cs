using DG.Tweening;
using Events;
using Events.Implementations;
using Interfaces;
using UnityEngine;
using Utility;

namespace PlayerSpace
{
    public class Bullet : MonoBehaviour
    {
        public int Level { get; private set; }
        
        [SerializeField] private MeshRenderer m_renderer;
        
        private int m_size;
        private int m_preLevelHealth;

        public int Damage => m_size * Level;
        public Vector3 Position => transform.position;
        
        
        public void MoveForward(float distance)
        {
            transform.position += distance * transform.forward;
        }

        public void SetLevel(int level) => Level = level;

        public void SetSize(int size)
        {
            if (size == 1)
                return;

            m_size = size;
            transform.localScale = m_size * Vector3.one;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void TweenPosition(Vector3 pos, float duration, Ease ease = Ease.InOutQuad)
        {
            transform.DOMove(pos, duration).SetEase(ease);
        }
        
        public void SetDirection(Vector3 direction)
        {
            transform.forward = direction;
        }
        
        public void SetMaterial(Material mat)
        {
            m_renderer.sharedMaterial = mat;
        }
        
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void GetDamage(int damage)
        {
            m_preLevelHealth -= damage;
            
            if (m_preLevelHealth < 1)
                GameEventSystem.Invoke<BulletDestroyedEvent>(this);
        }
        
        public void ResetPreLevelHealth()
        {
            m_preLevelHealth = Level * 100;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IInteractable interactable))
                interactable.OnBulletEnter(this);
        }
        
        #region FBX Scenario
        // [SerializeField] private MeshFilter m_meshFilter;
        //
        // public void SetMesh(Mesh mesh)
        // {
        //     m_meshFilter.mesh = mesh;
        // }
        #endregion
    }
}