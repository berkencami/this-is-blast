using ThisIsBlast.UI;
using UnityEngine;

namespace ThisIsBlast.View
{
    public class InGameView : ViewBase
    {
        [SerializeField] private LevelText _levelText;
        
        protected override void OnShow()
        {
            _levelText.SetLevelText();
        }

        protected override void OnHide()
        {
            
        }
    }

}