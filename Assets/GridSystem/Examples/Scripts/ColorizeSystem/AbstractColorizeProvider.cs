namespace GridSystem.ColorizeSystem
{
    using UnityEngine;

    public abstract class AbstractColorizeProvider : MonoBehaviour
    {
        public abstract void ColorizeToCoreect();
        public abstract void ColorizeToIncoreect();
        public abstract void ColorizeToBase();
        protected Color ColorizeWithAlpha(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
    }
}