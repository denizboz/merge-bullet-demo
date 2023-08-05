using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Managers
{
    public class CurrencyManager : MonoBehaviour, IDependency
    {
        private int m_currentAmount;

        private const string keyForCurrency = "currency";
        
        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyForCurrency))
                SetFirstGameAmount();

            GameEventSystem.AddListener<CurrencyEarnedOrLostEvent>(OnCurrencyEarnedOrLost);
        }

        private void Start()
        {
            m_currentAmount = PlayerPrefs.GetInt(keyForCurrency);
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
        }

        private void OnCurrencyEarnedOrLost(object change)
        {
            m_currentAmount += (int)change;
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
        }

        public bool TrySpend(int amount)
        {
            if (m_currentAmount < amount)
                return false;

            m_currentAmount -= amount;
            GameEventSystem.Invoke<CurrencyUpdatedEvent>(m_currentAmount);
            
            PlayerPrefs.SetInt(keyForCurrency, m_currentAmount);
            
            return true;
        }

        private void SetFirstGameAmount()
        {
            var gameManager = DI.Resolve<GameManager>();
            var firstGameAmount = gameManager.GameParameters.InitialCurrency;
            
            PlayerPrefs.SetInt(keyForCurrency, firstGameAmount);
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<CurrencyEarnedOrLostEvent>(OnCurrencyEarnedOrLost);
        }
    }
}