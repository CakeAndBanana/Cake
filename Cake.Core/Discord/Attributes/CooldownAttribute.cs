using System;

namespace Cake.Core.Discord.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CooldownAttribute : Attribute
    {
        public CooldownAttribute()
        {
            throw new NotImplementedException("Issue #33 github :)");
        }
    }
}