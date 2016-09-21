﻿#region COPYRIGHT
//
//     THIS IS GENERATED BY TEMPLATE
//     
//     AUTHOR  :     ROYE
//     DATE       :     2010
//
//     COPYRIGHT (C) 2010, TIANXIAHOTEL TECHNOLOGIES CO., LTD. ALL RIGHTS RESERVED.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    /// <summary>
    /// 封装普通的<see cref="BindingFlags"/> 并提供不同附加的标志
    /// </summary>
    public struct Flags
    {
        #region BindingFlags

        /// <summary>
        /// 与值<see href="BindingFlags.Default">BindingFlags.Default</see>对应
        /// </summary>
        public static readonly Flags None = new Flags((long)BindingFlags.Default);

        /// <summary>
        /// 与值<see href="BindingFlags.IgnoreCase">BindingFlags.IgnoreCase</see> 对应
        /// </summary>
        public static readonly Flags IgnoreCase = new Flags((long)BindingFlags.IgnoreCase);

        /// <summary>
        /// 与值<see href="BindingFlags.DeclaredOnly">BindingFlags.DeclaredOnly</see>对应
        /// </summary>
        public static readonly Flags DeclaredOnly = new Flags((long)BindingFlags.DeclaredOnly);

        /// <summary>
        /// 与值<see href="BindingFlags.ExactBinding">BindingFlags.ExactBinding</see> 对应
        /// </summary>
        public static readonly Flags ExactBinding = new Flags((long)BindingFlags.ExactBinding);  // Note that this value is respected even in cases where normal Reflection calls would ignore it.

        /// <summary>
        /// 与值<see href="BindingFlags.Public">BindingFlags.Public</see>对应
        /// </summary>
        public static readonly Flags Public = new Flags((long)BindingFlags.Public);

        /// <summary>
        /// 与值<see href="BindingFlags.NonPublic">BindingFlags.NonPublic</see>对应
        /// </summary>
        public static readonly Flags NonPublic = new Flags((long)BindingFlags.NonPublic);

        /// <summary>
        /// 与值<see href="BindingFlags.Instance">BindingFlags.Instance</see>对应
        /// </summary>
        public static readonly Flags Instance = new Flags((long)BindingFlags.Instance);

        /// <summary>
        /// 与值<see href="BindingFlags.Static">BindingFlags.Static</see>对应
        /// </summary>
        public static readonly Flags Static = new Flags((long)BindingFlags.Static);

        #endregion

        #region Flags Selectors

        #region FasterflectFlags

        /// <summary>
        /// If this option is specified the search for a named member will perform a partial match instead
        /// of an exact match. If <see href="TrimExplicitlyImplemented"/> is specified the trimmed name is
        /// used instead of the original member name. If <see href="IgnoreCase"/> is specified the 
        /// comparison uses <see href="StringComparison.OrginalIgnoreCase"/> and otherwise
        /// uses <see href="StringComparison.Ordinal"/>.
        /// </summary>
        public static readonly Flags PartialNameMatch = new Flags(1L << 32);

        /// <summary>
        /// If this option is specified the search for a named member will strip off the namespace and
        /// interface name from explicitly implemented interface members before applying any comparison
        /// operations.
        /// </summary>
        public static readonly Flags TrimExplicitlyImplemented = new Flags(1L << 33);

        /// <summary>
        /// If this option is specified the search for members will exclude explicitly implemented
        /// interface members.
        /// </summary>
        public static readonly Flags ExcludeExplicitlyImplemented = new Flags(1L << 34);

        /// <summary>
        /// If this option is specified all members that are backers for another member, such as backing
        /// fields for automatic properties or get/set methods for properties, will be excluded from the 
        /// result.
        /// </summary>
        public static readonly Flags ExcludeBackingMembers = new Flags(1L << 35);

        /// <summary>
        /// If this option is specified the search for methods will avoid checking whether parameters
        /// have been declared as ref or out. This allows you to locate a method by its signature
        /// without supplying the exact details for every parameter.
        /// </summary>
        public static readonly Flags IgnoreParameterModifiers = new Flags(1L << 36);

        #endregion

        /// <summary>
        /// 查询条件包含公共的和非公共的所有成员(包括基成员)
        /// </summary>
        public static readonly Flags AnyVisibility = Public | NonPublic;

        /// <summary>
        /// 查询条件包含公共的和非公共的所有实例成员(包括基成员)
        /// </summary>
        public static readonly Flags InstanceAnyVisibility = AnyVisibility | Instance;

        /// <summary>
        /// 查询条件包含公共的和非公共的所有静态成员(包括基成员)
        /// </summary>
        public static readonly Flags StaticAnyVisibility = AnyVisibility | Static;

        /// <summary>
        /// 查询条件包含公共的和非公共的所有实例成员(不包括基成员)
        /// </summary>
        public static readonly Flags InstanceAnyDeclaredOnly = InstanceAnyVisibility | DeclaredOnly;

        /// <summary>
        /// 查询条件包含公共的和非公共的所有静态成员(不包括基成员)
        /// </summary>
        public static readonly Flags StaticAnyDeclaredOnly = StaticAnyVisibility | DeclaredOnly;

        /// <summary>
        /// 查询条件包含所有成员(包括基成员)
        /// </summary>
        public static readonly Flags StaticInstanceAnyVisibility = InstanceAnyVisibility | Static;

        #endregion

        private readonly long _Flags;
        private static readonly Dictionary<Flags, string> _FlagNames = new Dictionary<Flags, string>(64);

        static Flags()
        {
            #region 初始化FlagNames

            foreach (BindingFlags flag in Enum.GetValues(typeof(BindingFlags)))
            {
                if (flag != BindingFlags.Default)
                {
                    _FlagNames[new Flags((long)flag)] = flag.ToString();
                }
            }
            _FlagNames[PartialNameMatch] = "PartialNameMatch"; // new Flags( 1L << 32 );
            _FlagNames[TrimExplicitlyImplemented] = "TrimExplicitlyImplemented"; // new Flags( 1L << 33 );
            _FlagNames[ExcludeExplicitlyImplemented] = "ExcludeExplicitlyImplemented"; // = new Flags( 1L << 34 );
            _FlagNames[ExcludeBackingMembers] = "ExcludeBackingMembers"; // = new Flags( 1L << 35 );
            _FlagNames[IgnoreParameterModifiers] = "IgnoreParameterModifiers"; // = new Flags( 1L << 36 );

            // not yet supported:
            //flagNames[ VisibilityMatch ] = "VisibilityMatch"; // = new Flags( 1L << 55 );
            //flagNames[ Private ] = "Private"; //   = new Flags( 1L << 56 );
            //flagNames[ Protected ] = "Protected"; // = new Flags( 1L << 57 );
            //flagNames[ Internal ] = "Internal"; //  = new Flags( 1L << 58 );

            //flagNames[ ModifierMatch ] = "ModifierMatch"; // = new Flags( 1L << 59 );
            //flagNames[ Abstract ] = "Abstract"; //  = new Flags( 1L << 60 );
            //flagNames[ Virtual ] = "Virtual"; //   = new Flags( 1L << 61 );
            //flagNames[ Override ] = "Override"; //  = new Flags( 1L << 62 );
            //flagNames[ New ] = "New"; //      = new Flags( 1L << 63 );

            #endregion
        }

        private Flags(long flags)
        {
            _Flags = flags;
        }

        #region Helper Methods

        /// <summary>
        ///获取当前值是否设置了指定的绑定标志<paramref name="mask"/> 
        /// </summary>
        public bool IsSet(BindingFlags mask)
        {
            return ((BindingFlags)_Flags & mask) == mask;
        }

        /// <summary>
        ///获取当前值是否设置了指定的绑定标志<paramref name="mask"/>
        /// </summary>
        public bool IsSet(Flags mask)
        {
            return (_Flags & mask) == mask;
        }

        /// <summary>
        ///获取当前值是否设置了至少一个指定的绑定标志<paramref name="mask"/>
        /// </summary>
        public bool IsAnySet(BindingFlags mask)
        {
            return ((BindingFlags)_Flags & mask) != 0;
        }

        /// <summary>
        ///获取当前值是否设置了至少一个指定的绑定标志<paramref name="mask"/>
        /// </summary>
        public bool IsAnySet(Flags mask)
        {
            return (_Flags & mask) != 0;
        }

        /// <summary>
        ///获取当前值是否未设置了指定的绑定标志<paramref name="mask"/>
        /// </summary>
        public bool IsNotSet(BindingFlags mask)
        {
            return ((BindingFlags)_Flags & mask) == 0;
        }

        /// <summary>
        ///获取当前值是否未设置了指定的绑定标志<paramref name="mask"/>
        /// </summary>
        public bool IsNotSet(Flags mask)
        {
            return (_Flags & mask) == 0;
        }

        /// <summary>
        /// Returns a new Flags instance with the union of the values from <paramref name="flags"/> and 
        /// <paramref name="mask"/> if <paramref name="condition"/> is true, and otherwise returns the
        /// supplied <paramref name="flags"/>.
        /// </summary>
        public static Flags SetIf(Flags flags, Flags mask, bool condition)
        {
            return condition ? flags | mask : flags;
        }

        /// <summary>
        /// Returns a new Flags instance with the union of the values from <paramref name="flags"/> and 
        /// <paramref name="mask"/> if <paramref name="condition"/> is true, and otherwise returns a new 
        /// Flags instance with the values from <paramref name="flags"/> that were not in <paramref name="mask"/>.
        /// </summary>
        public static Flags SetOnlyIf(Flags flags, Flags mask, bool condition)
        {
            return condition ? flags | mask : (Flags)(flags & ~mask);
        }

        /// <summary>
        /// Returns a new Flags instance returns a new Flags instance with the values from <paramref name="flags"/> 
        /// that were not in <paramref name="mask"/> if <paramref name="condition"/> is true, and otherwise returns
        /// the supplied <paramref name="flags"/>.
        /// </summary>
        public static Flags ClearIf(Flags flags, Flags mask, bool condition)
        {
            return condition ? (Flags)(flags & ~mask) : flags;
        }

        #endregion

        /// <summary>
        /// 获取指定的Flag实例是否与当前值相同。
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(Flags) && _Flags == ((Flags)obj)._Flags;
        }

        /// <summary>
        ///产生一个唯一的散列码
        /// </summary>
        public override int GetHashCode()
        {
            return _Flags.GetHashCode();
        }

        public override string ToString()
        {
            Flags @this = this;
            List<string> names = _FlagNames.Where(kvp => @this.IsSet(kvp.Key))
                .Select(kvp => kvp.Value)
                .OrderBy(n => n).ToList();
            int index = 0;
            StringBuilder sb = new StringBuilder();
            names.ForEach(n => sb.AppendFormat("{0}{1}", n, ++index < names.Count ? " | " : ""));
            return sb.ToString();
        }

        /// <summary>
        /// 取得从<paramref name="f1"/>中而不在<paramref name="f2"/>的绑定标志
        /// </summary>
        public static Flags operator -(Flags f1, Flags f2)
        {
            return new Flags(f1._Flags & ~f2._Flags);
        }

        /// <summary>
        /// 取得<paramref name="f1"/>与<paramref name="f2"/>并集的绑定标志
        /// </summary>
        public static Flags operator |(Flags f1, Flags f2)
        {
            return new Flags(f1._Flags | f2._Flags);
        }

        /// <summary>
        /// 取得<paramref name="f1"/>与<paramref name="f2"/>交集的绑定标志。
        /// </summary>
        public static Flags operator &(Flags f1, Flags f2)
        {
            return new Flags(f1._Flags & f2._Flags);
        }

        /// <summary>
        /// 判断两个绑定标志是否相等。
        /// </summary>
        public static bool operator ==(Flags f1, Flags f2)
        {
            return f1._Flags == f2._Flags;
        }

        /// <summary>
        /// 判断两个绑定标志是否不相等。
        /// </summary>
        public static bool operator !=(Flags f1, Flags f2)
        {
            return f1._Flags != f2._Flags;
        }

        /// <summary>
        /// (隐式)将BindingFlags转成Flags类型.
        /// </summary>
        public static implicit operator Flags(BindingFlags m)
        {
            return new Flags((long)m);
        }

        /// <summary>
        /// (显式)将long转成Flags类型.
        /// </summary>
        public static explicit operator Flags(long m)
        {
            return new Flags(m);
        }

        /// <summary>
        /// (隐式)将Flags转成BindingFlags类型.
        /// </summary>
        public static implicit operator BindingFlags(Flags m)
        {
            return (BindingFlags)m._Flags;
        }

        /// <summary>
        /// (隐式)将Flags转成long类型.
        /// </summary>
        public static implicit operator long(Flags m)
        {
            return m._Flags;
        }
    }
}
