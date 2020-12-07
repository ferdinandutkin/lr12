using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ii
{

    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
    }
    public static class TypeExtensions
    {

        public static string GetDeclaration(this MethodInfo method) =>
         $"{(method.IsPublic ? "public" : method.IsPrivate ? "private" : "protected")}" +
                $"{(method.IsStatic ? " static" : "")} {method.ReturnType.GetFriendlyName()} {method.Name}" +
                $"({string.Join(", ", method.GetParameters().Select(info => $"{info.ParameterType.GetFriendlyName()} {info.Name}"))})";




        private static Dictionary<Type, string> aliasDictionary = new Dictionary<Type, string>
    {
        {typeof(bool), "bool"},
        {typeof(byte), "byte"},
        {typeof(sbyte), "sbyte"},
        {typeof(char), "char"},
        {typeof(decimal), "decimal"},
        {typeof(double), "double"},
        {typeof(float), "float"},
        {typeof(int), "int"},
        {typeof(uint), "uint"},
        {typeof(long), "long"},
        {typeof(ulong), "ulong"},
        {typeof(object), "object"},
        {typeof(short), "short"},
        {typeof(ushort), "ushort"},
        {typeof(string), "string"},
        {typeof(void), "void" }
    };

        public static string GetFriendlyName(this Type type)
        {
            if (type.IsArray)
                return type.GetFriendlyNameOfArrayType();
            if (type.IsGenericType)
                return type.GetFriendlyNameOfGenericType();
            if (type.IsPointer)
                return type.GetFriendlyNameOfPointerType();
           
            return aliasDictionary.TryGetValue(type, out var aliasName)
                ? aliasName
                : type.Name;
        }

        private static string GetFriendlyNameOfArrayType(this Type type)
        {
            var arrayBuilder = new StringBuilder();
            while (type.IsArray)
            {
                arrayBuilder.Append($"[{new string(',', type.GetArrayRank() - 1)}]");
                type = type.GetElementType();
            }
            return type.GetFriendlyName() + arrayBuilder.ToString();
        }

        private static string GetFriendlyNameOfGenericType(this Type type)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments().First().GetFriendlyName() + "?";
            var friendlyName = type.Name;
            var indexOfBacktick = friendlyName.IndexOf('`');
            if (indexOfBacktick > 0)
                friendlyName = friendlyName.Remove(indexOfBacktick);
            var typeParameterNames = type
                .GetGenericArguments()
                .Select(typeParameter => typeParameter.GetFriendlyName());
        
            return $"{friendlyName}<{string.Join(", ", typeParameterNames)}>";
        }

        private static string GetFriendlyNameOfPointerType(this Type type) =>
            type.GetElementType().GetFriendlyName() + "*";
    }
}
