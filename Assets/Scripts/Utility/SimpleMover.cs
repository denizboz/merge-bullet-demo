using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Utility
{
    public class SimpleMover : MonoBehaviour, IDependency
    {
        private float m_speed;

        public Vector3 Position => transform.position;

        public void SetPosition(Vector3 pos) => transform.position = pos;


        public void Bind()
        {
            DI.Bind(this);
        }

        private void Awake()
        {
            EnableMoving(false);
            
            GameEventSystem.AddListener<LevelStartedEvent>(OnLevelStarted);
        }

        private void Update()
        {
            transform.position += m_speed * Time.deltaTime * transform.forward;
        }

        private void OnLevelStarted(object none)
        {
            EnableMoving(false);
        }
        
        public void EnableMoving(bool value, float speed = 0f)
        {
            enabled = value;
            m_speed = speed;
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelStartedEvent>(OnLevelStarted);
        }
    }
}