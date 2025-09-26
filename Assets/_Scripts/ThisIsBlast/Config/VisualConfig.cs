using ThisIsBlast.ShooterUnit;
using DG.Tweening;
using ThisIsBlast.Block;
using ThisIsBlast.Enums;
using ThisIsBlast.LevelSystem;
using UnityEngine;

namespace ThisIsBlast.Config
{
    [CreateAssetMenu(fileName = "VisualConfig", menuName = "ScriptableObjects/VisualConfig", order = 0)]
    public class VisualConfig : ScriptableObject
    {
        #region Prefabs References

        [Header("Prefabs References")]
        [SerializeField] private BlockBase _blockPrefab;
        [SerializeField] private Shooter _shooterPrefab;
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private LevelContainer _levelContainerPrefab;

        public BlockBase BlockPrefab => _blockPrefab;
        public Shooter ShooterPrefab => _shooterPrefab;
        public Projectile ProjectilePrefab => _projectilePrefab;
        public LevelContainer LevelContainer => _levelContainerPrefab;

        #endregion
        
        #region Materials References
        [Header("Materials References")]
        [SerializeField] private Material _blueMaterial;
        [SerializeField] private Material _greenMaterial;
        [SerializeField] private Material _orangeMaterial;
        [SerializeField] private Material _pinkMaterial;
        [SerializeField] private Material _yellowMaterial;
        [SerializeField] private Material _purpleMaterial;
        [SerializeField] private Material _redMaterial;
        
        public Material GetMaterial(BlockType type)
        {
            switch (type)
            {
                case BlockType.Blue: return _blueMaterial;
                case BlockType.Green: return _greenMaterial;
                case BlockType.Orange: return _orangeMaterial;
                case BlockType.Pink: return _pinkMaterial;
                case BlockType.Yellow: return _yellowMaterial;
                case BlockType.Purple: return _purpleMaterial;
                case BlockType.Red: return _redMaterial;
                default: return null;
            }
        }
        #endregion
       
        #region Board Board
        [Header("Board Board")]
        [SerializeField] private float _boardWidth = 5f;
        [SerializeField] private float _boardHeight = 8f;
        [SerializeField] private float _shooterCellSpacingMultiplier = 1f;
        [SerializeField] private int _blockColumnCount = 10;
        [SerializeField] private int _shooterColumnCount = 5;
        [SerializeField] private float _blockColumnSpacing = 0.5f;
        
        public float BoardWidth => _boardWidth;
        public float BoardHeight => _boardHeight;
        public float ShooterCellSpacingMultiplier => _shooterCellSpacingMultiplier;
        public int BlockColumnCount => _blockColumnCount;
        public int ShooterColumnCount => _shooterColumnCount;
        public float BlockColumnSpacing => _blockColumnSpacing;
        #endregion

        #region Shooter References

        [Header("Shooter References")] 
        [SerializeField] private float _moveDuration = 0.25f;
        [SerializeField] private float _projectileSpeed = 0.25f;
        [SerializeField] private float _colorChangeDuration = 0.25f;
        [SerializeField] private Color _disabledColor = Color.white;
        [SerializeField] private Ease _shooterScaleDownEase = Ease.OutBack;
        [SerializeField] private float _shooterRotateDuration = 0.3f;
        [SerializeField] private float _shooterRotationResetDuration = 1;
        [SerializeField] private float _outLineThickness = 0.1f;
        [SerializeField] private float _moveOutDistance = -20;
        [SerializeField] private float _moveOutDuration = 0.5f;
        [SerializeField] private float _shooterPunchAmount = 0.2f;
        [SerializeField] private float _shooterPunchDuration = 0.2f;

        public float MoveDuration => _moveDuration;
        public float ProjectileSpeed => _projectileSpeed;
        public float ColorChangeDuration => _colorChangeDuration;
        public Color DisabledColor => _disabledColor;
        public Ease ShooterScaleDownEase => _shooterScaleDownEase;
        public float ShooterRotateDuration => _shooterRotateDuration;
        public float ShooterRotationResetDuration => _shooterRotationResetDuration;
        public float OutLineThickness => _outLineThickness;
        public float MoveOutDistance => _moveOutDistance;
        public float MoveOutDuration => _moveOutDuration;
        public float ShooterPunchAmount => _shooterPunchAmount;
        public float ShooterPunchDuration => _shooterPunchDuration;
        
        
        #endregion

        #region Block References

        [SerializeField] private float _blockShiftDuration = 0.25f;
        [SerializeField] private Ease _blockShiftEase = Ease.OutQuad;
        [SerializeField] private float _blockScaleDownDuration = 0.25f;
        [SerializeField] private Ease _blockScaleDownEase = Ease.OutBack;
        
        public float BlockShiftDuration => _blockShiftDuration;
        public Ease BlockShiftEase => _blockShiftEase;
        public float BlockScaleDownDuration => _blockScaleDownDuration;
        public Ease BlockScaleDownEase => _blockScaleDownEase;
        

        #endregion

        #region Shooter Queue References

        [SerializeField] private float _shooterShiftDuration = 0.3f;
        [SerializeField] private Ease _shooterShiftEase = Ease.OutQuad;
        
        public float ShooterShiftDuration => _shooterShiftDuration;
        public Ease ShooterShiftEase => _shooterShiftEase;

        #endregion
        
        #region UI

        [Header("UI")]
        [SerializeField]private float _viewBackgroundAlpha = 0.8f;

        [SerializeField] private float _viewFadeDuration = 0.5f;
        [SerializeField] private float _buttonScaleUpDuration = 0.5f;
        [SerializeField] private Ease _buttonScaleUpEase = Ease.OutBack;
        [SerializeField] private float _buttonScaleUpDelay = 0;
        

        public float ViewBackgroundAlpha => _viewBackgroundAlpha;
        public float ViewFadeDuration => _viewFadeDuration;
        public float ButtonScaleUpDuration => _buttonScaleUpDuration;
        public Ease ButtonScaleUpEase => _buttonScaleUpEase;
        public float ButtonScaleUpDelay => _buttonScaleUpDelay;

        #endregion
    }
}