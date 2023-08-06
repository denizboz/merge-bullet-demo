using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Merge;
using UnityEngine;

namespace PlayerSpace
{
    public class GunPlacer : MonoBehaviour
    {
        private float m_offset;

        private readonly List<Gun> m_guns = new List<Gun>();

        private const float tweenDuration = 0.25f;

        private void Awake()
        {
            DI.Bind(this);
            GameEventSystem.AddListener<GunsLoadedEvent>(OnGunsLoaded);
        }

        private void Start()
        {
            var gridRenderer = DI.Resolve<GridRenderer>();
            m_offset = gridRenderer.CellSize;
        }

        public void TakeGun(Gun gun)
        {
            gun.transform.SetParent(transform);
            m_guns.Add(gun);
            
            PlaceGuns();
        }

        private void PlaceGuns()
        {
            var count = m_guns.Count;

            if (count == 1)
            {
                m_guns[0].DoLocalMove(Vector3.zero, tweenDuration);
                return;
            }
            
            var startPos = (count - 1) * m_offset / 2f * Vector3.left;

            for (var i = 0; i < count; i++)
            {
                var localPos = startPos + i * m_offset * Vector3.right;
                m_guns[i].DoLocalMove(localPos, tweenDuration);
            }
        }

        private void OnGunsLoaded(object none)
        {
            var player = DI.Resolve<Player>();
            player.RegisterGuns(m_guns.ToArray());
            
            GameEventSystem.Invoke<LevelStartedEvent>();
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<GunsLoadedEvent>(OnGunsLoaded);
        }
    }
}