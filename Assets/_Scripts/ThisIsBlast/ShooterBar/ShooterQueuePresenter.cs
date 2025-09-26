using System.Collections.Generic;
using DG.Tweening;
using ThisIsBlast.Managers;
using ThisIsBlast.ShooterUnit;
using ThisIsBlast.Utility;
using UnityEngine;

namespace ThisIsBlast.ShooterBar
{
    public class ShooterQueuePresenter : MonoBehaviour
    {
        [SerializeField] private Transform _root; // parent of all Shooter instances

        private MultiColumnQueue<Shooter> _model;
        private int _columns;
        private float _spacing;
        private float _halfExtent;
        

        public void Initialize(Transform anyShooterInRoot, int columnCount = -1)
        {
            if (_root == null)
            {
                _root = anyShooterInRoot ? anyShooterInRoot.parent : null;
            }

            if (_root == null)
            {
                Debug.LogWarning("ShooterQueuePresenter: Root not found.");
                return;
            }

            var vc = Game.VisualConfig;
            var tileSize = Mathf.Max(0.0001f, vc.BoardWidth / vc.BlockColumnCount);
            _columns = columnCount > 0 ? columnCount : Mathf.Max(1, vc.ShooterColumnCount);
            _spacing = tileSize * Mathf.Max(0.1f, vc.ShooterCellSpacingMultiplier);
            _halfExtent = (_columns * _spacing) * 0.5f;

            _model = new MultiColumnQueue<Shooter>(_columns);

            // Scan children -> assign to (col,row) -> add to model preserving order by row
            var shooters = _root.GetComponentsInChildren<Shooter>(true);
            var tempByColumn = new List<(Shooter s, int row)>[_columns];
            for (var i = 0; i < _columns; i++) tempByColumn[i] = new List<(Shooter, int)>();

            foreach (var s in shooters)
            {
                var lp = s.transform.localPosition;
                var col = Mathf.Clamp(Mathf.RoundToInt(((lp.x + _halfExtent) / _spacing) - 0.5f), 0, _columns - 1);
                var row = Mathf.Max(0, Mathf.RoundToInt(((-lp.z) / _spacing) - 0.5f));
                tempByColumn[col].Add((s, row));
            }

            for (var c = 0; c < _columns; c++)
            {
                tempByColumn[c].Sort((a, b) => a.row.CompareTo(b.row));
                foreach (var pair in tempByColumn[c])
                {
                    _model.AddBack(c, pair.s);
                }
            }

            // Enable outline for shooters in the front row (row 0)
            EnableOutlineForFrontRow();
        }

        public void TrySendToBar(Shooter shooter)
        {
            if (shooter == null) return;
            
            if (!_model.TryRemove(shooter, out var col, out var row))
            {
                return;
            }
            
            if (row != 0)
            {
                _model.AddBack(col, shooter);
                return;
            }
            
            var result =  LevelManager.ActiveLevel.BarController.RequestFillSlot(shooter);
          
            if (!result.Equals(false)) 
            {
                // Shooter successfully sent to bar, shift remaining shooters in this column
                ShiftColumn(col);
                return;
            }
            _model.AddBack(col, shooter);
        }

        public void ShiftColumn(int col)
        {
            if (col < 0 || col >= _columns) return;
            
            var items = _model.Columns[col].Items;
            for (var i = 0; i < items.Count; i++)
            {
                var shooter = items[i];
                if (shooter == null) continue;
                
                // Calculate new position based on row index
                var x = -((_columns * _spacing) * 0.5f) + (col + 0.5f) * _spacing;
                var z = -((i + 0.5f) * _spacing); // Move forward (towards camera)
                var target = new Vector3(x, 0f, z);
                
                shooter.transform.DOLocalMove(target, Game.VisualConfig.ShooterShiftDuration)
                    .SetEase(Game.VisualConfig.ShooterShiftEase);
            }

            // Update outline for the shifted column
            UpdateOutlineForColumn(col);
        }

        private void EnableOutlineForFrontRow()
        {
            for (var c = 0; c < _columns; c++)
            {
                var frontShooter = _model.GetFront(c);
                if (frontShooter != null)
                {
                    frontShooter.EnableOutline();
                }
            }
        }

        private void UpdateOutlineForColumn(int col)
        {
            if (col < 0 || col >= _columns) return;

            // Disable outline for all shooters in this column first
            var items = _model.Columns[col].Items;
            foreach (var shooter in items)
            {
                if (shooter != null)
                {
                    shooter.DisableOutline();
                }
            }

            // Enable outline for the front shooter in this column
            var frontShooter = _model.GetFront(col);
            if (frontShooter != null)
            {
                frontShooter.EnableOutline();
            }
        }
    }
}


