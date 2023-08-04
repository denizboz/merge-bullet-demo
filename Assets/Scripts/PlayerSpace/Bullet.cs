using Interfaces;
using UnityEngine;

namespace PlayerSpace
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private MeshRenderer m_renderer;

        private int m_level;
        
        private bool m_isLarge;
        private const int enlargeFactor = 2;

        public int Damage => enlargeFactor * m_level;
        public Vector3 Position => transform.position;
        
        
        public void MoveForward(float distance)
        {
            transform.position += distance * transform.forward;
        }

        public void SetLevel(int level)
        {
            m_level = level;
        }
        
        public void SetSize(bool large)
        {
            if (large == m_isLarge)
                return;

            transform.localScale = (large ? enlargeFactor : 1f) * Vector3.one;
            m_isLarge = large;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
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