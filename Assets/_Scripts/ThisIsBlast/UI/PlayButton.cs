using System;
using ThisIsBlast.Enums;
using ThisIsBlast.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace ThisIsBlast.UI
{
    public class PlayButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            EventManager.PublishLevelStateChangeEvent(LevelState.LevelInitialize);
        }
    }
}