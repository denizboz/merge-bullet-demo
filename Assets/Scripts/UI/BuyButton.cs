using Events;
using Events.Implementations;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuyButton : MonoBehaviour
    {
        private Button m_button;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(Buy);
        }

        private void Buy()
        {
            GameEventSystem.Invoke<BuyButtonPressedEvent>(1);
        }

        private void OnDestroy()
        {
            m_button.onClick.RemoveListener(Buy);
        }
    }
}