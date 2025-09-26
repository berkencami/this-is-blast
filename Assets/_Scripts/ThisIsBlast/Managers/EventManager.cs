using System;
using ThisIsBlast.Enums;

namespace ThisIsBlast.Managers
{
    public static class EventManager
    {
        public static event Action<GameState> OnGameStateChange;
        private static GameState gameState;
        
        public static event Action<LevelState> OnLevelStateChange;
        private static LevelState levelState;
        
        public static event Action OnFillBar;
        public static event Action OnDestroyBlock;
        
        public static void PublishGameStateChangeEvent(GameState newState)
        {
            if(gameState==newState) return;
            gameState = newState;
            OnGameStateChange?.Invoke(newState);
        }
        
        public static void PublishLevelStateChangeEvent(LevelState newState)
        {
            if(levelState==newState) return;
            levelState = newState;
            OnLevelStateChange?.Invoke(levelState);
        }
        
        public static void PublishOnFillBar()
        {
            OnFillBar?.Invoke();
        }

        public static void PublishOnDestroyBlockEvent()
        {
            OnDestroyBlock?.Invoke();
        }
    }
}