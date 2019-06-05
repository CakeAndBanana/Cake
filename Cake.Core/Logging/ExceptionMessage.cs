using System;

namespace Cake.Core.Logging
{
    public class ExceptionMessage : Message
    {
        public ExceptionMessage(Exception exception) : base(exception.Message, Type.Error)
        {
            
        }
    }
}
