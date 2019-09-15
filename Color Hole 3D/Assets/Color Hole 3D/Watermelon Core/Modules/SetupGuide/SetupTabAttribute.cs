using System;

namespace Watermelon
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SetupTabAttribute : Attribute
    {
        public string tabName;
        public int priority = int.MaxValue;

        public SetupTabAttribute(string tabName)
        {
            this.tabName = tabName;
        }
    }
}