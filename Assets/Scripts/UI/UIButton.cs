using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class UIButton : MonoBehaviour
    {
        [SerializeField] private Button m_button;

        protected abstract void OnPressed();
        
        private void Awake()
        {
            m_button.onClick.AddListener(OnPressed);
        }
        
        private void OnDestroy()
        {
            m_button.onClick.RemoveListener(OnPressed);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_button = GetComponent<Button>();
        }
#endif
    }
}