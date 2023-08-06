using System;
using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Events;
using Events.Implementations;
using PlayerSpace;
using UnityEngine;

namespace Utility
{
    public enum FollowMode { Merge, PreLevel, Level }
    
    public class CameraController : MonoBehaviour, IDependency
    {
        [SerializeField] private Transform m_cameraTr;
        [SerializeField] private Transform[] m_presets;

        private Vector3 m_initialPos;
        
        private Transform m_tr;
        [SerializeField] private Transform m_target;

        private Vector3 m_targetPos;
        
        private bool m_isFollowing;

        private Vector3 m_velocity;
        
        private const float smoothTime = 0.05f;
        private const float tweenDuration = 0.33f;

        public void EnableFollowing(bool value) => m_isFollowing = value;
        

        public void Bind()
        {
            DI.Bind(this);
        }
        
        private void Awake()
        {
            m_tr = transform;
            m_initialPos = m_tr.position;
            
            GameEventSystem.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.AddListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.AddListener<LevelFailedEvent>(OnLevelFinished);
        }

        private void Update()
        {
            if (!m_isFollowing)
                return;

            m_targetPos = m_target.position.WithXY(0f, 0f);
            m_tr.position = Vector3.SmoothDamp(m_tr.position, m_targetPos, ref m_velocity, smoothTime);
        }

        public void SetTarget(Transform target)
        {
            m_target = target;
        }
        
        public void SetMode(FollowMode mode)
        {
            var preset = m_presets[(int)mode];

            var duration = mode == FollowMode.Merge ? 0f : tweenDuration;
            
            m_cameraTr.DOLocalMove(preset.localPosition, duration).SetEase(Ease.InOutQuad);
            m_cameraTr.DOLocalRotateQuaternion(preset.localRotation, duration).SetEase(Ease.InOutQuad);
        }

        public void SetPosition(Vector3 pos, bool tweened = true)
        {
            var targetPos = pos.WithXY(0f, 0f);
            m_tr.DOMove(targetPos, tweened ? tweenDuration : 0f);
        }
        
        private void OnLevelLoaded(object none)
        {
            m_isFollowing = false;
            
            SetMode(FollowMode.Merge);
            SetPosition(m_initialPos, tweened: false);
        }

        private void OnLevelFinished(object none)
        {
            m_isFollowing = false;
        }
        
        private void OnLevelStarted(object none)
        {
            var player = DI.Resolve<Player>();
            
            SetMode(FollowMode.Level);
            SetTarget(player.transform);
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.RemoveListener<LevelLoadedEvent>(OnLevelLoaded);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFinished);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_cameraTr = GetComponentInChildren<Camera>().transform;
            
            var followModeVariety = Enum.GetValues(typeof(FollowMode)).Length;
            
            if (m_presets.Length != followModeVariety)
                Debug.LogWarning("Number of presets must be equal to FollowMode variety size.");
        }
#endif
    }
}