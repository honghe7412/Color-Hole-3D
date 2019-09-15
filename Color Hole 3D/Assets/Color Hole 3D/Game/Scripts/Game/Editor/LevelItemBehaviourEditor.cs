using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelItemBehaviour))]
public class LevelItemBehaviourEditor : Editor
{
    private LevelItemBehaviour behaviour;

    private LevelItemBehaviour.ItemType prevType;

    public void OnEnable()
    {
        behaviour = target as LevelItemBehaviour;

        behaviour.UpdateMaterial();
        prevType = behaviour.Type;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (behaviour.Type != prevType)
        {
            behaviour.UpdateMaterial();
        }

        prevType = behaviour.Type;
    }
}

