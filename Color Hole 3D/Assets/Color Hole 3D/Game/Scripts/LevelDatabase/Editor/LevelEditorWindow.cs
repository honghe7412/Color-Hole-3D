using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Watermelon;
using System.Collections.Generic;

public class LevelEditorWindow : EditorWindow
{
    private static Pagination pagination = new Pagination(10, 5);

    private LevelsDatabase levelsDatabase;
    private LevelItemsDatabase itemsDatabase;

    private SerializedObject levelDatabaseSerializedObject;
    private SerializedProperty levelsSerializedProperty;
    private static int selectedLevelID = -1;

    private bool levelLoaded = false;

    private GameObject objectsContainer;

    private SerializedProperty levelNameProperty;
    private Color selectedColor = new Color(0.75f, 0.75f, 0.75f);
    private Color redColor = new Color(1f, 0.8f, 0.8f);
    private Color defaultGUIColor;
    private GUIStyle lableStyle;
    private Transform cameraTransform;

    private const string LEVELS_PROPERTY_NAME = "levels";

    private const string TITLE = "Level Editor";
    private const string SCENE_NAME = "LevelEditor";
    private const string SCENES_PATH = "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Game/Scenes/";

    private const string OBJECTS_CONTAINER_NAME = "[EDITOR]";
    private const string LEVELS_DATABASE_PATH = "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Content/LevelSystem/";

    private const string LEVEL_NAME_PROPERTY_NAME = "editorName";


    private bool currentStageIsFirst = true;

    [MenuItem("Tools/Project/Level Editor")]
    static void ShowWindow()
    {
        LevelEditorWindow window = (LevelEditorWindow)EditorWindow.GetWindow(typeof(LevelEditorWindow));
        window.titleContent = new GUIContent("Level Editor");
        window.Show();
    }

    private void Init()
    {
        if (objectsContainer == null)
            objectsContainer = new GameObject(OBJECTS_CONTAINER_NAME);
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        Disable();

        if (selectedLevelID != -1)
            Init();
    }

    private void Disable()
    {
        DestroyImmediate(objectsContainer);
        objectsContainer = null;
    }

    private void OnEnable()
    {
        defaultGUIColor = GUI.color;

        lableStyle = new GUIStyle();
        lableStyle.fontSize = 13;
        lableStyle.fontStyle = FontStyle.Bold;
        lableStyle.normal.textColor = new Color(0.2f, 0.2f, 0.2f);

        selectedLevelID = -1;

        levelsDatabase = EditorUtils.GetAsset<LevelsDatabase>();
        if (levelsDatabase != null)
        {
            levelDatabaseSerializedObject = new SerializedObject(levelsDatabase);

            InitItemsDatabase();

            levelsSerializedProperty = levelDatabaseSerializedObject.FindProperty(LEVELS_PROPERTY_NAME);

            pagination.Init(levelsSerializedProperty.arraySize);
        }
        else
        {
            Debug.Log("Levels db is null.");
        }

        EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;

        cameraTransform = Camera.main.transform;
    }

    private void OnDisable()
    {
        if (levelLoaded)
        {
            LevelItemBehaviour[] existingItems = FindObjectsOfType<LevelItemBehaviour>();

            if (existingItems.Length != 0 && levelsDatabase.Levels[selectedLevelID].Items.Length != existingItems.Length && EditorUtility.DisplayDialog("Save level?", "Existing items (" + existingItems.Length + ") will not be saved!", "Save", "Cancel"))
            {
                SaveObjects();

                DestroyObjects();
            }
            else
            {
                DestroyObjects();
            }
        }

        Disable();

        EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
    }

    public void OnGUI()
    {
        if (levelsDatabase == null)
        {
            EditorGUILayout.HelpBox("Levels Database can't be found.", MessageType.Error, true);

            if (GUILayout.Button("Create Levels Database"))
            {
                levelsDatabase = EditorUtils.CreateAsset<LevelsDatabase>(LEVELS_DATABASE_PATH + "Levels Database", true);

                OnEnable();
            }

            return;
        }


        int arrayList = levelsSerializedProperty.arraySize;
        if (arrayList == 0)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.HelpBox("Levels Database is empty.", MessageType.Warning, true);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Level"))
            {
                AddNewLevel();

                return;
            }

            return;
        }


        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(264));
        int paginationEnd = pagination.GetMaxElementNumber();
        for (int i = pagination.GetMinElementNumber(); i < paginationEnd; i++)
        {
            int index = i;
            bool isLevelSelected = selectedLevelID == i;

            if (isLevelSelected)
                GUI.color = selectedColor;

            Rect clickRect = (Rect)EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.LabelField(levelsDatabase.Levels[i].EditorName + "   |   Items: " + (levelsDatabase.Levels[i].Items != null ? levelsDatabase.Levels[i].Items.Length : 0));

            GUILayout.FlexibleSpace();

            GUI.color = Color.grey;
            if (GUILayout.Button("=", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
            {
                GenericMenu menu = new GenericMenu();

                if (isLevelSelected)
                {
                    menu.AddItem(new GUIContent("Unselect"), false, delegate
                    {
                        Unselect();
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Select"), false, delegate
                    {
                        Select(index);
                    });
                }

                menu.AddItem(new GUIContent("Remove"), false, delegate
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "This level will be removed!", "Remove", "Cancel"))
                    {
                        if (isLevelSelected)
                        {
                            Unselect();
                        }

                        levelsSerializedProperty.DeleteArrayElementAtIndex(index);

                        pagination.Init(levelsSerializedProperty.arraySize);

                        return;
                    }
                });

                menu.AddSeparator("");

                if (i > 0)
                {
                    menu.AddItem(new GUIContent("Move up"), false, delegate
                    {
                        levelsSerializedProperty.MoveArrayElement(index, index - 1);

                        if (isLevelSelected)
                            selectedLevelID -= 1;
                        else if (selectedLevelID == index - 1)
                            selectedLevelID = index;
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Move up"));
                }

                if (i + 1 < arrayList)
                {
                    menu.AddItem(new GUIContent("Move down"), false, delegate
                    {
                        levelsSerializedProperty.MoveArrayElement(index, index + 1);

                        if (isLevelSelected)
                            selectedLevelID += 1;
                        else if (selectedLevelID == index + 1)
                            selectedLevelID = index;
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Move down"));
                }

                menu.ShowAsContext();
            }

            GUI.color = Color.white;

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

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndHorizontal();

        pagination.DrawPagination();

        EditorGUILayout.Space();
        if (levelsDatabase == null)
        {
            EditorGUILayout.HelpBox("Level Database is not found.", MessageType.Error, true);

            if (GUILayout.Button("Create Levels Database"))
            {
                levelsDatabase = EditorUtils.GetAsset<LevelsDatabase>();

                return;
            }
        }
        else
        {
            UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.GetActiveScene();

            if (selectedLevelID != -1)
            {
                bool rightSceneSelected = currentScene.name == "LevelEditor";

                EditorGUILayout.PropertyField(levelNameProperty);

                GUILayout.Space(5);

                if (!rightSceneSelected)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("LevelEditor scene is required for editing.", MessageType.Warning);
                    if (GUILayout.Button("Open", GUILayout.Height(40f)))
                    {
                        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                        EditorSceneManager.OpenScene(SCENES_PATH + "LevelEditor.unity");

                        LoadObjects();

                        cameraTransform = Camera.main.transform;

                        cameraTransform.position = CameraController.STAGE_ONE_CAM_POS_EDITOR;
                        currentStageIsFirst = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = false;
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Objects management:", lableStyle);

                EditorGUILayout.BeginHorizontal();

                if (levelsDatabase.Levels[selectedLevelID].Items != null && levelsDatabase.Levels[selectedLevelID].Items.Length == 0)
                {
                    GUI.enabled = false;
                }

                GUI.color = redColor;
                if (GUILayout.Button("Clear Objects", GUILayout.MinWidth(80f), GUILayout.MaxWidth(90f), GUILayout.Height(30f)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Level parts will be removed!", "Remove", "Cancel"))
                    {
                        ClearObjects();
                    }
                }
                GUI.color = defaultGUIColor;

                GUILayout.FlexibleSpace();



                if (GUILayout.Button("Load Objects", GUILayout.MinWidth(90f), GUILayout.MaxWidth(150f), GUILayout.Height(30f)))
                {
                    LoadObjects();
                }

                GUI.enabled = true;


                if (GUILayout.Button("Save Objects", GUILayout.MinWidth(90f), GUILayout.MaxWidth(150f), GUILayout.Height(30f)))
                {
                    SaveObjects();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Camera:", lableStyle);

                EditorGUILayout.BeginHorizontal();


                if (currentStageIsFirst)
                    GUI.color = selectedColor;

                if (GUILayout.Button("Stage One Position", GUILayout.MinWidth(90f), GUILayout.MaxWidth(150f), GUILayout.Height(30f)))
                {
                    cameraTransform.position = CameraController.STAGE_ONE_CAM_POS_EDITOR;
                    currentStageIsFirst = true;
                }

                GUI.color = defaultGUIColor;

                if (!currentStageIsFirst)
                    GUI.color = selectedColor;

                if (GUILayout.Button("Stage Two Position", GUILayout.MinWidth(90f), GUILayout.MaxWidth(150f), GUILayout.Height(30f)))
                {
                    cameraTransform.position = CameraController.STAGE_TWO_CAM_POS_EDITOR;
                    currentStageIsFirst = false;
                }

                GUI.color = defaultGUIColor;

                EditorGUILayout.EndHorizontal();

                if (!rightSceneSelected)
                    GUI.enabled = true;
            }
            else
            {
                if (GUILayout.Button("Add Level", GUILayout.Height(40f)))
                {
                    AddNewLevel();

                    return;
                }
            }
        }

        levelDatabaseSerializedObject.ApplyModifiedProperties();
    }



    private void AddNewLevel()
    {
        int arraySize = levelsSerializedProperty.arraySize;

        levelsSerializedProperty.arraySize++;

        pagination.Init(arraySize + 1);

        levelsSerializedProperty.GetArrayElementAtIndex(levelsSerializedProperty.arraySize - 1).FindPropertyRelative("editorName").stringValue = "Level " + levelsSerializedProperty.arraySize;

        levelsSerializedProperty.serializedObject.ApplyModifiedProperties();
        levelsDatabase.Levels[levelsDatabase.Levels.Length - 1].Items = null;
    }

    private void InitItemsDatabase()
    {
        if (itemsDatabase == null)
        {
            itemsDatabase = EditorUtils.GetAsset<LevelItemsDatabase>();
            itemsDatabase.Init();
        }
    }

    private void Select(int index)
    {
        if (selectedLevelID != -1)
        {
            Unselect();
        }

        selectedLevelID = index;

        SerializedProperty levelProperty = levelsSerializedProperty.GetArrayElementAtIndex(selectedLevelID);
        levelNameProperty = levelProperty.FindPropertyRelative(LEVEL_NAME_PROPERTY_NAME);

        if (EditorSceneManager.GetActiveScene().name.Equals("LevelEditor"))
        {
            Init();
            LoadObjects();

            if (levelsDatabase.Levels[index].Items.IsNullOrEmpty())
            {
                cameraTransform.position = CameraController.STAGE_ONE_CAM_POS_EDITOR;
                currentStageIsFirst = true;
            }
        }

    }

    private void Unselect()
    {
        selectedLevelID = -1;

        if (levelLoaded)
        {
            DestroyObjects();

            Disable();

            levelLoaded = false;
        }
    }

    private void DestroyObjects()
    {
        if (EditorSceneManager.GetActiveScene().name != "LevelEditor")
            return;

        //Clearing level objects
        LevelItemBehaviour[] items = FindObjectsOfType<LevelItemBehaviour>();
        for (int i = 0; i < items.Length; i++)
        {
            DestroyImmediate(items[i].gameObject);
        }


        // Clearing empty objects
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();

        for (int i = 0; i < allGameObjects.Length; i++)
        {
            if (allGameObjects[i].transform.parent == null && !IsEditorObjectName(allGameObjects[i].name))
            {
                DestroyImmediate(allGameObjects[i].gameObject);
            }
        }
    }

    private bool IsEditorObjectName(string name)
    {
        return name[0] == '[' && name[name.Length - 1] == ']';
    }

    private void ClearObjects()
    {
        if (selectedLevelID != -1)
        {
            DestroyObjects();

            levelsDatabase.Levels[selectedLevelID].Items = new Level.ItemSave[0];
        }
    }

    private void LoadObjects()
    {
        if (selectedLevelID != -1)
        {
            InitItemsDatabase();

            // getting all existing items on level
            LevelItemBehaviour[] existingItems = FindObjectsOfType<LevelItemBehaviour>();

            Level.ItemSave[] levelParts = levelsDatabase.Levels[selectedLevelID].Items;
            if (levelParts != null)
            {
                // if existing items amount is not equal to those that will be loaded
                // existing items will be overrided and current changes will be lost
                // so asking confirmation for this action
                if (existingItems.Length != 0 && existingItems.Length != levelParts.Length)
                {
                    if (EditorUtility.DisplayDialog("Do you want to override existing items?", "Saved items amount is not equal to existing. Existing items will be overrided, could be lost data.", "Override", "Cancel"))
                    {
                        LoadObjectsWithOverride(levelParts);
                    }
                }
                else
                {
                    LoadObjectsWithOverride(levelParts);
                }
            }

            levelLoaded = true;

            Debug.Log("[LevelEditor] Loaded " + (levelParts != null ? levelParts.Length : 0) + " objects.");
        }
    }

    private void LoadObjectsWithOverride(Level.ItemSave[] levelParts)
    {
        DestroyObjects();

        for (int i = 0; i < levelParts.Length; i++)
        {
            LevelItem item = itemsDatabase.GetItem(levelParts[i].Item);

            if (item != null)
            {
                GameObject prefab = Instantiate(item.Prefab);
                prefab.transform.position = levelParts[i].Position;
                prefab.transform.eulerAngles = levelParts[i].Rotation;

                LevelItemBehaviour itemBehaviour = prefab.GetComponent<LevelItemBehaviour>();
                itemBehaviour.transform.localScale = levelParts[i].Scale;
                itemBehaviour.Init(levelParts[i].Type);

                prefab.transform.SetParent(objectsContainer.transform);

                prefab.isStatic = levelParts[i].IsStatic;
            }
            else
            {
                Debug.LogError("[LevelEditor] Can't find item: \"" + levelParts[i].Item + "\"");
            }
        }
    }

    private void SaveObjects()
    {
        if (selectedLevelID != -1)
        {
            LevelItemBehaviour[] items = FindObjectsOfType<LevelItemBehaviour>();

            items = items.OrderBy(x => x.transform.position.z).ToArray();

            Level.ItemSave[] levelParts = new Level.ItemSave[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                levelParts[i] = new Level.ItemSave(items[i].Item, items[i].Type, items[i].transform.position, items[i].transform.eulerAngles, items[i].transform.lossyScale, items[i].gameObject.isStatic);
            }

            levelsDatabase.Levels[selectedLevelID].Items = levelParts;

            levelLoaded = true;

            EditorUtility.SetDirty(levelsDatabase);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[LevelEditor] Saved " + levelParts.Length + " objects.");
        }
    }


    [MenuItem("GameObject/LevelEditor/To First Stage Center", false, 0)]
    public static void SetFirstStagePosition(MenuCommand menuCommand)
    {

        UnityEngine.Object[] selectedGameObjects = Selection.objects;
        List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();

        for (int i = 0; i < selectedGameObjects.Length; i++)
        {
            if (selectedGameObjects[i] is GameObject)
            {
                GameObject tempGameObject = selectedGameObjects[i] as GameObject;
                tempGameObject.transform.position = new Vector3(2.75f, 0f, 4.75f);
            }
        }

        if (tempObjects.Count > 0)
            Selection.objects = tempObjects.ToArray();
    }

    [MenuItem("GameObject/LevelEditor/To First Stage Center", true, 0)]
    public static bool SetFirstStagePositionValidation()
    {
        return Selection.activeGameObject != null;
    }

    [MenuItem("GameObject/LevelEditor/To Second Stage Center", false, 0)]
    public static void SetSecondStagePosition(MenuCommand menuCommand)
    {
        UnityEngine.Object[] selectedGameObjects = Selection.objects;
        List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();

        for (int i = 0; i < selectedGameObjects.Length; i++)
        {
            if (selectedGameObjects[i] is GameObject)
            {
                GameObject tempGameObject = selectedGameObjects[i] as GameObject;
                tempGameObject.transform.position = new Vector3(2.75f, 0f, 20.25f);
            }
        }

        if (tempObjects.Count > 0)
            Selection.objects = tempObjects.ToArray();
    }

    [MenuItem("GameObject/LevelEditor/To Second Stage Center", true, 0)]
    public static bool SetSecondStagenValidation()
    {
        return Selection.activeGameObject != null;
    }
}
