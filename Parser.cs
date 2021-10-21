using System;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class Parser
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="lookAheadDepth"></param>
        public Parser(Lexer lexer, int lookAheadDepth)
        {
            LookAheadDepth = lookAheadDepth;

            Tokens = new Token[LookAheadDepth];

            Lexer = lexer;

            for (int i = 0; i < LookAheadDepth; i++)
            {
                Tokens[i] = Lexer.GetToken();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the next token
        /// </summary>
        /// <returns></returns>
        public Token LA()
        {
            return LA(1);
        }

        /// <summary>
        /// Get the next token
        /// </summary>
        /// <param name="lookAhead"></param>
        /// <returns></returns>
        public Token LA(int lookAhead)
        {
            lookAhead -= 1;

            if (lookAhead >= 0 && lookAhead < Tokens.Length)
            {
                return Tokens[lookAhead];
            }

            return null;
        }

        /// <summary>
        /// Consume the first token, and get another from the lexer stack
        /// </summary>
        public void Consume()
        {
            //  shift the tokens over
            for (int i = 0; i < LookAheadDepth - 1; i++)
            {
                Tokens[i] = Tokens[i + 1];
            }

            Tokens[LookAheadDepth - 1] = Lexer.GetToken();

            //Debug.WriteLine(string.Format("Token: {0}", Tokens[LookAheadDepth - 1].Text));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int LookAheadDepth
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Token[] Tokens
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Lexer Lexer
        {
            get;
            private set;
        }

        #endregion
    }
}
