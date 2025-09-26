using ThisIsBlast.LevelSystem;
using ThisIsBlast.Managers;
using UnityEngine;

namespace ThisIsBlast.Config
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private LevelData[] _levels;

        public LevelData GetLevel()
        {
            var levelIndex = DataManager.GetLevelIndex() % _levels.Length;
            return _levels[levelIndex];
        }
    }
}