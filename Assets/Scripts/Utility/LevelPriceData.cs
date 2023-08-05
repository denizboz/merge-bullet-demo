using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utility
{
    [CreateAssetMenu(fileName = "LevelPriceData", menuName = "Level-Price Data")]
    public class LevelPriceData : ScriptableObject
    {
        [SerializeField] private LevelPrice[] m_prices;

        public int GetPriceOf(int level) => m_prices[level - 1].Price;

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = 0; i < m_prices.Length; i++)
            {
                m_prices[i].Level = i + 1;
            }
            
            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct LevelPrice
    {
        public int Level;
        public int Price;
    }
}