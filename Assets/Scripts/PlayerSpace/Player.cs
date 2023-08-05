using System.Collections.Generic;
using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Interfaces;
using Managers;
using UnityEngine;

namespace PlayerSpace
{
    public class Player : MonoBehaviour
    {
        private GameManager m_gameManager;

        private List<Gun> m_guns = new List<Gun>(); // TODO: Register Guns

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
            
            m_gameManager = DI.Resolve<GameManager>();
            var parameters = m_gameManager.GameParameters;
            
            m_moveRange = parameters.MoveRange;
            m_moveSpeed = parameters.PlayerSpeed;
            m_dragSensitivity = parameters.DragSensitivity;
            m_maxGunBurst = parameters.MaxGunBurst;
            
            m_baseFireRate = parameters.BaseFireRate;
            m_maxFireRate = parameters.MaxFireRate;

            m_baseFireRange = parameters.BaseFireRange;
            m_maxFireRange = parameters.MaxFireRange;
        }

        private void Update()
        {
            if (m_gameManager.MainStageOver)
                return;
            
            transform.position += m_moveSpeed * Time.deltaTime * Vector3.forward;

            if (Input.GetMouseButton(0))
                Swerve();
        }

        private void Swerve()
        {
            m_targetX += InputUtility.GetDrag(Vector3.right, m_dragSensitivity);
            m_targetX = Mathf.Clamp(m_targetX, -m_moveRange / 2f, m_moveRange / 2f);

            m_smoothX = Mathf.SmoothDamp(m_smoothX, m_targetX, ref m_dragSpeed, smoothTime);
            transform.position = transform.position.WithX(m_smoothX);
        }

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

        public void SetRange(float range) => m_range = range;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out IInteractable interactable))
                interactable.OnPlayerEnter(this);
        }
        
        public bool IsBulletOutOfRange(Vector3 bulletPos) => 
            Vector3.Distance(transform.position, bulletPos) > m_range;
    }
}