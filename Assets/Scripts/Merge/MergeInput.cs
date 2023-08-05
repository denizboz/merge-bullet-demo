using CommonTools.Runtime;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using Managers;
using PlayerSpace;
using UnityEngine;

namespace Merge
{
    public class MergeInput : MonoBehaviour
    {
        [SerializeField] private LayerMask m_bulletLayer;
        [SerializeField] private LayerMask m_groundLayer;

        private MergeGrid m_mergeGrid;
        private BulletFactory m_bulletFactory;
        
        private bool m_inMergePhase;
        
        private Ray m_camRay => Camera.main.ScreenPointToRay(Input.mousePosition);
        
        private Bullet m_selectedBullet;
        private Vector3 m_initialBulletPos;
        private float m_bulletHeight;

        private void Awake()
        {
            m_mergeGrid = DI.Resolve<MergeGrid>();
            m_bulletFactory = DI.Resolve<BulletFactory>();
            
            m_inMergePhase = true; // for debug purpose
        }

        private void Update()
        {
            if (!m_inMergePhase)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(m_camRay, out var hit, 50f, m_bulletLayer))
                {
                    if (hit.transform.TryGetComponentInParent(out Bullet bullet))
                    {
                        m_selectedBullet = bullet;
                        m_initialBulletPos = bullet.Position;
                        m_bulletHeight = m_initialBulletPos.y;
                        
                        bullet.EnableCollider(false);
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (!m_selectedBullet)
                    return;
                
                if (Physics.Raycast(m_camRay, out var hit, 50f, m_groundLayer))
                    m_selectedBullet.SetPosition(hit.point.WithY(m_bulletHeight));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!m_selectedBullet)
                    return;
                
                if (Physics.Raycast(m_camRay, out var hit, 50f, m_bulletLayer))
                {
                    if (hit.transform.TryGetComponentInParent(out Bullet secondBullet))
                    {
                        if (secondBullet.Level != m_selectedBullet.Level)
                        {
                            ReleaseSelectedBullet();
                            return;
                        }

                        MergeBullets(m_selectedBullet, secondBullet);
                        m_selectedBullet = null;
                    }
                    else
                    {
                        ReleaseSelectedBullet();
                    }
                }
                else
                {
                    ReleaseSelectedBullet();
                }
            }
        }

        private void MergeBullets(Bullet first, Bullet second)
        {
            var mergedBullet = m_bulletFactory.Get(level: first.Level + 1, size: 2);
            
            mergedBullet.SetPosition(second.Position);
            mergedBullet.SetGridIndex(second.GridIndex);
                        
            m_mergeGrid.UpdateCell(first.GridIndex, null);
            m_mergeGrid.UpdateCell(second.GridIndex, mergedBullet);
            
            m_bulletFactory.Return(first);
            m_bulletFactory.Return(second);
        }
        
        private void ReleaseSelectedBullet()
        {
            m_selectedBullet.EnableCollider(true);
            m_selectedBullet.TweenPosition(m_initialBulletPos, 0.1f);

            m_selectedBullet = null;
        }
        
        private void EnableInput(object none)
        {
            m_inMergePhase = true;
        }

        private void DisableInput(object none)
        {
            m_inMergePhase = false;
        }
    }
}