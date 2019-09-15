﻿using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public abstract class TweenCase
    {
        public int activeId;

        public float delay;

        public bool isActive;
        public bool isPaused;
        public bool isUnscaled;
        public bool isCompleted;

        public bool isKilling;

        public float state;

        private Ease.EaseFunction easeFunction;

        public Tween.TweenCallback onCompleteCallback;

        public TweenCase()
        {
            SetEasing(Ease.Type.Linear);

            Tween.AddTween(this);
        }

        /// <summary>
        /// Stop and remove tween
        /// </summary>
        public TweenCase Kill()
        {
            if (!isKilling)
            {
                isActive = false;

                Tween.MarkForKilling(this);

                isKilling = true;
            }

            return this;
        }

        /// <summary>
        /// Complete tween
        /// </summary>
        public TweenCase Complete()
        {
            if (isPaused)
                isPaused = false;

            state = 1;

            return this;
        }

        /// <summary>
        /// Pause current coroutine.
        /// </summary>
        public TweenCase Pause()
        {
            isPaused = true;

            return this;
        }

        /// <summary>
        /// Play tween if it was paused.
        /// </summary>
        public TweenCase Resume()
        {
            isPaused = false;

            return this;
        }

        /// <summary>
        /// Interpolate current easing function.
        /// </summary>
        /// <param name="p">Value between 0 and 1</param>
        /// <returns>Interpolated value</returns>
        public float Interpolate(float p)
        {
            return easeFunction.Invoke(p);
        }

        #region Set
        public TweenCase SetUnscaledMode(bool isUnscaled)
        {
            this.isUnscaled = isUnscaled;

            return this;
        }

        /// <summary>
        /// Set tween easing function.
        /// </summary>
        /// <param name="ease">Easing type</param>
        public TweenCase SetEasing(Ease.Type ease)
        {
            easeFunction = Ease.GetFunction(ease);

            return this;
        }

        /// <summary>
        /// Change tween delay.
        /// </summary>
        /// <param name="newDelay">New tween delay.</param>
        public TweenCase SetTime(float newDelay)
        {
            delay = newDelay;

            return this;
        }
        #endregion

        public void NextState(float deltaTime)
        {
            state += deltaTime / delay;
            state = Mathf.Clamp01(state);

            if (state >= 1)
                isCompleted = true;
        }

        /// <summary>
        /// Init function that called when it will completed.
        /// </summary>
        /// <param name="callback">Complete function.</param>
        public TweenCase OnComplete(Tween.TweenCallback callback)
        {
            onCompleteCallback += callback;

            return this;
        }

        public abstract void Invoke();
        public abstract void DefaultComplete();
    }

    public abstract class TweenCaseFunction<TBaseObject, TValue> : TweenCase
    {
        public TBaseObject tweenObject;

        public TValue startValue;
        public TValue resultValue;

        public TweenCaseFunction(TBaseObject tweenObject, TValue resultValue)
        {
            this.tweenObject = tweenObject;
            this.resultValue = resultValue;
        }
    }

    public class TweenCaseDefault : TweenCase
    {
        public override void DefaultComplete() { }
        public override void Invoke() { }
    }

    public class TweenCaseFloat : TweenCase
    {
        public float startValue;
        public float resultValue;

        public TweenFloatCallback callback;

        public TweenCaseFloat(float startValue, float resultValue, TweenFloatCallback callback)
        {
            this.startValue = startValue;
            this.resultValue = resultValue;

            this.callback = callback;
        }

        public override void DefaultComplete()
        {
            callback.Invoke(resultValue);
        }

        public override void Invoke()
        {
            callback.Invoke(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }

        public delegate void TweenFloatCallback(float value);
    }

    public class TweenCaseTransfomRotateAngle : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomRotateAngle(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.eulerAngles;
        }

        public override void DefaultComplete()
        {
            tweenObject.eulerAngles = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.eulerAngles = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomRotateQuaternion : TweenCaseFunction<Transform, Quaternion>
    {
        public TweenCaseTransfomRotateQuaternion(Transform tweenObject, Quaternion resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.rotation;
        }

        public override void DefaultComplete()
        {
            tweenObject.rotation = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.rotation = Quaternion.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalRotate : TweenCaseFunction<Transform, Quaternion>
    {
        public TweenCaseTransfomLocalRotate(Transform tweenObject, Quaternion resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localRotation;
        }

        public override void DefaultComplete()
        {
            tweenObject.localRotation = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localRotation = Quaternion.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalRotateAngle : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLocalRotateAngle(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localEulerAngles;
        }

        public override void DefaultComplete()
        {
            tweenObject.localEulerAngles = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localEulerAngles = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomPosition : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomPosition(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.position;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.position = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomPositionX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.position.x;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(resultValue, tweenObject.position.y, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.y, tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomPositionY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.position.y;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, resultValue, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomPositionZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.position.z;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, tweenObject.position.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, tweenObject.position.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomScale : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomScale(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localScale;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localScale = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomScaleX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localScale.x;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(resultValue, tweenObject.localScale.y, tweenObject.localScale.z);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localScale.y, tweenObject.localScale.z);
        }
    }

    public class TweenCaseTransfomScaleY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localScale.y;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, resultValue, tweenObject.localScale.z);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localScale.z);
        }
    }

    public class TweenCaseTransfomScaleZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localScale.z;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, tweenObject.localScale.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, tweenObject.localScale.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLocalMove : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLocalMove(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localPosition;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localPosition = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalPositionX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localPosition.x;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(resultValue, tweenObject.position.y, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.y, tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomLocalPositionY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localPosition.y;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(tweenObject.position.x, resultValue, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(tweenObject.position.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomLocalPositionZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.localPosition.z;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(tweenObject.position.x, tweenObject.position.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(tweenObject.position.x, tweenObject.position.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLookAt : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLookAt(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.position;
        }

        public override void DefaultComplete()
        {
            tweenObject.LookAt(resultValue);
        }

        public override void Invoke()
        {
            tweenObject.LookAt(Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLookAt2D : TweenCaseFunction<Transform, Vector3>
    {
        public LookAtType type;
        float rotationZ;

        public TweenCaseTransfomLookAt2D(Transform tweenObject, Vector3 resultValue, LookAtType type) : base(tweenObject, resultValue)
        {
            this.type = type;

            startValue = tweenObject.eulerAngles;

            Vector3 different = (resultValue - tweenObject.position);
            different.Normalize();

            rotationZ = (Mathf.Atan2(different.y, different.x) * Mathf.Rad2Deg);

            if (type == LookAtType.Up)
                rotationZ -= 90;
        }

        public override void DefaultComplete()
        {
            tweenObject.LookAt(resultValue);
        }

        public override void Invoke()
        {
            tweenObject.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(startValue.z, rotationZ, Interpolate(state)));
        }

        public enum LookAtType
        {
            Up,
            Right,
            Forward
        }
    }

    public class TweenCaseRectTransformAnchoredPosition : TweenCaseFunction<RectTransform, Vector3>
    {
        public TweenCaseRectTransformAnchoredPosition(RectTransform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.anchoredPosition;
        }

        public override void DefaultComplete()
        {
            tweenObject.anchoredPosition = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.anchoredPosition = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseRectTransformSizeScale : TweenCaseFunction<RectTransform, Vector2>
    {
        public TweenCaseRectTransformSizeScale(RectTransform tweenObject, Vector2 resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.sizeDelta;
        }

        public override void DefaultComplete()
        {
            tweenObject.sizeDelta = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.sizeDelta = Vector2.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseSpriteRendererColor : TweenCaseFunction<SpriteRenderer, Color>
    {
        public TweenCaseSpriteRendererColor(SpriteRenderer tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseSpriteRendererFade : TweenCaseFunction<SpriteRenderer, float>
    {
        public TweenCaseSpriteRendererFade(SpriteRenderer tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color.a;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseImageColor : TweenCaseFunction<Image, Color>
    {
        public TweenCaseImageColor(Image tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseImageFade : TweenCaseFunction<Image, float>
    {
        public TweenCaseImageFade(Image tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color.a;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseImageFill : TweenCaseFunction<Image, float>
    {
        public TweenCaseImageFill(Image tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.fillAmount;
        }

        public override void DefaultComplete()
        {
            tweenObject.fillAmount = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.fillAmount = Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTextFontSize : TweenCaseFunction<Text, int>
    {
        public TweenCaseTextFontSize(Text tweenObject, int resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.fontSize;
        }

        public override void DefaultComplete()
        {
            tweenObject.fontSize = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.fontSize = (int)Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTextFade : TweenCaseFunction<Text, float>
    {
        public TweenCaseTextFade(Text tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color.a;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTextColor : TweenCaseFunction<Text, Color>
    {
        public TweenCaseTextColor(Text tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.color;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseCanvasGroupFade : TweenCaseFunction<CanvasGroup, float>
    {
        public TweenCaseCanvasGroupFade(CanvasGroup tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.alpha;
        }

        public override void DefaultComplete()
        {
            tweenObject.alpha = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.alpha = Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseAudioSourceVolume : TweenCaseFunction<AudioSource, float>
    {
        public TweenCaseAudioSourceVolume(AudioSource tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            startValue = tweenObject.volume;
        }

        public override void DefaultComplete()
        {
            tweenObject.volume = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.volume = Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseAction<T> : TweenCase
    {
        private System.Action<T, T, float> action;

        private T startValue;
        private T resultValue;

        public TweenCaseAction(T startValue, T resultValue, System.Action<T, T, float> action)
        {
            this.startValue = startValue;
            this.resultValue = resultValue;

            this.action = action;
        }

        public override void DefaultComplete()
        {
            action.Invoke(startValue, resultValue, 1);
        }

        public override void Invoke()
        {
            action.Invoke(startValue, resultValue, Interpolate(state));
        }
    }
}