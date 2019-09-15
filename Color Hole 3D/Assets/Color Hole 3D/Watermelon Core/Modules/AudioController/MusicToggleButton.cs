#pragma warning disable 0649 

using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class MusicToggleButton : BaseButton
    {
        [SerializeField]
        private Color activeColor;
        [SerializeField]
        private Color disableColor;

        private bool isActive = true;

        public new void Awake()
        {
            base.Awake();
            isActive = AudioController.GetMusicVolume() == 1.0f;

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
                AudioController.SetMusicVolume(1.0f);
            }
            else
            {
                graphic.color = disableColor;
                AudioController.SetMusicVolume(0.0f);
            }
        }
    }
}
