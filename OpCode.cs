using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class OpCode
    {
        #region Member Variables

        #endregion

        #region Constructors

        public OpCode(Token token)
        {
            Token = token;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Token == null ? "n/a" : Token.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the Value associated with this operational code
        /// (multiple objects: MethodInfo, Constant Value, etc.)
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the token associated with this operational code
        /// </summary>
        public Token Token
        {
            get;
            private set;
        }

        #endregion
    }
}
