﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Watermelon
{
    public static class SmartSelector
    {
        [MenuItem("GameObject/Selection/Select Children", false, 0)]
        public static void SelectChildren(MenuCommand menuCommand)
        {
            if (CallOnlyOnce(menuCommand))
                return;

            Object[] selectedGameObjects = Selection.objects;
            List<Object> tempObjects = new List<Object>();
            for (int i = 0; i < selectedGameObjects.Length; i++)
            {
                if (selectedGameObjects[i] is GameObject)
                {
                    GameObject tempGameObject = selectedGameObjects[i] as GameObject;
                    if (tempGameObject.transform.childCount > 0)
                    {
                        Transform child = tempGameObject.transform.GetChild(0);
                        if (child != null)
                        {
                            tempObjects.Add(child.gameObject);
                        }
                    }
                }
            }

            if (tempObjects.Count > 0)
                Selection.objects = tempObjects.ToArray();
        }

        [MenuItem("GameObject/Selection/Select Children", true, 0)]
        public static bool SelectChildrenValidation()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Selection/Select Parent", false, 0)]
        public static void SelectParent(MenuCommand menuCommand)
        {
            if (CallOnlyOnce(menuCommand))
                return;

            Object[] selectedGameObjects = Selection.objects;
            List<Object> tempObjects = new List<Object>();
            for (int i = 0; i < selectedGameObjects.Length; i++)
            {
                if (selectedGameObjects[i] is GameObject)
                {
                    GameObject tempGameObject = selectedGameObjects[i] as GameObject;
                    if (tempGameObject.transform.parent != null)
                    {
                        tempObjects.Add(tempGameObject.transform.parent.gameObject);
                    }
                }
            }

            if (tempObjects.Count > 0)
                Selection.objects = tempObjects.ToArray();
        }

        [MenuItem("GameObject/Selection/Select Parent", true, 0)]
        public static bool SelectParentValidation()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Selection/Select Anchor", false, 0)]
        public static void SelectAnchor(MenuCommand menuCommand)
        {
            if (CallOnlyOnce(menuCommand))
                return;

            Object[] selectedGameObjects = Selection.objects;
            List<Object> tempObjects = new List<Object>();
            for (int i = 0; i < selectedGameObjects.Length; i++)
            {
                if (selectedGameObjects[i] is GameObject)
                {
                    GameObject tempGameObject = selectedGameObjects[i] as GameObject;
                    bool hasAdded = false;

                    while (tempGameObject.transform.parent != null)
                    {
                        tempGameObject = tempGameObject.transform.parent.gameObject;

                        MonoBehaviour[] monoBehaviours = tempGameObject.GetComponents<MonoBehaviour>();
                        for (int j = 0; j < monoBehaviours.Length; j++)
                        {
                            if (monoBehaviours[j].GetType().IsDefined(typeof(SelectorAnchorAttribute), false))
                            {
                                tempObjects.Add(tempGameObject);

                                hasAdded = true;

                                break;
                            }
                        }

                        if (hasAdded)
                            break;
                    }
                }
            }

            if (tempObjects.Count > 0)
                Selection.objects = tempObjects.ToArray();
        }

        [MenuItem("GameObject/Selection/Select Anchor", true, 0)]
        public static bool SelectAnchorValidation()
        {
            return Selection.activeGameObject != null;
        }

        private static bool CallOnlyOnce(MenuCommand menuCommand)
        {
            if (Selection.gameObjects.Length > 1)
            {
                if (menuCommand.context != Selection.objects[0])
                {
                    return true;
                }
            }

            return false;
        }
    }
}