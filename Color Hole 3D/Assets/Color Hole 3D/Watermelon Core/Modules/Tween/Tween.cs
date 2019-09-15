using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Bugs
//Wrong unscaled time if tween called from start

namespace Watermelon
{
    /// <summary>
    /// Base tween class.
    /// </summary>
    public class Tween : MonoBehaviour
    {
        private static Tween instance;

        /// <summary>
        /// List of all tweens.
        /// </summary>
        private static TweenCase[] tweens = new TweenCase[DEFAULT_MAX_TWEENS];
        public TweenCase[] Tweens
        {
            get { return tweens; }
        }

        private static int tweensCount;

        private static List<TweenCase> killingTweens = new List<TweenCase>();

        private static bool hasActiveTweens = false;

        private static bool requiresActiveReorganization = false;
        private static int reorganizeFromID = -1;
        private static int maxActiveLookupID = -1;

        private const int DEFAULT_MAX_TWEENS = 100;

        /// <summary>
        /// Create tween instance.
        /// </summary>
        public static void Init()
        {
            if (instance == null)
            {
                GameObject tweenGO = new GameObject("[Tween]");
                instance = tweenGO.AddComponent<Tween>();

                DontDestroyOnLoad(tweenGO);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[Tween]: Tween already exist!");
            }
#endif
        }

        public static void AddTween(TweenCase tween)
        {
            if (requiresActiveReorganization)
                ReorganizeActiveTweens();

            tween.isActive = true;
            tween.activeId = (maxActiveLookupID = tweensCount);

            tweens[tweensCount] = tween;
            tweensCount++;

            hasActiveTweens = true;
        }

        public static void PauseAll()
        {
            for (int i = 0; i < tweensCount; i++)
            {
                TweenCase tween = tweens[i];
                if (tween != null)
                {
                    tween.Pause();
                }
            }
        }

        public static void ResumeAll()
        {
            for (int i = 0; i < tweensCount; i++)
            {
                TweenCase tween = tweens[i];
                if (tween != null)
                {
                    tween.Resume();
                }
            }
        }

        public static void RemoveAll()
        {
            for (int i = 0; i < tweensCount; i++)
            {
                TweenCase tween = tweens[i];
                if (tween != null)
                {
                    tween.Kill();
                }
            }
        }

        private void Update()
        {
            if (!hasActiveTweens)
                return;

            if (requiresActiveReorganization)
                ReorganizeActiveTweens();

            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            for (int i = 0; i < tweensCount; i++)
            {
                TweenCase tween = tweens[i];
                if (tween != null)
                {
                    if (tween.isActive && !tween.isPaused)
                    {
                        if (!tween.isUnscaled)
                        {
                            if (Time.timeScale == 0)
                                continue;

                            tween.NextState(deltaTime);
                        }
                        else
                        {
                            tween.NextState(unscaledDeltaTime);
                        }

                        tween.Invoke();

                        if (tween.isCompleted)
                        {
                            tween.DefaultComplete();

                            if (tween.onCompleteCallback != null)
                                tween.onCompleteCallback.Invoke();

                            tween.Kill();
                        }
                    }
                }
            }

            int killingTweensCount = killingTweens.Count - 1;
            for (int i = killingTweensCount; i > -1; i--)
            {
                RemoveActiveTween(killingTweens[i]);
            }
            killingTweens.Clear();
        }

        private static void ReorganizeActiveTweens()
        {
            if (tweensCount <= 0)
            {
                maxActiveLookupID = -1;
                reorganizeFromID = -1;
                requiresActiveReorganization = false;

                return;
            }

            if (reorganizeFromID == maxActiveLookupID)
            {
                maxActiveLookupID--;
                reorganizeFromID = -1;
                requiresActiveReorganization = false;

                return;
            }

            int defaultOffset = 1;
            int tweensTempCount = maxActiveLookupID + 1;

            maxActiveLookupID = reorganizeFromID - 1;

            for (int i = reorganizeFromID + 1; i < tweensTempCount; i++)
            {
                TweenCase tween = tweens[i];
                if (tween != null)
                {
                    tween.activeId = (maxActiveLookupID = i - defaultOffset);

                    tweens[i - defaultOffset] = tween;
                    tweens[i] = null;
                }
                else
                {
                    defaultOffset++;
                }

                //Debug.Log("MaxActiveLookupID: " + maxActiveLookupID + "; ReorganizeFromID: " + reorganizeFromID + "; Offset: " + defaultOffset + ";");
            }

            requiresActiveReorganization = false;
            reorganizeFromID = -1;
        }

        public static void MarkForKilling(TweenCase tween)
        {
            killingTweens.Add(tween);
        }

        private void RemoveActiveTween(TweenCase tween)
        {
            int activeId = tween.activeId;
            tween.activeId = -1;

            requiresActiveReorganization = true;

            if (reorganizeFromID == -1 || reorganizeFromID > activeId)
            {
                reorganizeFromID = activeId;
            }

            tweens[activeId] = null;

            tweensCount--;
            hasActiveTweens = (tweensCount > 0);
        }

        #region DeleyCall
        /// <summary>
        /// Delayed call of delegate.
        /// </summary>
        /// <param name="callback">Callback to call.</param>
        /// <param name="delay">Delay in seconds.</param>
        public static TweenCase DelayedCall(float delay, TweenCallback callback, bool unscaledTime = false)
        {
            return new TweenCaseDefault().SetTime(delay).SetUnscaledMode(unscaledTime).OnComplete(callback);
        }
        #endregion

        /// <summary>
        /// Interpolate float value
        /// </summary>
        public static TweenCase DoFloat(float startValue, float resultValue, float time, TweenCaseFloat.TweenFloatCallback callback, bool unscaledTime = false)
        {
            return new TweenCaseFloat(startValue, resultValue, callback).SetTime(time).SetUnscaledMode(unscaledTime);
        }

        /// <summary>
        /// Call function in next frame
        /// </summary>
        public static void NextFrame(TweenCallback callback)
        {
            instance.StartCoroutine(NextFrameCoroutine(callback));
        }
        private static IEnumerator NextFrameCoroutine(TweenCallback callback)
        {
            yield return null;

            callback.Invoke();
        }

        /// <summary>
        /// Call function in nexe fixed frame
        /// </summary>
        /// <param name="callback"></param>
        public static void NextFixedFrame(TweenCallback callback)
        {
            instance.StartCoroutine(NextFixedFrameCoroutine(callback));
        }
        private static IEnumerator NextFixedFrameCoroutine(TweenCallback callback)
        {
            yield return new WaitForFixedUpdate();

            callback.Invoke();
        }

        /// <summary>
        /// Invoke coroutine from non-monobehavior script
        /// </summary>
        public static Coroutine InvokeCoroutine(IEnumerator enumerator)
        {
            return instance.StartCoroutine(enumerator);
        }

        /// <summary>
        /// Stop custom coroutine
        /// </summary>
        public static void StopCustomCoroutine(Coroutine coroutine)
        {
            instance.StopCoroutine(coroutine);
        }

        public delegate void TweenCallback();
    }
}
