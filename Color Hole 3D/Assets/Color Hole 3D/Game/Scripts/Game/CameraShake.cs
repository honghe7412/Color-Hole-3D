using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

/// <summary>
/// Easy to setup camera shake script.
/// </summary>
public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// The intensity of the shake.
    /// </summary>
    public float magnitude;

    /// <summary>
    /// Roughness of the shake.
    /// </summary>
    public float roughness;

    /// <summary>
    /// Duration in seconds of fade in.
    /// </summary>
    public float fadeInDuration;

    /// <summary>
    /// Duration in seconds of fade out.
    /// </summary>
    public float fadeOutDuration;

    /// <summary>
    /// How much influence this shake has over the local position axes of the camera.
    /// </summary>
    public Vector3 positionInfluence = new Vector3(1f, 1f, 1f);

    /// <summary>
    /// How much influence this shake has over the local rotation axes of the camera.
    /// </summary>
    public Vector3 rotationInfluence = new Vector3(1f, 1f, 1f);


    private Transform transformRef;
    private Vector3 noise;
    private Vector3 offsetDelta;
    private Vector3 positionOffset;
    private Vector3 rotationOffset;

    private bool isActive;
    private bool onFadeInState;
    private bool isLooping;

    private float tick;
    private float currentShakeIntensivity;

    private void Awake()
    {
        isActive = false;
        transformRef = transform;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // !!! IMPORTANT: object with Camera component should be a child of object which moves camera for correct work of script //
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        if (isActive)
        {
            noise.x = Mathf.PerlinNoise(tick, 0) - 0.5f;
            noise.y = Mathf.PerlinNoise(0, tick) - 0.5f;
            noise.z = Mathf.PerlinNoise(tick, tick) - 0.5f;

            if (onFadeInState || isLooping)
            {
                // applying fade in effect
                if (fadeInDuration > 0 && currentShakeIntensivity < 1)
                {
                    currentShakeIntensivity += Time.deltaTime / fadeInDuration;
                }
                // if fade out value initialized - disable looping and starting fade out
                else if (!isLooping)
                {
                    onFadeInState = false;
                }

                tick += Time.deltaTime * roughness;
            }
            // fade out state
            else
            {
                if (fadeOutDuration > 0f)
                {
                    currentShakeIntensivity -= Time.deltaTime / fadeOutDuration;
                }
                else
                {
                    currentShakeIntensivity = 0f;
                }

                tick += Time.deltaTime * roughness * currentShakeIntensivity;
            }

            offsetDelta = noise * magnitude * currentShakeIntensivity;

            positionOffset = MultiplyVectors(offsetDelta * 0.2f, positionInfluence);
            rotationOffset = MultiplyVectors(offsetDelta, rotationInfluence);

            transformRef.localPosition =  positionOffset;
            transformRef.localEulerAngles =  rotationOffset;

            if (currentShakeIntensivity <= 0)
            {
                isActive = false;

                transformRef.localPosition = Vector3.zero;
                transformRef.localEulerAngles = Vector3.zero;
                return;
            }
        }
    }

    /// <summary>
    /// Shake the camera once, fading in and out over a specified durations.
    /// </summary>
    /// <param name="magnitudeOverride">The intensity of the shake.</param>
    /// <param name="roughnessOverride">Roughness of the shake. Lower values are smoother, higher values are more jarring.</param>
    /// <param name="fadeInTimeOverride">Duration in seconds of fade in.</param>
    /// <param name="fadeOutTimeOverride">Duration in seconds of fade out.</param>
    public void ShakeOnce(float magnitudeOverride = -1, float roughnessOverride = -1, float fadeInTimeOverride = -1, float fadeOutTimeOverride = -1)
    {
        tick = Random.Range(-100, 100);

        this.magnitude = magnitudeOverride != -1 ? magnitudeOverride : this.magnitude;
        this.roughness = roughnessOverride != -1 ? roughnessOverride : this.roughness;

        fadeInDuration = fadeInTimeOverride != -1 ? fadeInTimeOverride : this.fadeInDuration;
        fadeOutDuration = fadeOutTimeOverride != -1 ? fadeOutTimeOverride : this.fadeOutDuration;

        onFadeInState = true;
        isLooping = false;
        isActive = true;
    }



    /// <summary>
    /// Starts looping shake with fade in over the given number of seconds.
    /// </summary>
    /// <param name="fadeInTime">Override duration of the fade in.</param>
    public void StartShaking(float fadeInTime = -1)
    {
        tick = Random.Range(-100, 100);

        if (fadeInTime == 0)
            currentShakeIntensivity = 1;

        fadeInDuration = fadeInTime != -1 ? fadeInTime : fadeInDuration;

        onFadeInState = true;
        isLooping = true;
        isActive = true;
    }


    /// <summary>
    /// Stops shaking with fade out over the given number of seconds.
    /// </summary>
    /// <param name="fadeOutTime">Override duration of the fade out.</param>
    public void StopShaking(float fadeOutTime = -1)
    {
        if (fadeOutTime == 0)
            currentShakeIntensivity = 0;

        fadeOutDuration = fadeOutTime != -1 ? fadeOutTime : fadeOutDuration;

        onFadeInState = false;
        isLooping = false;
        isActive = true;
    }

#if UNITY_EDITOR
    [Button("Shake Once")]
    public void Shake()
    {
        ShakeOnce();
    }

    [Button("Start shake")]
    public void StartShake()
    {
        StartShaking();
    }

    [Button("Stop shake")]
    public void StopShake()
    {
        StopShaking();
    }
#endif

    private Vector3 MultiplyVectors(Vector3 v, Vector3 w)
    {
        v.x *= w.x;
        v.y *= w.y;
        v.z *= w.z;

        return v;
    }
}