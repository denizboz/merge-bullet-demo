using Events;
using Events.Implementations;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_levelNumberUI;
        [SerializeField] private TextMeshProUGUI m_currencyUI;


        private void Awake()
        {
            GameEventSystem.AddListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.AddListener<CurrencyUpdatedEvent>(OnCurrencyUpdated);
        }

        private void OnLevelLoaded(object levelNumber)
        {
            var number = (int)levelNumber + 1;
            m_levelNumberUI.text = $"Lvl {number.ToString()}";
        }

        private void OnCurrencyUpdated(object amount)
        {
            var currency = (int)amount;
            m_currencyUI.text = $"${currency.ToString()}";
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.RemoveListener<CurrencyUpdatedEvent>(OnCurrencyUpdated);
        }
    }
}