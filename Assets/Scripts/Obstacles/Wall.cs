using TMPro;
using UnityEngine;

namespace Obstacles
{
    public class Wall : Obstacle
    {
        [SerializeField] private TextMeshPro m_healthUI;
        
        protected override void UpdateUI()
        {
            m_healthUI.text = health.ToString();
        }
    }
}