using System;
using Events;
using Events.Implementations;
using TMPro;
using UnityEngine;
using Utility;

namespace Managers
{
    public enum UIPanel { Start, Level, Success, Fail }
    
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_panels;
        [SerializeField] private GameObject m_currencyPanel;
        
        [SerializeField] private TextMeshProUGUI m_levelNumberUI;
        [SerializeField] private TextMeshProUGUI m_currencyUI;


        private void Awake()
        {
            GameEventSystem.AddListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.AddListener<PlayButtonPressedEvent>(OnPlayButtonPressed);
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelSuccess);
            GameEventSystem.AddListener<LevelFailedEvent>(OnLevelFail);
            
            GameEventSystem.AddListener<CurrencyUpdatedEvent>(OnCurrencyUpdated);
        }

        private void OnLevelLoaded(object levelNumber)
        {
            var number = (int)levelNumber + 1;
            m_levelNumberUI.text = $"Lvl {number.ToString()}";
            
            SetPanel(UIPanel.Start);
        }

        private void OnPlayButtonPressed(object none)
        {
            SetPanel(UIPanel.Level);
        }

        private void OnLevelSuccess(object none)
        {
            SetPanel(UIPanel.Success);
        }

        private void OnLevelFail(object none)
        {
            SetPanel(UIPanel.Fail);
        }
        
        private void OnCurrencyUpdated(object amount)
        {
            var currency = (int)amount;
            m_currencyUI.text = $"${currency.ToString()}";
        }

        private void SetPanel(UIPanel panel)
        {
            for (var i = 0; i < m_panels.Length; i++)
            {
                m_panels[i].SetActive(i == (int)panel);
            }

            var showCurrency = panel == UIPanel.Start || panel == UIPanel.Level;
            m_currencyPanel.SetActive(showCurrency);
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.RemoveListener<PlayButtonPressedEvent>(OnPlayButtonPressed);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelSuccess);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFail);
            
            GameEventSystem.RemoveListener<CurrencyUpdatedEvent>(OnCurrencyUpdated);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var uiPanelVariety = Enum.GetValues(typeof(UIPanel)).Length;
            
            if (m_panels.Length != uiPanelVariety)
                Debug.LogWarning("Number of panels must be equal to UIPanel variety size.");
        }
#endif
    }
}