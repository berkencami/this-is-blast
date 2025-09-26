using ThisIsBlast.ShooterUnit;
using UnityEngine;

namespace ThisIsBlast.ShooterBar
{
    public class BarSlot : MonoBehaviour
    {
        private Shooter _shooter;
        public bool IsEmpty => _shooter == null;
        public Shooter Shooter => _shooter;
        
        public void SetShooter(Shooter shooter)
        {
            _shooter = shooter;
        }
    }
}
