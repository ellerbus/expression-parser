using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSniper.Library.ExpressionEngine
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ExpressionMethodAttribute : Attribute
    {
        // This is a positional argument
        public ExpressionMethodAttribute(string name)
        {
            Name = name.ToUpper();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}
