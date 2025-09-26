using System.Collections.Generic;
using DG.Tweening;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using ThisIsBlast.Config;
using ThisIsBlast.Utility;
using UnityEngine;

namespace ThisIsBlast.Block
{
    public class BlockQueuePresenter : MonoBehaviour
    {
        [SerializeField] private Transform _blockRoot;
        [SerializeField] private  List<Transform> _columnParents = new List<Transform>(10);
        
        private MultiColumnQueue<BlockBase> _model;
        private float _tileSize;
        
        public List<Transform> ColumnParents => _columnParents;

        public void Initialize(Transform blockRoot)
        {
            if (_blockRoot == null) _blockRoot = blockRoot;
            if (_blockRoot == null) return;

            var vc = Game.VisualConfig;
            _tileSize = Mathf.Max(0.0001f, vc.BoardWidth / Game.VisualConfig.BlockColumnCount);

            _model = new MultiColumnQueue<BlockBase>(Game.VisualConfig.BlockColumnCount);
            
            for (var i = 0; i < Game.VisualConfig.BlockColumnCount; i++)
            {
                if (_columnParents[i] == null) continue;
                var blocks = _columnParents[i].GetComponentsInChildren<BlockBase>(true);
                System.Array.Sort(blocks, (a, b) => a.transform.localPosition.z.CompareTo(b.transform.localPosition.z));
                foreach (var b in blocks)
                {
                    _model.AddBack(i, b);
                }
            }
        }

        public BlockBase GetFrontBlock(int column)
        {
            if (column < 0 || column >= Game.VisualConfig.BlockColumnCount) return null;
            return _model.GetFront(column);
        }
        
        public bool TryRemoveBlock(BlockBase block, out int columnIndex)
        {
            columnIndex = -1;
            if (block == null) return false;

            var removed = _model.TryRemove(block, out columnIndex, out _);
            if (removed)
            {
                CheckWinCondition();
            }
            return removed;
        }

        public void ShiftColumn(int col)
        {
            if (col < 0 || col >= Game.VisualConfig.BlockColumnCount) return;
            var parent = _columnParents[col];
            if (parent == null) return;
            var items = _model.Columns[col].Items;
            for (var i = 0; i < items.Count; i++)
            {
                var b = items[i];
                if (b == null) continue;
                
                // Calculate X position based on column index and spacing
                var x = (col - (Game.VisualConfig.BlockColumnCount - 1) * 0.5f) * Game.VisualConfig.BlockColumnSpacing;
                var target = new Vector3(x, 0f, (i + 0.5f) * _tileSize);
                b.transform.DOLocalMove(target, Game.VisualConfig.BlockShiftDuration).SetEase(Game.VisualConfig.BlockShiftEase);
            }
        }

        private void CheckWinCondition()
        {
            var allBlocksDestroyed = true;
            var totalBlocks = 0;

            for (var i = 0; i < Game.VisualConfig.BlockColumnCount; i++)
            {
                var frontBlock = _model.GetFront(i);
                if (frontBlock != null)
                {
                    allBlocksDestroyed = false;
                    break;
                }
                
                var column = _model.Columns[i];
                totalBlocks += column.Items.Count;
            }

            if (!allBlocksDestroyed || totalBlocks != 0) return;
            
            EventManager.PublishLevelStateChangeEvent(LevelState.LevelSuccess);
            FXManager.Instance.PlaySoundFX(SoundType.LevelSuccess);
        }

        public BlockBase GetBlock(BlockType blockType)
        {
            for (var i = 0; i < Game.VisualConfig.BlockColumnCount; i++)
            {
                var frontBlock = GetFrontBlock(i);
                if (frontBlock == null || frontBlock.BlockType != blockType || frontBlock.IsTarget) continue;
              
                frontBlock.SetAsTarget(true);
                return frontBlock;
            }

            return null;
        }
    }
}


