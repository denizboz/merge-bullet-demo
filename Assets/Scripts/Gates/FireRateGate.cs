using CommonTools.Runtime;
using Events;
using Events.Implementations;
using PlayerSpace;
using TMPro;
using UnityEngine;

namespace Gates
{
    public class FireRateGate : Gate
    {
        [SerializeField] private int m_fireRate;

        [SerializeField] private MeshRenderer m_renderer;
        [SerializeField] private TextMeshPro m_fireRateUI;
        
        [SerializeField] private Material m_positiveMat;
        [SerializeField] private Material m_negativeMat;
        
        public override void OnPlayerEnter(Player player)
        {
            player.SetFireRate(m_fireRate);
            gameObject.SetActive(false);
        }
        
        public override void OnBulletEnter(Bullet bullet)
        {
            UpdateRate(bullet.Damage);
            GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
        }

        private void UpdateRate(int change)
        {
            if (m_fireRate.ChangesSignOnUpdate(change))
                UpdateColor();
            
            m_fireRate += change;
            UpdateText();
        }

        private void UpdateText()
        {
            m_fireRateUI.text = m_fireRate.ToString();
        }

        private void UpdateColor()
        {
            m_renderer.sharedMaterial = m_fireRate > 0 ? m_positiveMat : m_negativeMat;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateText();
            UpdateColor();
        }
#endif
    }
}