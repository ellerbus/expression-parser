using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class Scanner
    {
        #region Constructor

        /// <summary>
        /// Instantiate the character buffer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="depth">depth of this buffer</param>
        public Scanner(string text)
        {
            Reset(text);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Reset(string text)
        {
            Position = 0;
            Text = text;
            Length = text.Length;

            Line = 1;
            Column = 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookAhead"></param>
        /// <returns></returns>
        public char LA()
        {
            return LA(1);
        }

        /// <summary>
        /// Get the next character by the lookahead
        /// </summary>
        /// <param name="lookAhead"></param>
        /// <returns></returns>
        public char LA(int lookAhead)
        {
            int index = Position + lookAhead - 1;

            if (index >= 0 && index < Text.Length)
            {
                return Text[index];
            }

            return '\0';
        }

        /// <summary>
        /// Remove the first character, and pull the next one in.
        /// </summary>
        public void Consume()
        {
            if (Position < Length)
            {
                if (Text[Position] == '\n')
                {
                    Column = 0;
                    Line += 1;
                }

                Column += 1;
                Position += 1;
            }
        }

        #endregion

        #region Properties

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
        public bool EndOfText
        {
            get { return Position >= Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get;
            private set;
        }

        #endregion
    }
}
