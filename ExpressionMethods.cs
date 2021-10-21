using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace StockSniper.Library.ExpressionEngine
{
    sealed class ExpressionMethods
    {
        #region Member Variables

        private HashSet<string> _registeredTypes;

        private Dictionary<string, MethodInfo> _registeredMethods;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static ExpressionMethods()
        {
            Default = new ExpressionMethods();

            Assembly a = typeof(ExpressionMethods).Assembly;

            foreach (Type type in a.GetTypes())
            {
                if (type.IsClass)
                {
                    if (!type.IsAbstract && !type.IsGenericType)
                    {
                        Default.RegisterMethodsOf(type);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ExpressionMethods()
        {
            _registeredTypes = new HashSet<string>();
            _registeredMethods = new Dictionary<string, MethodInfo>(32);

            RegisterMethodsOf(GetType());
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public bool IsTypeRegistered(Type type)
        {
            lock (_registeredTypes)
            {
                return _registeredTypes.Contains(type.FullName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterMethodsOf(Type type)
        {
            if (IsTypeRegistered(type))
            {
                return;
            }

            BindingFlags flags = BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public
                ;

            RegisterMethods(type.GetMethods(flags));

            lock (_registeredTypes)
            {
                _registeredTypes.Add(type.FullName);
            }
        }

        private void RegisterMethods(MethodInfo[] methods)
        {
            lock (_registeredMethods)
            {
                foreach (MethodInfo method in methods)
                {
                    object[] attributes = method.GetCustomAttributes(
                        typeof(ExpressionMethodAttribute),
                        true
                        );

                    if (attributes != null && attributes.Length > 0)
                    {
                        InspectMethod(method);

                        ExpressionMethodAttribute ema = attributes[0] as ExpressionMethodAttribute;

                        if (!_registeredMethods.ContainsKey(ema.Name))
                        {
                            //    throw new InvalidOperationException(
                            //        "Method '" + ema.Name + "' already exists"
                            //        );
                            //}

                            _registeredMethods.Add(ema.Name, method);
                        }
                    }
                }
            }
        }

        private void InspectMethod(MethodInfo method)
        {
            if (method.ReturnType != typeof(double))
            {
                throw new InvalidOperationException(
                    "Expression Methods must have" +
                    " a return type a double: '" +
                    method.Name + "'"
                    );
            }

            foreach (ParameterInfo pi in method.GetParameters())
            {
                if (pi.ParameterType != typeof(double) &&
                    pi.ParameterType != typeof(bool) &&
                    pi.ParameterType != typeof(string))
                {
                    throw new InvalidOperationException(
                        "Expression Method Parameters can only accept" +
                        " either boolean, double, or string: '" +
                        method.Name + "'"
                        );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMethodRegistered(string name)
        {
            return _registeredMethods.ContainsKey(name.ToUpper());
        }

        #endregion

        #region Static Expression Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [ExpressionMethod("isnan")]
        public static double IsNan(double number)
        {
            return Convert.ToDouble(double.IsNaN(number));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [ExpressionMethod("iif")]
        public static double AlternateIf(bool condition, double trueValue, double falseValue)
        {
            return If(condition, trueValue, falseValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [ExpressionMethod("if")]
        public static double If(bool condition, double trueValue, double falseValue)
        {
            if (condition)
            {
                return trueValue;
            }

            return falseValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [ExpressionMethod("abs")]
        public static double Abs(double number)
        {
            return Math.Abs(number);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [ExpressionMethod("round")]
        public static double Round(double number, double digits)
        {
            return Math.Round(number, (int)digits, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MethodInfo this[string name]
        {
            get { return _registeredMethods[name]; }
        }

        public static ExpressionMethods Default
        {
            get;
            private set;
        }

        #endregion
    }
}
