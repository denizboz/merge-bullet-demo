using CommonTools.Runtime;
using Events;
using Events.Implementations;
using PlayerSpace;
using UnityEngine;

namespace Props
{
    public class LevelEnd : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponentInParent(out Player _))
                GameEventSystem.Invoke<LevelWonEvent>();
        }
    }
}