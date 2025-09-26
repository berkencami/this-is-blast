using ThisIsBlast.Enums;
using ThisIsBlast.LevelSystem;
using ThisIsBlast.Config;
using UnityEngine;

namespace ThisIsBlast.Managers
{
    public static class LevelManager
    {
        private static Level _activeLevel;

        public static Level ActiveLevel => _activeLevel;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            EventManager.OnLevelStateChange += OnLevelStateChange;
            EventManager.OnFillBar += CheckLevelFail;
            EventManager.OnDestroyBlock += CheckLevelFail;
        }

        private static void OnLevelStateChange(LevelState obj)
        {
            if (obj != LevelState.LevelInitialize) return;
            if (_activeLevel != null)
            {
                PoolManager.DespawnAllObjects();
                Object.Destroy(_activeLevel.LevelContainer.gameObject);
                _activeLevel = null;
            }

            _activeLevel = new Level(Game.LoadLevel());
        }

        private static void CheckLevelFail()
        {
            if (!_activeLevel.BarController.BarIsFull() || _activeLevel.BarController.HasTarget()) return;
          
            EventManager.PublishLevelStateChangeEvent(LevelState.LevelFailed);
            FXManager.Instance.PlaySoundFX(SoundType.LevelFail);

        }
    }
}