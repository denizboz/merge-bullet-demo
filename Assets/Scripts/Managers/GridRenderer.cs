using UnityEngine;

namespace Managers
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private int m_width = 2;
        [SerializeField] private int m_height = 2;

        [SerializeField] private float m_cellSize = 1f;
        [SerializeField] private float m_lineWidth = 0.1f;
        
        [SerializeField] private Color m_lineColor = Color.white;

        private readonly LineRenderer[] m_lineRenderers = new LineRenderer[2 * (maxSize + 1)];

        private const int maxSize = 4;

        private void Awake()
        {
            CreateRenderers();
            RenderGrid(m_width, m_height);
        }

        private void RenderGrid(int width, int height)
        {
            width = Mathf.Clamp(width, 0, maxSize);
            height = Mathf.Clamp(height, 0, maxSize);

            var actualWidth = width * m_cellSize;
            var actualHeight = height * m_cellSize;

            var verticalCount = width + 1;
            var horizontalCount = height + 1;

            var lowerLeft = (actualWidth / 2f) * Vector3.left + (actualHeight / 2f) * Vector3.back;
            var upperRight = (actualWidth / 2f) * Vector3.right + (actualHeight / 2f) * Vector3.forward;

            var iterator = 0;
            
            for (int i = 0; i < verticalCount; i++)
            {
                var lineRenderer = m_lineRenderers[iterator];
                
                var startPos = lowerLeft + i * Vector3.right;
                var endPos = startPos + actualHeight * Vector3.forward;
                
                lineRenderer.gameObject.SetActive(true);
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                iterator++;
            }

            for (int i = 0; i < horizontalCount; i++)
            {
                var lineRenderer = m_lineRenderers[iterator];
                
                var startPos = lowerLeft + i * Vector3.forward;
                var endPos = startPos + actualWidth * Vector3.right;
                
                lineRenderer.gameObject.SetActive(true);
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                iterator++;
            }
        }
        
        private void CreateRenderers()
        {
            for (var i = 0; i < m_lineRenderers.Length; i++)
            {
                var lrObject = new GameObject($"LR_{i.ToString()}");
                lrObject.transform.SetParent(transform);

                var lineRenderer = lrObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = m_lineWidth;
                lineRenderer.endWidth = m_lineWidth;
                lineRenderer.startColor = m_lineColor;
                lineRenderer.endColor = m_lineColor;

                m_lineRenderers[i] = lineRenderer;
                lrObject.SetActive(false);
            }
        }
    }
}