using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Merge
{
    public class MergeGrid : MonoBehaviour
    {
        [SerializeField] private Transform m_center;
        [SerializeField] private GameObject m_prefab;
        
        private GridRenderer m_gridRenderer;
        private Vector3[] m_cellCenters;

        private const int gridWidth = 5;
        private const int gridHeight = 3;
        // const parameters might be dynamic in future
        
        
        private void Awake()
        {
            m_gridRenderer = DI.Resolve<GridRenderer>();
            m_cellCenters = m_gridRenderer.RenderGrid(m_center.position, gridWidth, gridHeight);
        }

        private void Start()
        {
            // load saved grid items
        }
    }
    
    public struct GridItemData
    {
        public int Level;
        public int Row;
        public int Column;
    }
}