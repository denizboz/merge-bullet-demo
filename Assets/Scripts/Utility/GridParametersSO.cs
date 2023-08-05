using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "GridParameters", menuName = "Grid Parameters")]
    public class GridParametersSO : ScriptableObject
    {
        [Range(1f, 5f), Tooltip("In terms of world units.")]
        public float CellSize;

        [Range(0f, 5f), Tooltip("Grid elevation from the ground.")]
        public float GridHeight;

        [Range(0f, 5f), Tooltip("In order for the selected bullet to be rendered above other bullets.")]
        public float ElevationOffset;
        
        [Range(0.1f, 0.25f)]
        public float LineWidth = 0.1f;

        public Material LineMaterial;
    }
}