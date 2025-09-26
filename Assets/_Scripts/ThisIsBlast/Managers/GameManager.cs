using ThisIsBlast.Config;
using UnityEngine;
using ThisIsBlast.Enums;

namespace ThisIsBlast.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        
        private void Awake()
        {
            Application.targetFrameRate = _gameConfig.TargetFrameRate;
            EventManager.PublishGameStateChangeEvent(GameState.Initial);
        }

        private void Start()
        {
            EventManager.PublishGameStateChangeEvent(GameState.GameStart);
        }
    }
}