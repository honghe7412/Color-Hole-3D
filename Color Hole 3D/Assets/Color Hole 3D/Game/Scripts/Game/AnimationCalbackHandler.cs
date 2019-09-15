using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationCalbackHandler : MonoBehaviour
{
    public UnityEvent callback;

    public void AnimationCallback()
    {
        callback.Invoke();
    }
}