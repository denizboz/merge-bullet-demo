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
        [SerializeField] private LayerMask m_cellLayer;
        [SerializeField] private LayerMask m_groundLayer;

        private MergeGrid m_mergeGrid;
        private BulletFactory m_bulletFactory;
        
        private bool m_inMergePhase;
        
        private Ray m_camRay => Camera.main.ScreenPointToRay(Input.mousePosition);
        
        private Bullet m_selectedBullet;
        private Vector3 m_initialBulletPos;
        private float m_elevation;

        
        private void Awake()
        {
            m_mergeGrid = DI.Resolve<MergeGrid>();
            m_bulletFactory = DI.Resolve<BulletFactory>();

            m_inMergePhase = true; // for debug purpose
        }

        private void Start()
        {
            var gridRenderer = DI.Resolve<GridRenderer>();
            m_elevation = gridRenderer.GridElevation + gridRenderer.ElevationOffset;
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
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (!m_selectedBullet)
                    return;
                
                if (Physics.Raycast(m_camRay, out var hit, 50f, m_groundLayer))
                    m_selectedBullet.SetPosition(hit.point.WithY(m_elevation));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!m_selectedBullet)
                    return;
                
                if (Physics.Raycast(m_camRay, out var hit, 50f, m_cellLayer))
                {
                    if (hit.transform.TryGetComponentInParent(out GridCell cell))
                    {
                        var bulletAtCell = m_mergeGrid.BulletAtIndex(cell.Index);

                        if (!bulletAtCell)
                        {
                            m_selectedBullet.SetPosition(cell.Position);
                            
                            var index = m_mergeGrid.IndexOfBullet(m_selectedBullet);
                            
                            m_mergeGrid.UpdateCell(cell.Index, m_selectedBullet);
                            m_mergeGrid.UpdateCell(index, null);
                            
                            m_mergeGrid.SaveData();
                        }
                        else if (m_selectedBullet == bulletAtCell)
                        {
                            ReleaseSelectedBullet();
                        }
                        else if (bulletAtCell.Level == m_selectedBullet.Level)
                        {
                            MergeBullets(m_selectedBullet, bulletAtCell);
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
                else
                {
                    ReleaseSelectedBullet();
                }

                m_selectedBullet = null;
            }
        }

        private void MergeBullets(Bullet first, Bullet second)
        {
            var mergedBullet = m_bulletFactory.Get(level: first.Level + 1, size: 2);
            mergedBullet.SetPosition(second.Position);
            mergedBullet.ResetPreLevelHealth();

            var index1 = m_mergeGrid.IndexOfBullet(first);
            var index2 = m_mergeGrid.IndexOfBullet(second);
                        
            m_mergeGrid.UpdateCell(index1, null);
            m_mergeGrid.UpdateCell(index2, mergedBullet);
            
            m_mergeGrid.SaveData();
            
            GameEventSystem.Invoke<BulletDestroyedEvent>(first);
            GameEventSystem.Invoke<BulletDestroyedEvent>(second);
        }
        
        private void ReleaseSelectedBullet()
        {
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