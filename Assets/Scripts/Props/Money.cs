using CommonTools.Runtime;
using PlayerSpace;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Props
{
    public class Money : MonoBehaviour
    {
        private int m_amount;

        private void Awake()
        {
            m_amount = GetComponentsInChildren<Bill>().Length;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponentInParent(out Player _))
            {
                GameEventSystem.Invoke<CurrencyEarnedEvent>(m_amount);
                gameObject.SetActive(false);
            }
        }
    }
}