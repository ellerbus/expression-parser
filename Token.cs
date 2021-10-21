using System;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class Token
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Token(string text, Scanner sc, TokenTypes type)
            : this(text, sc.Line, sc.Column, sc.Position, type)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Token(string text, int line, int column, int position, TokenTypes type)
        {
            Text = text;

            Line = line;
            Column = column;
            Position = position;

            Type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(
                Text,
                " ",
                "[", Type.ToString(), ", ",
                Line.ToString(), ":", Column.ToString(), "]"
                );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets / Sets the text of this token
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Line
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Column
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Position
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public TokenTypes Type
        {
            get;
            set;
        }

        #endregion
    }
}
