using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomEditor(typeof(Tween))]
    public class TweenEditor : Editor
    {
        private Tween refTarget;

        private void OnEnable()
        {
            refTarget = (Tween)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < refTarget.Tweens.Length; i++)
            {
                if (refTarget.Tweens[i] != null)
                {
                    if (GUILayout.Button(refTarget.Tweens[i].activeId + ": " + refTarget.Tweens[i].GetType()))
                    {
                        refTarget.Tweens[i].Kill();

                        break;
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
