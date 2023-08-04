using CommonTools.Runtime.DependencyInjection;
using UnityEngine;
using UnityEngine.Rendering;
using Utility;

namespace Merge
{
    public class GridRenderer : MonoBehaviour, IDependency
    {
        [SerializeField] private GridParametersSO m_gridParameters;
        
        private float m_cellSize;
        private float m_lineWidth;
        private Material m_lineMat;

        private LineRenderer[] m_lineRenderers;


        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_cellSize = m_gridParameters.CellSize;
            m_lineWidth = m_gridParameters.LineWidth;
            m_lineMat = m_gridParameters.LineMaterial;
        }

        /// <summary>
        /// Renders grid and returns cell center positions as a Vector3 array.
        /// </summary>
        public Vector3[] RenderGrid(Vector3 center, int width, int height)
        {
            CreateRenderers(width + height + 2);
            
            var actualWidth = width * m_cellSize;
            var actualHeight = height * m_cellSize;

            var verticalCount = width + 1;
            var horizontalCount = height + 1;

            var lowerLeft = center + (actualWidth / 2f) * Vector3.left + (actualHeight / 2f) * Vector3.back;

            var iterator = 0;
            
            for (int i = 0; i < verticalCount; i++)
            {
                var lineRenderer = m_lineRenderers[iterator];
                
                var startPos = lowerLeft + i * m_cellSize * Vector3.right;
                var endPos = startPos + actualHeight * Vector3.forward;
                
                lineRenderer.gameObject.SetActive(true);
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                iterator++;
            }

            for (int i = 0; i < horizontalCount; i++)
            {
                var lineRenderer = m_lineRenderers[iterator];
                
                var startPos = lowerLeft + i * m_cellSize * Vector3.forward;
                var endPos = startPos + actualWidth * Vector3.right;
                
                lineRenderer.gameObject.SetActive(true);
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);

                iterator++;
            }

            return GetCellCenters(lowerLeft, width, height);
        }

        private Vector3[] GetCellCenters(Vector3 lowerLeft, int width, int height)
        {
            lowerLeft += (m_cellSize / 2f * Vector3.right + m_cellSize / 2f * Vector3.forward);
            
            var centers = new Vector3[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    centers[i * width + j] = lowerLeft + i * m_cellSize * Vector3.forward + j * m_cellSize * Vector3.right;
                }
            }
            
            return centers;
        }
        
        private void CreateRenderers(int count)
        {
            m_lineRenderers = new LineRenderer[count];
            
            for (var i = 0; i < count; i++)
            {
                var lrObject = new GameObject($"LR_{i.ToString()}");
                lrObject.transform.SetParent(transform);

                var lineRenderer = lrObject.AddComponent<LineRenderer>();
                
                lineRenderer.startWidth = m_lineWidth;
                lineRenderer.endWidth = m_lineWidth;
                lineRenderer.sharedMaterial = m_lineMat;

                lineRenderer.numCapVertices = 2;
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

                m_lineRenderers[i] = lineRenderer;
                lrObject.SetActive(false);
            }
        }
    }
}