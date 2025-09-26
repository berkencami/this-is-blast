using UnityEngine;
using System.Collections.Generic;
using ThisIsBlast.Enums;

namespace ThisIsBlast.LevelSystem
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private ColumnData[] _blockColumns = new ColumnData[10];
        [SerializeField] private List<ShooterCell> _shooters = new List<ShooterCell>();
        [SerializeField] private int _shooterColumns = 5;
        
        public ColumnData[] BlockColumns => _blockColumns;
        public List<ShooterCell> Shooters => _shooters;
        public int ShooterColumns => _shooterColumns;
    }
    
    [System.Serializable]
    public class ColumnData
    {
        public List<BlockType> items = new List<BlockType>();
    }

    [System.Serializable]
    public class ShooterCell
    {
        public BlockType color;
        public int count;
    }
}