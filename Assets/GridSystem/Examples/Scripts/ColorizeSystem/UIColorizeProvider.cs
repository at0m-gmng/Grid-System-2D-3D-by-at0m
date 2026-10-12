namespace GridSystem.ColorizeSystem
{
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class UIColorizeProvider : AbstractColorizeProvider
    {
        [SerializeField]
        private Image _image;

        public override void ColorizeToCoreect()
            => _image.color = ColorizeWithAlpha(Color.green, 1f);

        public override void ColorizeToIncoreect()
            => _image.color = ColorizeWithAlpha(Color.red, 1f);

        public override void ColorizeToBase()
            => _image.color = ColorizeWithAlpha(Color.white, 1f);
    }
}