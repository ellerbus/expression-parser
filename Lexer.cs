using System;
using System.Text;

namespace StockSniper.Library.ExpressionEngine
{
    /// <summary>
    /// Rules for which token a list of characters make
    /// </summary>
    sealed class Lexer
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public Lexer(string text)
        {
            Scanner = new Scanner(text);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Token GetToken()
        {
            if (!Scanner.EndOfText)
            {
                if (IsWhiteSpace(Scanner.LA()))
                {
                    ConsumeWhiteSpace();
                }

                char la = Scanner.LA();

                switch (la)
                {
                    case '=':
                        Match('=');
                        if (Scanner.LA() == '=')
                        {
                            Match('=');
                        }
                        return new Token("=", Scanner, TokenTypes.Eq);

                    case '!':
                        Match('!');
                        if (Scanner.LA() == '=')
                        {
                            Match('=');
                            return new Token("!=", Scanner, TokenTypes.Ne);
                        }
                        return new Token("!", Scanner, TokenTypes.Not);

                    case '<':
                        Match('<');
                        if (Scanner.LA() == '=')
                        {
                            Match('=');
                            return new Token("<=", Scanner, TokenTypes.Le);
                        }
                        if (Scanner.LA() == '>')
                        {
                            Match('>');
                            return new Token("<>", Scanner, TokenTypes.Ne);
                        }
                        return new Token("<", Scanner, TokenTypes.Lt);

                    case '>':
                        Match('>');
                        if (Scanner.LA() == '=')
                        {
                            Match('=');
                            return new Token(">=", Scanner, TokenTypes.Ge);
                        }
                        return new Token(">", Scanner, TokenTypes.Gt);

                    case '+':
                        Match('+');
                        return new Token("+", Scanner, TokenTypes.Add);

                    case '-':
                        Match('-');
                        return new Token("-", Scanner, TokenTypes.Sub);

                    case '*':
                        Match('*');
                        return new Token("*", Scanner, TokenTypes.Mul);

                    case '/':
                        Match('/');
                        return new Token("/", Scanner, TokenTypes.Div);

                    case '^':
                        Match('^');
                        return new Token("^", Scanner, TokenTypes.Pow);

                    case ')':
                        Match(')');
                        return new Token(")", Scanner, TokenTypes.RParen);

                    case '(':
                        Match('(');
                        return new Token("(", Scanner, TokenTypes.LParen);

                    case ',':
                        Match(',');
                        return new Token(",", Scanner, TokenTypes.Comma);

                    case '"':
                    case '\'':
                        return GetString(la);
                }

                if (IsKeywordMatch("true"))
                {
                    return new Token("1", Scanner, TokenTypes.Num);
                }

                if (IsKeywordMatch("false"))
                {
                    return new Token("0", Scanner, TokenTypes.Num);
                }

                if (IsKeywordMatch("and"))
                {
                    return new Token("and", Scanner, TokenTypes.And);
                }

                if (IsKeywordMatch("&&"))
                {
                    return new Token("and", Scanner, TokenTypes.And);
                }

                if (IsKeywordMatch("or"))
                {
                    return new Token("or", Scanner, TokenTypes.Or);
                }

                if (IsKeywordMatch("||"))
                {
                    return new Token("or", Scanner, TokenTypes.Or);
                }

                if (IsKeywordMatch("not"))
                {
                    return new Token("not", Scanner, TokenTypes.Not);
                }

                if (IsInRange(la, '0', '9'))
                {
                    return GetNumber();
                }

                if (IsInRange(la, 'A', 'Z') ||
                    IsInRange(la, 'a', 'z') ||
                    la == '_')
                {
                    return GetIdentifier();
                }
            }

            return new Token("<EOF>", Scanner, TokenTypes.EOF);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConsumeWhiteSpace()
        {
            while (IsWhiteSpace(Scanner.LA()))
            {
                Scanner.Consume();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsWhiteSpace(char c)
        {
            if (char.IsWhiteSpace(c) || c == '$')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool IsInRange(char la, char start, char end)
        {
            if (start <= la && la <= end)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Consume if character matches, otherwise throw a format exception
        /// </summary>
        private bool IsKeywordMatch(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char la = char.ToUpper(Scanner.LA(i + 1));
                char ch = char.ToUpper(str[i]);

                if (la != ch)
                {
                    return false;
                }
            }

            for (int i = 0; i < str.Length; i++)
            {
                Match(Scanner.LA());
            }

            return true;
        }

        /// <summary>
        /// Consume if character matches, otherwise throw a format exception
        /// </summary>
        private void Match(char c)
        {
            char la = Scanner.LA();

            if (la != c)
            {
                throw new FormatException();
            }

            Scanner.Consume();
        }

        /// <summary>
        /// Consume if character matches within a range, otherwise throw a format exception
        /// </summary>
        private void MatchRange(char start, char end)
        {
            char la = Scanner.LA();

            if (la < start || la > end)
            {
                throw new FormatException();
            }

            Scanner.Consume();
        }

        /// <summary>
        /// Consume if character matches, otherwise throw a format exception
        /// </summary>
        private void Match(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                Match(str[i]);
            }
        }

        #endregion

        #region Token Methods

        /// <summary>
        /// [0-9][0-9]*(.[0-9]+)?(%)?
        /// </summary>
        /// <returns></returns>
        private Token GetNumber()
        {
            int start = Scanner.Position;

            MatchRange('0', '9');

            ConsumeDigits();

            if (Scanner.LA() == '.')
            {
                Match('.');

                MatchRange('0', '9');

                ConsumeDigits();
            }

            //  optionally followed by (T|B|M|K) E#, e#, %
            char la = Scanner.LA();

            switch (la)
            {
                case 'E':
                case 'e':
                    Match(la);
                    MatchRange('0', '9');
                    ConsumeDigits();
                    break;

                case '%':
                case 'T':
                case 't':
                case 'B':
                case 'b':
                case 'M':
                case 'm':
                case 'K':
                case 'k':
                    Match(la);
                    break;
            }

            string text = Scanner.Text.Substring(start, Scanner.Position - start);

            return new Token(text, Scanner, TokenTypes.Num);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConsumeDigits()
        {
            while (true)    // (...)*
            {
                char la = Scanner.LA();

                if (IsInRange(la, '0', '9'))
                {
                    MatchRange('0', '9');
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// [a-z,A-Z,_][a-z,A-Z,0-9,_]*
        /// </summary>
        /// <returns></returns>
        private Token GetIdentifier()
        {
            int start = Scanner.Position;

            char la = Scanner.LA();

            if (IsInRange(la, 'a', 'z'))
            {
                MatchRange('a', 'z');
            }
            else if (IsInRange(la, 'A', 'Z'))
            {
                MatchRange('A', 'Z');
            }
            else if (la == '_')
            {
                Match('_');
            }

            while (true)    // (...)*
            {
                la = Scanner.LA();

                if (IsInRange(la, 'A', 'Z'))
                {
                    MatchRange('A', 'Z');
                }
                else if (IsInRange(la, 'a', 'z'))
                {
                    MatchRange('a', 'z');
                }
                else if (IsInRange(la, '0', '9'))
                {
                    MatchRange('0', '9');
                }
                else if (la == '_')
                {
                    Match('_');
                }
                else
                {
                    break;
                }
            }

            string text = Scanner.Text.Substring(start, Scanner.Position - start);

            return new Token(text.ToUpper(), Scanner, TokenTypes.Id);
        }

        /// <summary>
        /// "([^"]*)"
        /// </summary>
        /// <returns></returns>
        private Token GetString(char quote)
        {
            StringBuilder sb = new StringBuilder(32);

            Match(quote);

            while (true)
            {
                //  check for escape
                char la = Scanner.LA();

                if (la == quote && Scanner.LA(2) == quote)
                {
                    //  escape
                    //  "this is ""my"" sample"
                    Match(quote);
                    Match(quote);

                    sb.Append(quote);
                }
                else if (la == quote)
                {
                    Match(quote);

                    break;
                }
                else
                {
                    sb.Append(la);

                    Match(la);
                }
            }

            string text = sb.ToString();

            return new Token(text, Scanner, TokenTypes.Str);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Scanner Scanner
        {
            get;
            private set;
        }

        #endregion
    }
}
