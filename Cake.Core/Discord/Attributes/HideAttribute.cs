using System;

namespace Cake.Core.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute
    {
        public bool Hide;

        public HideAttribute(bool status = true)
        {
            Hide = status;
        }
    }
}
