using ThisIsBlast.Enums;
using ThisIsBlast.View;
using System.Collections.Generic;
using UnityEngine;

namespace ThisIsBlast.Managers
{
    public static class UIManager 
    {
        private static Dictionary<System.Type, ViewBase> _spawnedViews = new Dictionary<System.Type, ViewBase>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            EventManager.OnLevelStateChange += OnLevelStateChange;
            EventManager.OnGameStateChange += OnGameStateChange;
        }

        private static void OnLevelStateChange(LevelState obj)
        {
            switch (obj)
            {
                case LevelState.LevelInitialize:
                    InitializeInGameUI();
                    break;
                case LevelState.LevelSuccess:
                    GetView<LevelSuccessView>().Show();
                    break;
                case LevelState.LevelFailed:
                    GetView<LevelFailView>().Show();
                    break;
                case LevelState.Invalid:
                    break;
            }
        }
        
        private static void OnGameStateChange(GameState obj)
        {
            if (obj == GameState.GameStart)
            {
                InitializeMenuUI();
            }
        }

        private static void InitializeInGameUI()
        {
            GetView<MenuView>().Hide();
            GetView<InGameView>().Show();
        }

        private static void InitializeMenuUI()
        {
            GetView<MenuView>().Show();
        }

        private static T GetView<T>() where T : ViewBase
        {
            var type = typeof(T);
            if (_spawnedViews.TryGetValue(type, out var view))
            {
                return view as T;
            }

            var spawned = Game.ViewConfig.GetView<T>() as T;
            if (spawned != null)
            {
                _spawnedViews[type] = spawned;
            }

            return spawned;
        }
    }
}