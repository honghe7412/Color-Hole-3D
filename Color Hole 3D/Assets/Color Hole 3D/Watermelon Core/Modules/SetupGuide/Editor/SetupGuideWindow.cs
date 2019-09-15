using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace Watermelon
{
    public class SetupGuideWindow : EditorWindow
    {
        private const string DOCUMENTATION_URL = @"https://drive.google.com/open?id=1RLLzv5l00YveB9LGlkiUGN3Teoo7mVib";

        private const string PROTOTYPE_URL = @"https://wmelongames.com/prototype/guide.php";
        private const string SITE_URL = @"https://wmelongames.com";

        private const string COMPANY_EMAIL = "wmelongames@gmail.com";
        private const string GAME_NAME = "Color Hole 3D";

        private const string PREFS_NAME = "ShowSetupGuideOnStartup";

        private const string PROJECT_DESCRIPTION = @"Thank you for purchasing " + GAME_NAME + ".\nBefore start working with project, please, read documentation. \nIf project has any bugs, let us know. \nPlease, write review and rate the project.";

        private static readonly Vector2 WINDOW_SIZE = new Vector2(650, 450);
        private static readonly string WINDOW_TITLE = GAME_NAME + " | Setup Guide";

        private static readonly ProjectSettings[] projectSettings = new ProjectSettings[]
        {
            new ProjectSettings("Game Settings", typeof(GameSettings)),
            new ProjectSettings("Ads Settings", typeof(AdsSettings)),
            new ProjectSettings("Levels Database", typeof(LevelsDatabase)),
        };

        private static readonly ProjectFolders[] projectFolders = new ProjectFolders[]
        {
            new ProjectFolders("Scenes Folder", "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Game/Scenes"),
            new ProjectFolders("Scripts Folder", "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Game/Scripts")
        };

        private static readonly ProjectFolders[] projectFiles = new ProjectFolders[]
        {
            new ProjectFolders("Icon", "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Resources/Icons/icon512x512.png"),
        };

        private static readonly ProjectWindow[] projectWindows = new ProjectWindow[]
        {
            new ProjectWindow("Level Editor", "Tools/Project/Level Editor"),
            new ProjectWindow("Define Manager", "Tools/Editor/Define Manager"),
        };

        private static SetupGuideWindow window;

        public Texture2D logoBlack;
        public Texture2D logoWhite;

        private int currentTab = 0;
        private bool requireTabInit = true;
        
        private GUIStyle backgroundStyle;
        private GUIStyle boldTextTitleStyle;
        private GUIStyle gameButtonStyle;

        private GUIContent helpBoxWithIconContent;
        private GUIContent logoContent;

        private bool stylesInited;

        private Vector2 scrollView;
        
        private FinishedProject[] finishedProjects;

        private TabContainer[] tabContainers;
        private string[] tabs;

        [MenuItem("Tools/Project Setup Guide")]
        [MenuItem("Window/Project Setup Guide")]
        static void ShowWindow()
        {
            SetupGuideWindow tempWindow = (SetupGuideWindow)GetWindow(typeof(SetupGuideWindow), true, WINDOW_TITLE);
            tempWindow.minSize = WINDOW_SIZE;
            tempWindow.maxSize = WINDOW_SIZE;

            window = tempWindow;

            EditorStylesExtended.InitializeStyles();
        }

        private void OnEnable()
        {
            window = this;

            // Tabs
            List<TabContainer> tabsList = new List<TabContainer>();
            tabsList.Add(new TabContainer("Info", window.TabInfo, -1));

            Assembly assembly = Assembly.GetAssembly(typeof(SetupTabAttribute));
            Type[] gameTypes = assembly.GetTypes().Where(m => m.IsDefined(typeof(SetupTabAttribute), true)).ToArray();

            foreach (Type type in gameTypes)
            {
                //Get attribute
                SetupTabAttribute[] tabAttributes = (SetupTabAttribute[])Attribute.GetCustomAttributes(type, typeof(SetupTabAttribute));

                for (int i = 0; i < tabAttributes.Length; i++)
                {
                    UnityEngine.Object tabObject = EditorUtils.GetAsset(type);
                    if (tabObject != null)
                    {
                        tabsList.Add(new TabContainer(tabAttributes[i].tabName, tabObject, tabAttributes[i].priority));
                    }
                }
            }
            
            tabContainers = tabsList.OrderBy(x => x.priority).ToArray();
            tabs = new string[tabContainers.Length];
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i] = tabContainers[i].name;
            }

            //Buttons
            //Project settings
            for(int i = 0; i < projectSettings.Length; i++)
            {
                projectSettings[i].Init();
            }

            //Project folders
            for (int i = 0; i < projectFolders.Length; i++)
            {
                projectFolders[i].Init();
            }

            //Project files
            for (int i = 0; i < projectFiles.Length; i++)
            {
                projectFiles[i].Init();
            }

            EditorCoroutines.Execute(GetRequest(PROTOTYPE_URL));
        }

        private void InitStyles()
        {
            if (stylesInited)
                return;
            
            backgroundStyle = EditorStylesExtended.GetBoxWithColor(EditorGUIUtility.isProSkin ? new Color(0.11f, 0.11f, 0.11f, 1.0f) : new Color(0.6f, 0.6f, 0.6f, 1.0f));
            logoContent = new GUIContent(EditorGUIUtility.isProSkin ? logoWhite : logoBlack, SITE_URL);

            boldTextTitleStyle = EditorStylesExtended.GetAligmentStyle(EditorStylesExtended.label_medium_bold, TextAnchor.MiddleCenter);
            boldTextTitleStyle.alignment = TextAnchor.MiddleCenter;
            boldTextTitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            gameButtonStyle = EditorStylesExtended.GetPaddingStyle(EditorStylesExtended.button_05, new RectOffset(2, 2, 2, 2));

            helpBoxWithIconContent = new GUIContent(PROJECT_DESCRIPTION, EditorStylesExtended.GetTexture("icon_info"));

            stylesInited = true;
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(backgroundStyle, GUILayout.Width(250), GUILayout.MaxWidth(250));

            GUILayout.Space(40);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(logoContent, GUIStyle.none, GUILayout.Width(200)))
            {
                Application.OpenURL(SITE_URL);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            EditorGUILayout.LabelField(GAME_NAME, boldTextTitleStyle, GUILayout.Height(40));
            GUILayout.Space(10);

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (finishedProjects != null)
            {
                for (int i = 0; i < finishedProjects.Length; i++)
                {
                    if (GUILayout.Button(new GUIContent(finishedProjects[i].gameTexture, finishedProjects[i].name), gameButtonStyle, GUILayout.Height(80), GUILayout.Width(80)))
                    {
                        Application.OpenURL(finishedProjects[i].url);
                    }

                    if ((i + 1) % 2 == 0)
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(8);

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.Space(8);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Documentation", EditorStylesExtended.button_05_large))
            {
                Application.OpenURL(DOCUMENTATION_URL);
            }

            GUILayout.Space(10);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            GUILayout.Space(5);

            int tempTab = GUILayout.Toolbar(currentTab, tabs, EditorStylesExtended.tab);
            if (tempTab != currentTab)
            {
                currentTab = tempTab;

                requireTabInit = true;

                scrollView = Vector2.zero;

                GUI.FocusControl(null);
            }

            scrollView = EditorGUILayout.BeginScrollView(scrollView);
            tabContainers[currentTab].DrawTab();
            GUILayout.Space(5);
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void TabInfo()
        {
            if (requireTabInit)
            {
                requireTabInit = false;
            }

            GUILayout.Space(5);

            EditorGUILayout.LabelField(helpBoxWithIconContent, EditorStylesExtended.helpbox);
            
            GUILayout.Space(5);

            EditorGUILayout.SelectableLabel(COMPANY_EMAIL, boldTextTitleStyle, GUILayout.Height(24));

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            
            GUILayout.Space(5);

            for (int i = 0; i < projectSettings.Length; i++)
            {
                projectSettings[i].Draw(EditorStylesExtended.button_02);
            }

            GUILayout.Space(5);

            for (int i = 0; i < projectFolders.Length; i++)
            {
                projectFolders[i].Draw(EditorStylesExtended.button_03);
            }
            
            GUILayout.Space(5);

            for (int i = 0; i < projectFiles.Length; i++)
            {
                projectFiles[i].Draw(EditorStylesExtended.button_04);
            }

            GUILayout.Space(5);

            for (int i = 0; i < projectWindows.Length; i++)
            {
                projectWindows[i].Draw(EditorStylesExtended.button_01);
            }

            GUILayout.FlexibleSpace();
        }

        #region Web
        private IEnumerator GetRequest(string uri)
        {
            UnityWebRequest www = UnityWebRequest.Get(uri);
            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if(www.downloadHandler.data != null)
                {
                    // Or retrieve results as binary data
                    byte[] results = www.downloadHandler.data;

                    // For that you will need to add reference to System.Runtime.Serialization
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(results, new System.Xml.XmlDictionaryReaderQuotas());

                    // For that you will need to add reference to System.Xml and System.Xml.Linq
                    var root = XElement.Load(jsonReader);

                    List<FinishedProject> finishedProjectsTemp = new List<FinishedProject>();
                    foreach (var element in root.Elements())
                    {
                        FinishedProject projectTemp = new FinishedProject(element.XPathSelectElement("name").Value, element.XPathSelectElement("url").Value, null);

                        EditorCoroutines.Execute(GetTexture(element.XPathSelectElement("image").Value, (texture) =>
                        {
                            projectTemp.gameTexture = texture;
                        }));

                        finishedProjectsTemp.Add(projectTemp);
                    }

                    finishedProjects = finishedProjectsTemp.ToArray();
                }
            }
        }

        private IEnumerator GetTexture(string uri, System.Action<Texture2D> onLoad)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
            www.SendWebRequest();

            while (!www.isDone)
            {
                yield return null;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if (myTexture != null)
                {
                    onLoad.Invoke(myTexture);

                    window.Repaint();
                }
            }
        }
        #endregion
        
        [InitializeOnLoadMethod]
        public static void SetupGuideStartup()
        {
            if (EditorPrefs.HasKey("ShowStartupGuide"))
                return;

            EditorApplication.delayCall += delegate
            {
                SetupGuideWindow.ShowWindow();
            };

            EditorPrefs.SetBool("ShowStartupGuide", true);
        }

        private class FinishedProject
        {
            public string name = "";
            public string url = "";

            public Texture2D gameTexture;

            public FinishedProject(string name, string url, Texture2D gameTexture)
            {
                this.name = name;
                this.url = url;
                this.gameTexture = gameTexture;
            }
        }

        private class TabContainer
        {
            public string name;
            public int priority;

            private Object tabObject;
            private Editor tabEditor;
            private DrawTabDelegate drawTabFunction;

            public TabContainer(string name, DrawTabDelegate drawTabFunction, int priority = int.MaxValue)
            {
                this.name = name;
                this.drawTabFunction = drawTabFunction;
                this.priority = priority;
            }

            public TabContainer(string name, Object tabObject, int priority = int.MaxValue)
            {
                this.name = name;
                this.tabObject = tabObject;
                this.priority = priority;

                Editor.CreateCachedEditor(tabObject, null, ref tabEditor);
            }

            public void DrawTab()
            {
                if(tabEditor)
                {
                    tabEditor.serializedObject.Update();
                    tabEditor.OnInspectorGUI();
                    tabEditor.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if(drawTabFunction != null)
                    {
                        drawTabFunction.Invoke();
                    }
                }
            }

            public delegate void DrawTabDelegate();
        }

        private class ProjectSettings
        {
            public string name;
            public Type type;

            private Object settingObject;

            public ProjectSettings(string name, Type type)
            {
                this.name = name;
                this.type = type;
            }

            public void Init()
            {
                settingObject = EditorUtils.GetAsset(type);
            }

            public void Draw(GUIStyle customStyle = null)
            {
                if(settingObject != null && GUILayout.Button(name, customStyle != null ? customStyle : EditorStylesExtended.button_01))
                {
                    Selection.activeObject = settingObject;
                    EditorGUIUtility.PingObject(settingObject);
                }
            }
        }

        private class ProjectFolders
        {
            public string name;
            public string path;

            private Object settingObject;

            public ProjectFolders(string name, string path)
            {
                this.name = name;
                this.path = path;
            }

            public void Init()
            {
                settingObject = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            }

            public void Draw(GUIStyle customStyle = null)
            {
                if (settingObject != null && GUILayout.Button(name, customStyle != null ? customStyle : EditorStylesExtended.button_01))
                {
                    Selection.activeObject = settingObject;
                    EditorGUIUtility.PingObject(settingObject);
                }
            }
        }

        private class ProjectWindow
        {
            public string name;
            public string path;
            
            public ProjectWindow(string name, string path)
            {
                this.name = name;
                this.path = path;
            }

            public void Draw(GUIStyle customStyle = null)
            {
                if (GUILayout.Button(name, customStyle != null ? customStyle : EditorStylesExtended.button_01))
                {
                    EditorApplication.ExecuteMenuItem(path);
                }
            }
        }
    }
}