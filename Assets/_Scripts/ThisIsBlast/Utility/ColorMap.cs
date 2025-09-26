using UnityEngine;
using ThisIsBlast.Enums;

namespace ThisIsBlast.Utility
{
    public static class ColorMap
    {
        public static Color Get(BlockType type)
        {
            switch (type)
            {
                case BlockType.Blue: return new Color(0.12f, 0.50f, 0.95f);
                case BlockType.Green: return new Color(0.20f, 0.75f, 0.28f);
                case BlockType.Orange: return new Color(1.00f, 0.55f, 0.00f);
                case BlockType.Pink: return new Color(0.95f, 0.20f, 0.60f);
                case BlockType.Yellow: return new Color(1.00f, 0.85f, 0.20f);
                case BlockType.Purple: return new Color(0.57f, 0.30f, 0.95f);
                case BlockType.Red: return new Color(0.90f, 0.12f, 0.12f);
                default: return Color.gray;
            }
        }
    }
}



