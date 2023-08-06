using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "Level_00", menuName = "New Level")]
    public class LevelSO : ScriptableObject
    {
        public int Number;
        public GameObject LevelObjects;
    }
}