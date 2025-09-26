using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThisIsBlast.Block;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using ThisIsBlast.Config;
using ThisIsBlast.ShooterBar;
using TMPro;
using UnityEngine;

namespace ThisIsBlast.ShooterUnit
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private BlockType _blockType;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _countText;
        [SerializeField] private Transform _shootPosition;

        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        private int _shootCount;
        private ShooterState _state = ShooterState.InQueue;
        private BarSlot _barSlot;
        private Vector3 _initialScale;
        private Collider _collider;

        public BlockType BlockType => _blockType;

        private void Awake()
        {
            _initialScale = transform.localScale;
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            EventManager.OnDestroyBlock += StartShooting;
        }
        
        private void OnDisable()
        {
            EventManager.OnDestroyBlock -= StartShooting;
        }

        public void Initialize(BlockType blockType, int count, Material material)
        {
            _blockType = blockType;
            _shootCount = count;
            _countText.text = count.ToString();
            if (_meshRenderer != null && material != null)
            {
                _meshRenderer.material = material;
            }

            SetState(ShooterState.InQueue);
            transform.localScale = _initialScale;
            _collider.enabled = true;
        }

        private void SetState(ShooterState state)
        {
            _state = state;
        }

        private void ConsumeOneShot()
        {
            _shootCount--;
            _countText.text = _shootCount.ToString();
            if (_shootCount > 0) return;
            Destroy().Forget();
        }

        public async UniTask MoveToBar(BarSlot targetSlot)
        {
            if (targetSlot == null) return;
           
            DisableOutline();
          
            FXManager.Instance.PlaySoundFX(SoundType.ShooterClick);
            _barSlot = targetSlot;
            targetSlot.SetShooter(this);
          
            SetState(ShooterState.MovingToBar);
          
            await transform.DOMove(targetSlot.transform.position, Game.VisualConfig.MoveDuration)
                .SetEase(Ease.OutQuad).AsyncWaitForCompletion();
           
            SetState(ShooterState.InBar);
            
            StartShooting();
        }
        
        private void StartShooting()
        {
            if (_state != ShooterState.InBar) return;
            var target = LevelManager.ActiveLevel.BlockQueuePresenter.GetBlock(_blockType);
            if (target == null)
            {
                transform.DOKill();
                transform.DORotate((Vector3.zero), Game.VisualConfig.ShooterRotationResetDuration);
                
                SetState(ShooterState.InBar);
                return;
            }
            
            ShootRoutine(target).Forget();
         
        }

        private async UniTaskVoid ShootRoutine(BlockBase target)
        {
            SetState(ShooterState.Firing);
            transform.DOKill();
            await transform.DOLookAt(target.transform.position, Game.VisualConfig.ShooterRotateDuration)
                .AsyncWaitForCompletion();

            FireAtTarget(target);
            ConsumeOneShot();

            await UniTask.Yield();
            StartShooting();
        }

        private void FireAtTarget(BlockBase target)
        {
            var visualConfig = Game.VisualConfig;
            var projectilePrefab = visualConfig.ProjectilePrefab;
            
            var projectile = PoolManager<Projectile>.SpawnObject(projectilePrefab);
          
            projectile.Initialize(_shootPosition.position);
            FXManager.Instance.PlaySoundFX(SoundType.Shoot,transform.GetInstanceID());
            
            projectile.FireAt(target);
           
            SetState(ShooterState.InBar);
          
        }

        public ShooterState GetState()
        {
            return _state;
        }

        private async UniTaskVoid Destroy()
        {
            SetState(ShooterState.Depleted);
            
            _barSlot.SetShooter(null);
            _collider.enabled = false;
           
            var vc = Game.VisualConfig;
            var sequence = DOTween.Sequence();
            
            transform.DOKill();
            _meshRenderer.material.DOKill();

            await sequence
                .Append(transform.DOPunchScale(_initialScale * vc.ShooterPunchAmount, vc.ShooterPunchDuration, 1, 10))
                .Append(_meshRenderer.material.DOColor(vc.DisabledColor, vc.ColorChangeDuration))
                .Join(transform.DORotate(Vector3.down * 90, vc.ShooterRotateDuration).SetEase(vc.ShooterScaleDownEase))
                .Join(transform.DOMoveX(vc.MoveOutDistance, vc.MoveOutDuration))
                .AsyncWaitForCompletion();
            
            sequence.Kill();
            
           PoolManager<Shooter>.DespawnObject(this);
        }

        public void EnableOutline()
        {
            _meshRenderer.material.SetFloat(OutlineThickness,Game.VisualConfig.OutLineThickness);
        }
        
        public void DisableOutline()
        {
            _meshRenderer.material.SetFloat(OutlineThickness,0);
        }
    }

    public enum ShooterState
    {
        InQueue,
        MovingToBar,
        InBar,
        Firing,
        Depleted
    }
}