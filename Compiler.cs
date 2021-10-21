using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class Compiler
    {
        #region Member Variables

        private Parser _parser;

        private List<OpCode> _ops;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Compiler()
        {
            _ops = new List<OpCode>(32);
        }

        #endregion

        #region Compile Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OpCode> Compile(string expression)
        {
            if (expression.StartsWith("="))
            {
                expression = expression.Substring(1);
            }

            CheckForBalancedParens(expression);

            _parser = new Parser(new Lexer(expression), 3);

            CompileExpression();

            return _ops;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckForBalancedParens(string expression)
        {
            int lp = 0;
            int rp = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                {
                    lp += 1;
                }
                if (expression[i] == ')')
                {
                    rp += 1;
                }
            }

            if (lp != rp)
            {
                throw new InvalidOperationException("Unbalanced Paranthesis");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileExpression()
        {
            CompileLogicalExpression();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileLogicalExpression()
        {
            CompileRelationalExpression();

            //  and,or

            Token token = _parser.LA();

            do
            {
                if (token.Type == TokenTypes.And ||
                    token.Type == TokenTypes.Or)
                {
                    _parser.Consume();

                    CompileRelationalExpression();

                    AddOpCode(token);
                }
                else
                {
                    break;
                }

                token = _parser.LA();

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileRelationalExpression()
        {
            CompileAdditiveExpression();

            //  >,>=,<,<=,=,<>

            Token token = _parser.LA();

            do
            {
                if (token.Type == TokenTypes.Eq ||
                    token.Type == TokenTypes.Ne ||
                    token.Type == TokenTypes.Gt ||
                    token.Type == TokenTypes.Ge ||
                    token.Type == TokenTypes.Lt ||
                    token.Type == TokenTypes.Le)
                {
                    _parser.Consume();

                    CompileAdditiveExpression();

                    AddOpCode(token);
                }
                else
                {
                    break;
                }

                token = _parser.LA();

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileAdditiveExpression()
        {
            CompileMultiplicativeExpression();

            //  +,-

            Token token = _parser.LA();

            do
            {
                if (token.Type == TokenTypes.Add ||
                    token.Type == TokenTypes.Sub)
                {
                    _parser.Consume();

                    CompileMultiplicativeExpression();

                    AddOpCode(token);
                }
                else
                {
                    break;
                }

                token = _parser.LA();

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileMultiplicativeExpression()
        {
            CompilePowerExpression();

            //  *,/

            Token token = _parser.LA();

            do
            {
                if (token.Type == TokenTypes.Mul ||
                    token.Type == TokenTypes.Div)
                {
                    _parser.Consume();

                    CompilePowerExpression();

                    AddOpCode(token);
                }
                else
                {
                    break;
                }

                token = _parser.LA();

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompilePowerExpression()
        {
            CompileUnaryExpression();

            //  *,/

            Token token = _parser.LA();

            do
            {
                if (token.Type == TokenTypes.Pow)
                {
                    _parser.Consume();

                    CompileUnaryExpression();

                    AddOpCode(token);
                }
                else
                {
                    break;
                }

                token = _parser.LA();

            } while (true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileUnaryExpression()
        {
            //  -,!,not

            Token token = _parser.LA();

            switch (token.Type)
            {
                case TokenTypes.Sub:
                case TokenTypes.Not:
                    _parser.Consume();
                    break;
            }

            CompileFactorExpression();

            switch (token.Type)
            {
                case TokenTypes.Sub:
                    token.Type = TokenTypes.Neg;
                    AddOpCode(token);
                    break;

                case TokenTypes.Not:
                    AddOpCode(token);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileFactorExpression()
        {
            Token token = _parser.LA();

            if (token.Type == TokenTypes.LParen)
            {
                _parser.Consume();	//LPAREN

                CompileExpression();

                _parser.Consume();	//RPAREN
            }
            else
            {
                CompileAtom();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileAtom()
        {
            Token token = _parser.LA();

            switch (token.Type)
            {
                case TokenTypes.Num:
                case TokenTypes.Str:
                    AddOpCode(token);
                    _parser.Consume();
                    break;

                case TokenTypes.Id:
                    _parser.Consume();

                    CompileArguments();

                    AddOpCode(token);
                    break;

                case TokenTypes.EOF:
                    break;

                default:
                    ThrowUnexpectedToken(
                        token,
                        TokenTypes.Num,
                        TokenTypes.Str,
                        TokenTypes.Id,
                        TokenTypes.LParen
                        );
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parser"></param>
        /// <returns></returns>
        private void CompileArguments()
        {
            Token token = _parser.LA();

            if (token.Type == TokenTypes.LParen)
            {
                _parser.Consume();	//LPAREN

                token = _parser.LA();

                do
                {
                    if (token.Type == TokenTypes.RParen)
                    {
                        break;
                    }

                    if (token.Type == TokenTypes.Comma)
                    {
                        _parser.Consume();
                    }

                    CompileExpression();

                    token = _parser.LA();

                } while (true);

                _parser.Consume();	//RPAREN
            }
            else
            {
                ThrowUnexpectedToken(token, TokenTypes.LParen);
            }
        }

        /// <summary>
        /// Adds an 'OpCode' to the _ops list, and consumes the parser
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Newly created OpCode</returns>
        private OpCode AddOpCode(Token token)
        {
            OpCode op = new OpCode(token);

            _ops.Add(op);

            if (op.Token.Type == TokenTypes.Id)
            {
                //  make sure the identifier is valid
                if (!ExpressionMethods.Default.IsMethodRegistered(token.Text))
                {
                    throw new InvalidOperationException(string.Format(
                        "[{0}:{1}] Invalid Method/Function found, '{2}' does not exist",
                        token.Line,
                        token.Column,
                        token.Text
                        ));
                }

                op.Value = ExpressionMethods.Default[token.Text];
            }

            return op;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="types"></param>
        private void ThrowUnexpectedToken(Token token, params TokenTypes[] types)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < types.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(types[i]);
            }

            throw new InvalidOperationException(string.Format(
                "[{0}:{1}] Expected '{2}', found '{3}'",
                token.Line,
                token.Column,
                sb.ToString(),
                token.Type
                ));
        }

        #endregion
    }
}
