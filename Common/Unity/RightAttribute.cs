using System;

namespace Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RightAttribute : Attribute


    {
        public RightAttribute()

        {

        }


        public string ResourceName { get; set; }

        public string OperationCode { get; set; }


    }
}