using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "GridParameters", menuName = "Grid Parameters")]
    public class GridParametersSO : ScriptableObject
    {
        [Range(1f, 5f)]
        public float CellSize;
        
        [Range(0.1f, 0.25f)]
        public float LineWidth = 0.1f;

        public Material LineMaterial;
    }
}