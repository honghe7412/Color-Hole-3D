using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Watermelon;

public static class ScenesMenu
{
    private static void OpenScene(string path)
    {
        int option = EditorUtility.DisplayDialogComplex("Select Scene Loading Mode", "Select Single mode if you want to close all previous scenes and Additive if you want to add selected scene to current opened scene.", "Single", "Additive", "Cancel");
        switch(option)
        {
             case 0:
                 Scene[] scenes = new Scene[SceneManager.sceneCount];
                 for (int i = 0; i < scenes.Length; i++)
                 {
                     scenes[i] = SceneManager.GetSceneAt(i);
                 }
                 EditorSceneManager.SaveModifiedScenesIfUserWantsTo(scenes);
                 EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                 
                 break;
             case 1:
                 EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                 break;
        }
    }

    [MenuItem("Scenes/Init")]
    public static void Scene0()
    {
        if(Application.isPlaying)
        {
             string sceneEnumName = "Init";
             if (System.Array.FindIndex(System.Enum.GetNames(typeof(Scenes)), x => x == sceneEnumName) != -1)
             {
                 Scenes enumScene = (Scenes)System.Enum.Parse(typeof(Scenes), sceneEnumName);
                 SceneLoader.LoadScene(enumScene);
             }
        }
        else
        {
            OpenScene("Assets/Color Hole 3D/Game/Scenes/Init.unity");
        }
    }

    [MenuItem("Scenes/Game")]
    public static void Scene1()
    {
        if(Application.isPlaying)
        {
             string sceneEnumName = "Game";
             if (System.Array.FindIndex(System.Enum.GetNames(typeof(Scenes)), x => x == sceneEnumName) != -1)
             {
                 Scenes enumScene = (Scenes)System.Enum.Parse(typeof(Scenes), sceneEnumName);
                 SceneLoader.LoadScene(enumScene);
             }
        }
        else
        {
            OpenScene("Assets/Color Hole 3D/Game/Scenes/Game.unity");
        }
    }

}
