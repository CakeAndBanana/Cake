using System;

namespace Cake.Core.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CooldownAttribute : Attribute
    {
        private readonly bool _hide;

        public CooldownAttribute(bool status = true)
        {
            throw new NotImplementedException("Issue #33 github :)");
        }
    }
}