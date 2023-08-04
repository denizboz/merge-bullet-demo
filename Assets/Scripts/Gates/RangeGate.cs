using PlayerSpace;
using TMPro;
using UnityEngine;

namespace Gates
{
    public class RangeGate : Gate
    {
        [SerializeField] private int m_range;
        [SerializeField] private TextMeshPro m_rangeUI;

        [SerializeField] private Material m_positiveMat;
        [SerializeField] private Material m_negativeMat;
        
        
        public override void OnPlayerEnter(Player player)
        {
            player.SetRange(m_range);
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_rangeUI.text = m_range.ToString();
            
            if (TryGetComponent(out MeshRenderer mr))
                mr.sharedMaterial = m_range >= 0 ? m_positiveMat : m_negativeMat;
        }
#endif
    }
}