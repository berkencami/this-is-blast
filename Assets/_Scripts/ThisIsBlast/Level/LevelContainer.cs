using ThisIsBlast.ShooterBar;
using ThisIsBlast.Block;
using UnityEngine;

namespace ThisIsBlast.LevelSystem
{
    public class LevelContainer : MonoBehaviour
    {
        [SerializeField] private BarController _shooterBar;
        [SerializeField] private Transform _shooterTransform;
        [SerializeField] private ShooterQueuePresenter _shooterQueuePresenter;
        [SerializeField] private BlockQueuePresenter _blockQueuePresenter;

        public Transform ShooterTransform => _shooterTransform;
        public BarController ShooterBar => _shooterBar;
        public ShooterQueuePresenter ShooterQueuePresenter => _shooterQueuePresenter;
        public BlockQueuePresenter BlockQueuePresenter => _blockQueuePresenter;
    }
}
