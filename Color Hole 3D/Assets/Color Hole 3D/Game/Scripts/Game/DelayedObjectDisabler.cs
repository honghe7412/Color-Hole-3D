using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DelayedObjectDisabler : MonoBehaviour
{
    public float delay;

    private void OnEnable()
    {
        Tween.DelayedCall(delay, () => gameObject.SetActive(false));
    }
}
