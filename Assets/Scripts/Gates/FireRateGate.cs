using CommonTools.Runtime;
using Events;
using Events.Implementations;
using PlayerSpace;
using UnityEngine;

namespace Gates
{
    public class FireRateGate : Gate
    {
        [SerializeField] private MeshRenderer m_renderer;
        
        [SerializeField] private Material m_positiveMat;
        [SerializeField] private Material m_negativeMat;
        

        public override void OnPlayerEnter(Player player)
        {
            player.SetFireRate(effectPower);
            gameObject.SetActive(false);
        }
        
        public override void OnBulletEnter(Bullet bullet)
        {
            UpdateRate(bullet.Damage);
            GameEventSystem.Invoke<BulletDestroyedEvent>(bullet);
        }

        private void UpdateRate(int change)
        {
            if (effectPower.ChangesSignOnUpdate(change))
                UpdateColor();
            
            effectPower += change;
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            var stringBuilder = StringBuilderPool.Get();
            
            if (effectPower > -1) 
                stringBuilder.Append(positiveSign);
            
            stringBuilder.Append(effectPower.ToString());
            
            var effectString = StringBuilderPool.GetStringAndReturn(stringBuilder);
            effectUI.text = effectString;
        }

        private void UpdateColor()
        {
            m_renderer.sharedMaterial = effectPower > -1 ? m_positiveMat : m_negativeMat;
        }

#if UNITY_EDITOR
        protected new void OnValidate()
        {
            base.OnValidate();
            UpdateColor();
        }
#endif
    }
}