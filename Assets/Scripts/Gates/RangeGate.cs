using PlayerSpace;
using UnityEngine;

namespace Gates
{
    public class RangeGate : Gate
    {
        [SerializeField] private Material m_positiveMat;
        [SerializeField] private Material m_negativeMat;
        
        
        public override void OnPlayerEnter(Player player)
        {
            player.SetRange(effectPower);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private new void OnValidate()
        {
            base.OnValidate();
            
            if (TryGetComponent(out MeshRenderer mr))
                mr.sharedMaterial = effectPower >= 0 ? m_positiveMat : m_negativeMat;
        }
#endif
    }
}