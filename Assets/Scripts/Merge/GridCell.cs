using UnityEngine;

namespace Merge
{
    public class GridCell : MonoBehaviour
    {
        public int Index { get; private set; }

        public Vector3 Position => transform.position;

        public void SetIndex(int index) => Index = index;
    }
}