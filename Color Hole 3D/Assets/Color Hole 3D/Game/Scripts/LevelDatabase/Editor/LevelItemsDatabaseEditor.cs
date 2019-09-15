#pragma warning disable 0414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Watermelon;

[CustomEditor(typeof(LevelItemsDatabase))]
public class LevelItemsDatabaseEditor : Editor
{
    private LevelItemsDatabase itemsDatabase;

    private GUIStyle addCenteredStyle;
    private GUIStyle lableStyle;
    private GUIContent addGUIContent;
    private Color selectedColor = new Color(0.75f, 0.75f, 0.75f);

    private bool isInited = false;

    private SerializedProperty ItemsProperty;
    private SerializedProperty selectedObstacleProperty;

    private const string itemsPropertyName = "items";

    private readonly float branchPairIconSize = 50f;
    private readonly float treeHorizontalSpace = 10f;
    private readonly float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
    private readonly float singleLineHeight = EditorGUIUtility.singleLineHeight;
    private float currentVerticalSpacing = 120f;

    private Level.Item[] partTypes;

    private string requiredTypes;

    private void OnEnable()
    {
        itemsDatabase = (LevelItemsDatabase)target;

        ItemsProperty = serializedObject.FindProperty(itemsPropertyName);

        Init();
    }

    private void Init()
    {
        Level.Item[] partTypes = System.Enum.GetValues(typeof(Level.Item)).Cast<Level.Item>().ToArray();

        List<Level.Item> levelParts = new List<Level.Item>(partTypes);

        for (int i = 0; i < itemsDatabase.Items.Length; i++)
        {
            if (itemsDatabase.Items[i].Prefab != null)
                levelParts.Remove(itemsDatabase.Items[i].PartType);
        }

        int levelPartsCount = levelParts.Count;
        requiredTypes = "";
        for (int i = 0; i < levelPartsCount; i++)
        {
            requiredTypes += levelParts[i].ToString();

            if (i != levelPartsCount - 1)
                requiredTypes += "\n";
        }
    }

    private void InitStyles()
    {
        if (isInited)
            return;

        addGUIContent = new GUIContent("Drag items here");

        addCenteredStyle = GUI.skin.box;
        addCenteredStyle.alignment = TextAnchor.MiddleCenter;
        addCenteredStyle.fontSize = 15;
        addCenteredStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);

        lableStyle = new GUIStyle();
        lableStyle.fontSize = 13;
        lableStyle.fontStyle = FontStyle.Bold;
        lableStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

        isInited = true;
    }

    private int selectedElement = -1;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InitStyles();

        Rect boxRect = GUILayoutUtility.GetRect(addGUIContent, addCenteredStyle, GUILayout.Height(48), GUILayout.ExpandWidth(true));
        GUI.Box(boxRect, "Drag items here", addCenteredStyle);

        if (!string.IsNullOrEmpty(requiredTypes))
        {
            EditorGUILayout.LabelField("Missing items:", lableStyle);
            EditorGUILayout.HelpBox(requiredTypes, MessageType.None, true);
            GUILayout.Space(5);
        }

        EditorGUILayout.LabelField("Items:", lableStyle);

        if (itemsDatabase.Items.IsNullOrEmpty())
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(50));
            EditorGUILayout.LabelField("There's no items yet");
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(264));

            for (int i = 0; i < itemsDatabase.Items.Length; i++)
            {
                int index = i;
                bool isLevelSelected = selectedElement == i;

                if (isLevelSelected)
                    GUI.color = selectedColor;

                Rect clickRect = (Rect)EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.LabelField("Item " + i.ToString("00") + " " + itemsDatabase.Items[i].PartType.ToString());

                GUILayout.FlexibleSpace();

                GUILayout.Space(5);

                if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                {
                    if (isLevelSelected)
                    {
                        Unselect();
                    }
                    else
                    {
                        Select(i);
                    }

                    return;
                }

                if (isLevelSelected)
                    GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();

                if (isLevelSelected)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.PropertyField(selectedObstacleProperty, true);
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        DraggingAndDropping(boxRect);

        serializedObject.ApplyModifiedProperties();
    }

    private void Select(int index)
    {
        if (selectedElement != -1)
        {
            Unselect();
        }

        Init();

        selectedElement = index;

        selectedObstacleProperty = ItemsProperty.GetArrayElementAtIndex(selectedElement).FindPropertyRelative("prefab");
    }

    private void Unselect()
    {
        selectedElement = -1;
    }

    private void DraggingAndDropping(Rect dropArea)
    {
        // Cache the current event.
        Event currentEvent = Event.current;

        //if (currentEvent.type == EventType.DragExited)
        //{
        //    displayWarning = false;
        //}

        // If the drop area doesn't contain the mouse then return.
        if (!dropArea.Contains(currentEvent.mousePosition))
            return;


        switch (currentEvent.type)
        {
            // If the mouse is dragging something...
            case EventType.DragUpdated:

                // ... change whether or not the drag *can* be performed by changing the visual mode of the cursor based on the IsDragValid function.
                DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Link : DragAndDropVisualMode.Rejected;

                // Make sure the event isn't used by anything else.
                currentEvent.Use();

                break;

            // If the mouse was dragging something and has released...
            case EventType.DragPerform:

                // ... accept the drag event.
                DragAndDrop.AcceptDrag();

                bool reinitRequired = false;
                serializedObject.Update();
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    GameObject dragObject = DragAndDrop.objectReferences[i] as GameObject;
                    if (dragObject != null)
                    {
                        LevelItemBehaviour levelItem = dragObject.GetComponent<LevelItemBehaviour>();
                        if (levelItem)
                        {
                            int obstacleDatabaseIndex = System.Array.FindIndex(itemsDatabase.Items, x => x.PartType == levelItem.Item);
                            if (obstacleDatabaseIndex == -1)
                            {
                                ItemsProperty.InsertArrayElementAtIndex(ItemsProperty.arraySize);
                                obstacleDatabaseIndex = ItemsProperty.arraySize - 1;
                            }

                            SerializedProperty insertedProperty = ItemsProperty.GetArrayElementAtIndex(obstacleDatabaseIndex);

                            insertedProperty.FindPropertyRelative("partType").intValue = (int)levelItem.Item;
                            insertedProperty.FindPropertyRelative("prefab").objectReferenceValue = dragObject;

                            reinitRequired = true;
                        }
                    }
                }
                serializedObject.ApplyModifiedProperties();

                if (reinitRequired)
                    Init();

                // Make sure the event isn't used by anything else.
                currentEvent.Use();

                break;
        }

    }

    private bool IsDragValid()
    {
        // Go through all the objects being dragged...
        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
        {
            if (PrefabUtility.GetPrefabAssetType(DragAndDrop.objectReferences[i]) != PrefabAssetType.Regular)
            {
                return false;
            }
        }

        // If none of the dragging objects returned that the drag was invalid, return that it is valid.
        return true;
    }
}
