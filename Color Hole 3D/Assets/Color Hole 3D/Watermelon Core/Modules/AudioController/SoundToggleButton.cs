#pragma warning disable 0649 

using UnityEngine;

namespace Watermelon
{
    public class SoundToggleButton : BaseButton
    {
        [SerializeField]
        private Color activeColor;
        [SerializeField]
        private Color disableColor;

        private bool isActive = true;

        public new void Awake()
        {
            base.Awake();
            isActive = AudioController.GetSoundVolume() == 1.0f;

            if (isActive)
                graphic.color = activeColor;
            else
                graphic.color = disableColor;
        }

        public override void OnClick(Tween.TweenCallback callback = null)
        {
            base.OnClick(callback);

            isActive = !isActive;

            if (isActive)
            {
                graphic.color = activeColor;
                AudioController.SetSoundVolume(1.0f);
            }
            else
            {
                graphic.color = disableColor;
                AudioController.SetSoundVolume(0.0f);
            }
        }
    }
}
