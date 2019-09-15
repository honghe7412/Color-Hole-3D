using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(LineSpacerAttribute))]
    public class LineSpacerDrawer : DecoratorDrawer
    {

        private LineSpacerAttribute lineSpacer
        {
            get { return (LineSpacerAttribute)attribute; }
        }

        public override void OnGUI(Rect position)
        {
            LineSpacerAttribute lineSpacer = this.lineSpacer;
            float lineHeight = lineSpacer.lineHeight;

            Color oldGuiColor = GUI.color;
            if (!string.IsNullOrEmpty(lineSpacer.title))
                EditorGUI.LabelField(new Rect(position.x, position.y + lineHeight - 8, position.width, lineHeight), lineSpacer.title, EditorStyles.boldLabel);
            EditorGUI.LabelField(new Rect(position.x, position.y + lineHeight, position.width, lineHeight), "", GUI.skin.horizontalSlider);
            GUI.color = oldGuiColor;
        }

        public override float GetHeight()
        {
            return base.GetHeight() + lineSpacer.lineHeight;
        }
    }
}