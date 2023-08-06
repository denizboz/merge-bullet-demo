using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Interfaces;
using Managers;
using UnityEngine;

namespace PlayerSpace
{
    public class Player : MonoBehaviour
    {
        private bool m_isPlaying;

        private Gun[] m_guns;

        private float m_moveRange;
        private float m_dragSensitivity;
        private float m_moveSpeed;
        
        private float m_targetX;
        private float m_smoothX;
        private float m_dragSpeed;
        
        private float m_baseFireRate;
        private float m_maxFireRate;

        private float m_baseFireRange;
        private float m_maxFireRange;
        
        private int m_maxGunBurst;

        private const float smoothTime = 0.1f;

        private float m_range;
        
        
        private void Awake()
        {
            DI.Bind(this);
            
            var gameManager = DI.Resolve<GameManager>();
            var parameters = gameManager.GameParameters;
            
            m_moveRange = parameters.MoveRange;
            m_moveSpeed = parameters.PlayerSpeed;
            m_dragSensitivity = parameters.DragSensitivity;
            m_maxGunBurst = parameters.MaxGunBurst;
            
            m_baseFireRate = parameters.BaseFireRate;
            m_maxFireRate = parameters.MaxFireRate;

            m_baseFireRange = parameters.BaseFireRange;
            m_maxFireRange = parameters.MaxFireRange;
            
            GameEventSystem.AddListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.AddListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.AddListener<LevelFailedEvent>(OnLevelFinished);
        }

        
        private void Update()
        {
            if (!m_isPlaying)
                return;
            
            if (Input.GetMouseButton(0))
            {
                transform.position += m_moveSpeed * Time.deltaTime * Vector3.forward;
                Swerve();
            }
        }

        private void Swerve()
        {
            m_targetX += InputUtility.GetDrag(Vector3.right, m_dragSensitivity);
            m_targetX = Mathf.Clamp(m_targetX, -m_moveRange / 2f, m_moveRange / 2f);

            m_smoothX = Mathf.SmoothDamp(m_smoothX, m_targetX, ref m_dragSpeed, smoothTime);
            transform.position = transform.position.WithX(m_smoothX);
        }

        public void RegisterGuns(Gun[] guns) => m_guns = guns;
        
        public void SetFireRate(float fireRate)
        {
            fireRate = Mathf.Clamp(fireRate, m_baseFireRate, m_maxFireRate);
            
            foreach (var gun in m_guns)
            {
                gun.SetFireRate(fireRate);
            }
        }
        
        /// <summary>
        /// Must be odd in order for one bullet to be shot from the middle.
        /// </summary>
        public void SetFireBurst(int burst)
        {
            burst = Mathf.Clamp(burst, 1, m_maxGunBurst);

            foreach (var gun in m_guns)
            {
                gun.SetFireBurst(burst);
            }
        }

        public void SetBulletSize(int size)
        {
            foreach (var gun in m_guns)
            {
                gun.SetBulletSize(size);
            }
        }

        public void SetRange(float range)
        {
            m_range = Mathf.Clamp(range, m_baseFireRange, m_maxFireRange);
        }

        private void OnLevelStarted(object none)
        {
            var gameManager = DI.Resolve<GameManager>();
            var parameters = gameManager.GameParameters;

            SetRange(parameters.BaseFireRange);
            SetBulletSize(1);
            
            m_isPlaying = true;
            
            foreach (var gun in m_guns)
            {
                gun.EnableFiring(true);
            }
        }

        private void OnLevelFinished(object none)
        {
            m_isPlaying = false;
        }
        
        public bool IsBulletOutOfRange(Vector3 bulletPos) => 
            Vector3.Distance(transform.position, bulletPos) > m_range;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IInteractable interactable))
                interactable.OnPlayerEnter(this);
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<LevelStartedEvent>(OnLevelStarted);
            GameEventSystem.RemoveListener<LevelWonEvent>(OnLevelFinished);
            GameEventSystem.RemoveListener<LevelFailedEvent>(OnLevelFinished);
        }
    }
}