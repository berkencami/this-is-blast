using ThisIsBlast.Config;
using ThisIsBlast.LevelSystem;
using UnityEngine;

namespace ThisIsBlast.Managers
{
    public static class Game
    {
        public static VisualConfig VisualConfig { get; private set; } 
        public static ViewConfig ViewConfig { get; private set; }
      
        private static LevelConfig levelConfig;

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            VisualConfig = Resources.Load<VisualConfig>("Configs/VisualConfig");
            ViewConfig = Resources.Load<ViewConfig>("Configs/ViewConfig");
            levelConfig = Resources.Load<LevelConfig>("Configs/LevelConfig");
        }
        
        public static LevelData LoadLevel()
        {
            return levelConfig.GetLevel();
        }
    }
}