using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public static class TweenExtensions
    {
        #region Transform
        /// <summary>
        /// Changes rotation angle of object.
        /// </summary>
        public static TweenCase DORotate(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomRotateAngle(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes quaternion rotation of object.
        /// </summary>
        public static TweenCase DORotate(this Transform tweenObject, Quaternion resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomRotateQuaternion(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes local quaternion rotation of object.
        /// </summary>
        public static TweenCase DOLocalRotate(this Transform tweenObject, Quaternion resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalRotate(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes local angle rotation of object.
        /// </summary>
        public static TweenCase DOLocalRotate(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalRotateAngle(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes position of object.
        /// </summary>
        public static TweenCase DOMove(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomPosition(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes x position of object.
        /// </summary>
        public static TweenCase DOMoveX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomPositionX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes y position of object.
        /// </summary>
        public static TweenCase DOMoveY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomPositionY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes z position of object.
        /// </summary>
        public static TweenCase DOMoveZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomPositionZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes local scale of object.
        /// </summary>
        public static TweenCase DOScale(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomScale(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes local scale of object.
        /// </summary>
        public static TweenCase DOScale(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomScale(tweenObject, new Vector3(resultValue, resultValue, resultValue)).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes x scale of object.
        /// </summary>
        public static TweenCase DOScaleX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomScaleX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes y scale of object.
        /// </summary>
        public static TweenCase DOScaleY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomScaleY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes z scale of object.
        /// </summary>
        public static TweenCase DOScaleZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomScaleZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes local position of object.
        /// </summary>
        public static TweenCase DOLocalMove(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalMove(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes x local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalPositionX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes y local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalPositionY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Changes z local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLocalPositionZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Rotates object face to position.
        /// </summary>
        public static TweenCase DOLookAt(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLookAt(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Rotates 2D object face to position.
        /// </summary>
        public static TweenCase DOLookAt2D(this Transform tweenObject, Vector3 resultValue, TweenCaseTransfomLookAt2D.LookAtType type, float time, bool unscaledTime = false)
        {
            return new TweenCaseTransfomLookAt2D(tweenObject, resultValue, type).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region RectTransform
        /// <summary>
        /// Change anchored position of rectTransform
        /// </summary>
        public static TweenCase DOAnchoredPosition(this RectTransform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseRectTransformAnchoredPosition(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change sizeDelta of rectTransform
        /// </summary>
        public static TweenCase DOSizeScale(this RectTransform tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseRectTransformSizeScale(tweenObject, tweenObject.sizeDelta * resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change sizeDelta of rectTransform
        /// </summary>
        public static TweenCase DOSize(this RectTransform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseRectTransformSizeScale(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region SpriteRenderer
        /// <summary>
        /// Change color of sprite renderer
        /// </summary>
        public static TweenCase DOColor(this SpriteRenderer tweenObject, Color resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseSpriteRendererColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change sprite renderer color alpha
        /// </summary>
        public static TweenCase DOFade(this SpriteRenderer tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseSpriteRendererFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region Image
        /// <summary>
        /// Change color of image
        /// </summary>
        public static TweenCase DOColor(this Image tweenObject, Color resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseImageColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change image color alpha
        /// </summary>
        public static TweenCase DOFade(this Image tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseImageFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region Text
        /// <summary>
        /// Change text font size
        /// </summary>
        public static TweenCase DOFontSize(this Text tweenObject, int resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTextFontSize(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change text color alpha
        /// </summary>
        public static TweenCase DOFade(this Text tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTextFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Change color of text
        /// </summary>
        public static TweenCase DOColor(this Text tweenObject, Color resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseTextColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region CanvasGroup
        /// <summary>
        /// Change alpha value of canvas group
        /// </summary>
        public static TweenCase DOFade(this CanvasGroup tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseCanvasGroupFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region AudioSource
        /// <summary>
        /// Change audio source volume
        /// </summary>
        public static TweenCase DOVolume(this AudioSource tweenObject, float resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseAudioSourceVolume(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion

        #region Other
        public static TweenCase DOAction<T>(this object tweenObject, System.Action<T, T, float> action, T startValue, T resultValue, float time, bool unscaledTime = false)
        {
            return new TweenCaseAction<T>(startValue, resultValue, action).SetTime(time).SetUnscaledMode(unscaledTime);
        }
        #endregion
    }
}