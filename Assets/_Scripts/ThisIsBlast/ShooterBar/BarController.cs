using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using ThisIsBlast.ShooterUnit;

namespace ThisIsBlast.ShooterBar
{
    public class BarController : MonoBehaviour
    {
        [SerializeField] private BarSlot[] _barSlots;
        
        private BarSlot GetFirstEmptySlot()
        {
            foreach (var s in _barSlots)
            {
                if (s != null && s.IsEmpty) return s;
            }
            return null;
        }

        public bool BarIsFull()
        {
            foreach (var barSlot in _barSlots)
            {
                if(barSlot.IsEmpty) return false;
            }

            return true;
        }

        public bool HasTarget()
        {
            var shooterColors = new HashSet<BlockType>();
            foreach (var barSlot in _barSlots)
            {
                if (!barSlot.IsEmpty)
                {
                    shooterColors.Add(barSlot.Shooter.BlockType);
                }
            }
            
            if (shooterColors.Count == 0)
            {
                return false;
            }
            
            var blockColors = new HashSet<BlockType>();
            for (var i = 0; i < Game.VisualConfig.BlockColumnCount; i++)
            {
                var frontBlock = LevelManager.ActiveLevel.BlockQueuePresenter.GetFrontBlock(i);
                if (frontBlock != null)
                {
                    blockColors.Add(frontBlock.BlockType);
                }
            }
            
            return shooterColors.Overlaps(blockColors);
        }

        public async UniTask<bool> RequestFillSlot(Shooter shooter)
        {
            var slot = GetFirstEmptySlot();
            if (slot == null) return false;
            await shooter.MoveToBar(slot);
            EventManager.PublishOnFillBar();
            return true;
        }
        
        
    }
}