using System;
using UnityEngine;

namespace Watermelon
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LineSpacerAttribute : PropertyAttribute
    {
        public float lineHeight;
        public string title;

        public LineSpacerAttribute(string title = "", float lineHeight = 18)
        {
            this.title = title;

            if (string.IsNullOrEmpty(title))
                this.lineHeight = 0;
            else
                this.lineHeight = lineHeight;
        }
    }
}
