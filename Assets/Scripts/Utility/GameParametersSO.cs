using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "GameParameters", menuName = "Game Parameters")]
    public class GameParametersSO : ScriptableObject
    {
        [Range(5f, 25f)]
        public float PlayerSpeed;

        [Range(5f, 25f)]
        public float DragSensitivity;
        
        [Range(1f, 10f)]
        public float MoveRange;

        [Range(1f, 10f)]
        public float BaseFireRate;

        [Range(10f, 50f)]
        public float BulletSpeed;

        [Range(1, 10), Tooltip("Max number of bullets that can be fired from a gun at once.")]
        public int MaxGunBurst;

        [Range(5f, 90f), Tooltip("Angle to be covered by bullets in multiple shot scenario.")]
        public float BurstAngleCover;
    }
}