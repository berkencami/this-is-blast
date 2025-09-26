using Cysharp.Threading.Tasks;
using DG.Tweening;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using UnityEngine;

namespace ThisIsBlast.Block
{
    public class BlockBase : MonoBehaviour
    {
        [SerializeField] private BlockType _blockType;
        [SerializeField] private MeshRenderer _meshRenderer;

        private bool _isTarget = false;
        private Vector3 _initializeScale;

        public BlockType BlockType => _blockType;
        public bool IsTarget => _isTarget;

        private void Awake()
        {
            _initializeScale = transform.localScale;
        }

        public void Initialize(BlockType blockType, Material material)
        {
            _blockType = blockType;
            _isTarget = false;
            if (_meshRenderer != null && material != null)
            {
                _meshRenderer.material = material;
            }

            transform.localScale = _initializeScale;
        }

        public void SetAsTarget(bool isTarget)
        {
            _isTarget = isTarget;
        }

        public void Destroy()
        {
            var level = LevelManager.ActiveLevel;
            var blockPresenter = level?.BlockQueuePresenter;
            if (blockPresenter == null) return;

            if (!blockPresenter.TryRemoveBlock(this, out var columnIndex)) return;

            SetAsTarget(false);

            blockPresenter.ShiftColumn(columnIndex);
            EventManager.PublishOnDestroyBlockEvent();
            DestroySequence().Forget();
        }

        private async UniTask DestroySequence()
        {
           await transform.DOScale(Vector3.zero, Game.VisualConfig.BlockScaleDownDuration)
                .SetEase(Game.VisualConfig.BlockScaleDownEase).AsyncWaitForCompletion();
           
            PoolManager<BlockBase>.DespawnObject(this);
        }
    }
}