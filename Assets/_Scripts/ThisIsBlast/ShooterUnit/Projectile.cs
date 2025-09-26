using DG.Tweening;
using ThisIsBlast.Block;
using ThisIsBlast.Managers;
using ThisIsBlast.Config;
using UnityEngine;

namespace ThisIsBlast.ShooterUnit
{
    public class Projectile : MonoBehaviour
    {
        private TrailRenderer _trailRenderer;

        private void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
        }

        public void Initialize(Vector3 position)
        {
            transform.position = position;
            _trailRenderer.Clear();
        }
        
        public void FireAt(BlockBase target)
        {
            var targetPos = target.transform.position;
            
            transform.DOMove(targetPos, Game.VisualConfig.ProjectileSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    target.Destroy();
                    FXManager.Instance.PlayParticle(ParticleType.ProjectileHit,transform.position,Quaternion.identity);
                    PoolManager<Projectile>.DespawnObject(this);
                });
        }
    }
}
