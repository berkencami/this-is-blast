using ThisIsBlast.Enums;
using ThisIsBlast.ShooterUnit;
using ThisIsBlast.Utility;
using UnityEngine;

namespace ThisIsBlast.Managers
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private LayerMask _shooterLayer;
        [SerializeField] private Camera _mainCamera;
        private bool _inputEnabled = true;

        private void Awake()
        {
            EventManager.OnLevelStateChange += OnLevelStateChange;
        }

        private void OnLevelStateChange(LevelState obj)
        {
            _inputEnabled = obj == LevelState.LevelInitialize;
        }

        private void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if(!_inputEnabled) return;
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var mask = _shooterLayer.value == 0 ? ~0 : _shooterLayer.value;


            if (!Physics.Raycast(ray, out var hit, 1000f, mask)) return;
            var shooter = hit.collider ? hit.collider.GetComponentInParent<Shooter>() : null;
            if (shooter == null || shooter.GetState() != ShooterState.InQueue) return;
                
            var presenter = LevelManager.ActiveLevel.ShooterQueuePresenter;
            if(LevelManager.ActiveLevel.BarController.BarIsFull()) return;
            presenter.TrySendToBar(shooter);
        }
        
    }
} 