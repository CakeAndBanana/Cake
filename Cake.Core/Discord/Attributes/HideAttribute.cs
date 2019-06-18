using System;

namespace Cake.Core.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute
    {
        private readonly bool _hide;

        public HideAttribute(bool status = true)
        {
            _hide = status;
        }
    }
}
