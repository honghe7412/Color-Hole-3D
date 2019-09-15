using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Watermelon
{
    [InitializeOnLoad]
    public static class EditorStylesExtended
    {
        private const string ICONS_FOLDER_PATH = "/Sprites/Icons";
        private const string GUISKIN_NAME = "EditorStylesExtendedSkin";
        private const string GUISKIN_PRO_NAME = "EditorStylesExtendedProSkin";

        public static GUISkin editorSkin;

        public static GUIStyle tab;

        public static GUIStyle box_01;
        public static GUIStyle box_02;
        public static GUIStyle box_03;

        public static GUIStyle label_small;
        public static GUIStyle label_small_bold;

        public static GUIStyle label_medium;
        public static GUIStyle label_medium_bold;

        public static GUIStyle label_large;
        public static GUIStyle label_large_bold;

        public static GUIStyle button_01;
        public static GUIStyle button_01_large;

        public static GUIStyle button_02;
        public static GUIStyle button_02_large;

        public static GUIStyle button_03;
        public static GUIStyle button_03_large;

        public static GUIStyle button_04;
        public static GUIStyle button_04_large;

        public static GUIStyle button_05;
        public static GUIStyle button_05_large;

        public static GUIStyle helpbox;

        private static Dictionary<string, Texture2D> projectIcons = new Dictionary<string, Texture2D>();

        private static bool isInited = false;
        private static bool isLoading = false;

        static EditorStylesExtended()
        {
            InitializeStyles();
        }

        public static void InitializeStyles()
        {
            if (isInited)
                return;

            if(!editorSkin)
            {
                if (EditorGUIUtility.isProSkin)
                    editorSkin = EditorUtils.GetAsset<GUISkin>(GUISKIN_PRO_NAME);

                if (!editorSkin)
                    editorSkin = EditorUtils.GetAsset<GUISkin>(GUISKIN_NAME);
            }
            
            if(editorSkin)
            {
                LoadIcons();

                tab = editorSkin.GetStyle("Tab");

                box_01 = editorSkin.GetStyle("box_01");
                box_02 = editorSkin.GetStyle("box_02");
                box_03 = editorSkin.GetStyle("box_03");

                label_small = editorSkin.GetStyle("label_small");
                label_small_bold = editorSkin.GetStyle("label_small_bold");

                label_medium = editorSkin.GetStyle("label_medium");
                label_medium_bold = editorSkin.GetStyle("label_medium_bold");

                label_large = editorSkin.GetStyle("label_large");
                label_large_bold = editorSkin.GetStyle("label_large_bold");

                button_01 = editorSkin.GetStyle("button_01");
                button_01_large = editorSkin.GetStyle("button_01_large");

                button_02 = editorSkin.GetStyle("button_02");
                button_02_large = editorSkin.GetStyle("button_02_large");

                button_03 = editorSkin.GetStyle("button_03");
                button_03_large = editorSkin.GetStyle("button_03_large");

                button_04 = editorSkin.GetStyle("button_04");
                button_04_large = editorSkin.GetStyle("button_04_large");

                button_05 = editorSkin.GetStyle("button_05");
                button_05_large = editorSkin.GetStyle("button_05_large");

                helpbox = editorSkin.GetStyle("helpbox");

                isInited = true;
            }
            else
            {
                if(!isLoading)
                    EditorCoroutines.Execute(TryToInitStyles());

                isLoading = true;
            }
        }

        #region Styles
        public static GUIStyle GetAligmentStyle(GUIStyle style, TextAnchor textAnchor)
        {
            GUIStyle tempStyle = new GUIStyle(style);
            tempStyle.alignment = textAnchor;

            return tempStyle;
        }

        public static GUIStyle GetTextColorStyle(GUIStyle style, Color color)
        {
            GUIStyle tempStyle = new GUIStyle(style);
            tempStyle.normal.textColor = color;

            return tempStyle;
        }

        public static GUIStyle GetTextFontSizeStyle(GUIStyle style, int fontSize)
        {
            GUIStyle tempStyle = new GUIStyle(style);
            tempStyle.fontSize = fontSize;

            return tempStyle;
        }

        public static GUIStyle GetPaddingStyle(GUIStyle style, RectOffset padding)
        {
            GUIStyle tempStyle = new GUIStyle(style);
            tempStyle.padding = padding;

            return tempStyle;
        }

        public static GUIStyle GetBoxWithColor(Color color)
        {
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();

            GUIStyle backgroundStyle = new GUIStyle();
            backgroundStyle.normal.background = backgroundTexture;

            return backgroundStyle;
        }
        #endregion

        #region Icons
        private static void LoadIcons()
        {
            projectIcons = new Dictionary<string, Texture2D>();
            
            string folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(editorSkin)).Replace(@"\", "/") + ICONS_FOLDER_PATH;

            string[] guids = AssetDatabase.FindAssets("t:texture2D", new[] { folderPath });

            foreach (string guid in guids)
            {
                Texture2D tempTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));

                projectIcons.Add(tempTexture.name, tempTexture);
            }
        }

        public static Texture2D GetTexture(string name)
        {
            if (projectIcons.ContainsKey(name))
            {
                return projectIcons[name];
            }

            Debug.LogWarning("Texture " + name + " can't be found!");

            return null;
        }
        #endregion

        private static IEnumerator TryToInitStyles()
        {
            for (int i = 0; i < 5; i++)
            {
                if (isInited)
                    yield break;

                InitializeStyles();

                yield return null;
            }
        }
    }
}
