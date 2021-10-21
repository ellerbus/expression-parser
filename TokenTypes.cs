using System;
using System.ComponentModel;

namespace StockSniper.Library.ExpressionEngine
{
    enum TokenTypes
    {
        /// <summary>
        /// Left Paren (
        /// </summary>
        [Description("left paren")]
        LParen,
        /// <summary>
        /// Right Paren )
        /// </summary>
        [Description("right paren")]
        RParen,
        /// <summary>
        /// Comma ,
        /// </summary>
        [Description("comma")]
        Comma,

        /// <summary>
        /// Equal = or ==
        /// </summary>
        [Description("=")]
        Eq,

        /// <summary>
        /// Not Equal != or <![CDATA[<>]]> 
        /// </summary>
        [Description("!=")]
        Ne,

        /// <summary>
        /// Greater Than <![CDATA[>]]> 
        /// </summary>
        [Description(">")]
        Gt,

        /// <summary>
        /// Greater Than or Equal <![CDATA[>=]]> 
        /// </summary>
        [Description(">=")]
        Ge,

        /// <summary>
        /// Less Than <![CDATA[<]]> 
        /// </summary>
        [Description("<")]
        Lt,

        /// <summary>
        /// Less Than or Equal <![CDATA[<=]]> 
        /// </summary>
        [Description(">=")]
        Le,

        /// <summary>
        /// And && or 'and'
        /// </summary>
        [Description("and")]
        And,

        /// <summary>
        /// Or || or 'or'
        /// </summary>
        [Description("or")]
        Or,

        /// <summary>
        /// Not ! or 'not'
        /// </summary>
        [Description("not")]
        Not,

        /// <summary>
        /// Add +
        /// </summary>
        [Description("+")]
        Add,

        /// <summary>
        /// Subtract -
        /// </summary>
        [Description("-")]
        Sub,

        /// <summary>
        /// Unary Negate Operation -
        /// </summary>
        [Description("-")]
        Neg,

        /// <summary>
        /// Multiply *
        /// </summary>
        [Description("*")]
        Mul,

        /// <summary>
        /// Divide /
        /// </summary>
        [Description("/")]
        Div,

        /// <summary>
        /// Power ^
        /// </summary>
        [Description("^")]
        Pow,

        ///// <summary>
        ///// Percent %
        ///// </summary>
        //[Description("%")]
        //Per,

        /// <summary>
        /// Identifier
        /// </summary>
        [Description("identifier")]
        Id,

        /// <summary>
        /// Number
        /// </summary>
        [Description("number")]
        Num,

        /// <summary>
        /// String
        /// </summary>
        [Description("string")]
        Str,

        /// <summary>
        /// End of File
        /// </summary>
        [Description("end of file")]
        EOF
    }
}
