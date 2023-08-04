using CommonTools.Runtime;
using UnityEngine;

namespace Props
{
    public class Platform : MonoBehaviour
    {
        [SerializeField, Range(10f, 500f)]
        private float m_length;
        
        [SerializeField, Range(5f, 15f)]
        private float m_width;

        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var ground = transform.Find("ground");
            
            ground.localScale = ground.localScale.WithX(m_width);
            transform.localScale = transform.localScale.WithZ(m_length);
        }
#endif
    }
}