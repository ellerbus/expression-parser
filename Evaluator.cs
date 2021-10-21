using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using StockSniper.Library.Utilities;

namespace StockSniper.Library.ExpressionEngine
{
    public delegate object EvaluateStringValueHandler(string text);

    public sealed class Evaluator
    {
        #region Member Variables

        struct PairOfNumbers
        {
            public double Lhs;
            public double Rhs;
        }

        struct PairOfBooleans
        {
            public bool Lhs;
            public bool Rhs;
        }

        private object _container;
        private List<OpCode> _ops;
        private Stack<object> _stack;

        /// <summary>
        /// Fired when a string value is encountered and it's
        /// underlying meaning or value is determined by the
        /// 'caller'
        /// </summary>
        public EvaluateStringValueHandler EvaluateStringValue;

        #endregion

        #region Constructors

        public Evaluator()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Evaluator(object container)
            : this()
        {
            _container = container;

            Type t = _container.GetType();

            if (!ExpressionMethods.Default.IsTypeRegistered(t))
            {
                ExpressionMethods.Default.RegisterMethodsOf(t);
            }
        }

        #endregion

        #region Evaluation/Interpretation Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public void Validate(string expression)
        {
            Compiler c = new Compiler();

            c.Compile(expression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public double Evaluate(string expression)
        {
            Compiler c = new Compiler();

            _ops = new List<OpCode>(c.Compile(expression));

            _stack = new Stack<object>();

            Interpret();

            return Convert.ToDouble(_stack.Pop());
        }

        private void Interpret()
        {
            foreach (OpCode op in _ops)
            {
                switch (op.Token.Type)
                {
                    case TokenTypes.Num:
                        _stack.Push(GeneralHelper.GetDecimal(op.Token.Text));
                        break;

                    case TokenTypes.Str:
                        _stack.Push(OnEvaluateStringValue(op.Token.Text));
                        break;

                    case TokenTypes.Add:
                    case TokenTypes.Sub:
                    case TokenTypes.Mul:
                    case TokenTypes.Div:
                    case TokenTypes.Pow:
                        MathOp(op.Token.Type);
                        break;

                    case TokenTypes.Eq:
                    case TokenTypes.Ne:
                    case TokenTypes.Lt:
                    case TokenTypes.Le:
                    case TokenTypes.Gt:
                    case TokenTypes.Ge:
                        RelationalOp(op.Token.Type);
                        break;

                    case TokenTypes.And:
                    case TokenTypes.Or:
                        LogicalOp(op.Token.Type);
                        break;

                    case TokenTypes.Neg:
                        Neg();
                        break;

                    case TokenTypes.Not:
                        Not();
                        break;

                    case TokenTypes.Id:
                        MethodCall(op);
                        break;

                    default:
                        throw new InvalidOperationException("Unhandled Token Type '" + op.Token.Type + "'");
                }
            }
        }

        private void MathOp(TokenTypes operation)
        {
            PairOfNumbers pair = PopPairOfNumbers();

            double result = 0;

            switch (operation)
            {
                case TokenTypes.Add:
                    result = pair.Lhs + pair.Rhs;
                    break;

                case TokenTypes.Sub:
                    result = pair.Lhs - pair.Rhs;
                    break;

                case TokenTypes.Mul:
                    result = pair.Lhs * pair.Rhs;
                    break;

                case TokenTypes.Div:
                    result = pair.Lhs / pair.Rhs;
                    break;

                case TokenTypes.Pow:
                    result = Math.Pow(pair.Lhs, pair.Rhs);
                    break;
            }

            _stack.Push(result);
        }

        private void RelationalOp(TokenTypes operation)
        {
            PairOfNumbers pair = PopPairOfNumbers();

            bool result = false;

            switch (operation)
            {
                case TokenTypes.Eq:
                    result = pair.Lhs == pair.Rhs;
                    break;

                case TokenTypes.Ne:
                    result = pair.Lhs != pair.Rhs;
                    break;

                case TokenTypes.Lt:
                    result = pair.Lhs < pair.Rhs;
                    break;

                case TokenTypes.Le:
                    result = pair.Lhs <= pair.Rhs;
                    break;

                case TokenTypes.Gt:
                    result = pair.Lhs > pair.Rhs;
                    break;

                case TokenTypes.Ge:
                    result = pair.Lhs >= pair.Rhs;
                    break;
            }

            _stack.Push(result);
        }

        private void LogicalOp(TokenTypes operation)
        {
            PairOfBooleans pair = PopPairOfBooleans();

            bool result = false;

            switch (operation)
            {
                case TokenTypes.And:
                    result = pair.Lhs && pair.Rhs;
                    break;

                case TokenTypes.Or:
                    result = pair.Lhs || pair.Rhs;
                    break;
            }

            _stack.Push(Convert.ToDouble(result));
        }

        private void Neg()
        {
            double atom = PopNumber();

            _stack.Push(-atom);
        }

        private void Not()
        {
            double atom = PopNumber();

            bool result = !Convert.ToBoolean(atom);

            _stack.Push(Convert.ToDouble(result));
        }

        private void MethodCall(OpCode op)
        {
            MethodInfo method = op.Value as MethodInfo;

            ParameterInfo[] parms = method.GetParameters();

            object[] parameters = new object[parms.Length];

            for (int i = parms.Length - 1; i > -1; i--)
            {
                object parmValue = _stack.Pop();

                if (parms[i].ParameterType == typeof(double))
                {
                    parmValue = Convert.ToDouble(parmValue);
                }
                else if (parms[i].ParameterType == typeof(bool))
                {
                    parmValue = Convert.ToBoolean(parmValue);
                }
                else if (parms[i].ParameterType == typeof(string))
                {
                    parmValue = Convert.ToString(parmValue);
                }

                parameters[i] = parmValue;
            }

            object result = method.Invoke(
                method.IsStatic ? null : _container,
                parameters
                );

            if (method.ReturnType == typeof(bool))
            {
                result = Convert.ToDouble(result);
            }

            _stack.Push(result);
        }

        private double PopNumber()
        {
            return (double)_stack.Pop();
        }

        private bool PopBoolean()
        {
            return Convert.ToBoolean(_stack.Pop());
        }

        private PairOfNumbers PopPairOfNumbers()
        {
            PairOfNumbers pair = new PairOfNumbers();

            pair.Rhs = PopNumber();
            pair.Lhs = PopNumber();

            return pair;
        }

        private PairOfBooleans PopPairOfBooleans()
        {
            PairOfBooleans pair = new PairOfBooleans();

            pair.Rhs = PopBoolean();
            pair.Lhs = PopBoolean();

            return pair;
        }

        #endregion

        #region Custom Methods

        private object OnEvaluateStringValue(string text)
        {
            if (EvaluateStringValue != null)
            {
                return EvaluateStringValue(text);
            }

            return 0;
        }

        #endregion

        #region Properties

        #endregion
    }
}
