using System;

namespace Cake.Logger
{
    public class ExceptionMessage : Message
    {
        public ExceptionMessage(Exception exception) : base(exception.Message, Type.Error)
        {
            
        }
    }
}
