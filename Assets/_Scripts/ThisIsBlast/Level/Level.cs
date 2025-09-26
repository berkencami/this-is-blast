using ThisIsBlast.Block;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using ThisIsBlast.ShooterBar;
using ThisIsBlast.ShooterUnit;
using UnityEngine;

namespace ThisIsBlast.LevelSystem
{
    public sealed class Level
    {
        private LevelContainer _levelContainer;
        private ShooterQueuePresenter _shooterQueuePresenter;
        private BlockQueuePresenter _blockQueuePresenter;
        private LevelData _levelData;

        private BarController _barController;
        public BarController BarController => _barController;
        public ShooterQueuePresenter ShooterQueuePresenter => _shooterQueuePresenter;
        public BlockQueuePresenter BlockQueuePresenter => _blockQueuePresenter;
        public LevelContainer LevelContainer => _levelContainer;

        public Level(LevelData levelData)
        {
            _levelData = levelData;
            Load(_levelData);
        }
        
        private void Load(LevelData levelData)
        {
            if (levelData == null) return ;

            var visualConfig = Game.VisualConfig;
            var levelContainer = Object.Instantiate(visualConfig.LevelContainer);
            _levelContainer = levelContainer;
            _shooterQueuePresenter = levelContainer.ShooterQueuePresenter;
            _blockQueuePresenter = levelContainer.BlockQueuePresenter;
            _barController = levelContainer.ShooterBar;
          
            var boardWidth = Mathf.Max(0.1f, visualConfig.BoardWidth);
            var boardHeight = Mathf.Max(0.1f, visualConfig.BoardHeight);
            var tileSize = boardWidth / 10f;
            var visibleRows = Mathf.Max(1, Mathf.FloorToInt(boardHeight / tileSize));
            
            ClearChildren(levelContainer.ShooterTransform);
            
            
            for (var i = 0; i < visualConfig.BlockColumnCount; i++)
            {
                var items = (levelData.BlockColumns != null && i < levelData.BlockColumns.Length) ? levelData.BlockColumns[i].items : null;
                if (items == null) continue;
                var n = Mathf.Min(visibleRows, items.Count);
                for (var r = 0; r < n; r++)
                {
                    var type = items[r];
                    if (type == BlockType.None) continue;
                    var mat = visualConfig.GetMaterial(type);
                    var block =  PoolManager<BlockBase>.SpawnObject(visualConfig.BlockPrefab,levelContainer.BlockQueuePresenter.ColumnParents[i]);
                    
                    
                    var x = (i - (visualConfig.BlockColumnCount - 1) * 0.5f) * visualConfig.BlockColumnSpacing;
                  
                    var z = (r + 0.5f) * tileSize;
                    block.transform.localPosition = new Vector3(x, 0f, z);
                    block.Initialize(type, mat);
                }
            }
            
            if (levelData.Shooters != null && levelData.Shooters.Count > 0)
            {
                var cols = Mathf.Max(1, levelData.ShooterColumns);
                var spacing = tileSize * Mathf.Max(0.1f, visualConfig.ShooterCellSpacingMultiplier);
                for (var idx = 0; idx < levelData.Shooters.Count; idx++)
                {
                    var cell = levelData.Shooters[idx];
                    if (cell == null) continue;
                    var type = cell.color;
                    var count = cell.count;
                    if (type == BlockType.None || count <= 0) continue;
                    var mat = visualConfig.GetMaterial(type);
                    var c = idx % cols;
                    var r = idx / cols;
                    var shooter = PoolManager<Shooter>.SpawnObject(visualConfig.ShooterPrefab, levelContainer.ShooterTransform);
                    var x = -((cols * spacing) * 0.5f) + (c + 0.5f) * spacing;
                    var z = -((r + 0.5f) * spacing);
                    shooter.transform.localPosition = new Vector3(x, 0f, z);
                    shooter.Initialize(type, count, mat);
                }
            }
            
            _blockQueuePresenter?.Initialize(levelContainer.BlockQueuePresenter.transform);
            _shooterQueuePresenter?.Initialize(null, levelData.ShooterColumns);
        }

        private static void ClearChildren(Transform parent)
        {
            if (parent == null) return;
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }
    }
}