using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ii
{
    


    static class Reflector
    {

        private static Dictionary<string, Type> aliasDictionary = new Dictionary<string, Type>
    {
        { "bool", typeof(bool)},
        { "byte", typeof(byte)},
        { "sbyte", typeof(sbyte)},
        { "char", typeof(char)},
        { "decimal", typeof(decimal)},
        { "double", typeof(double)},
        { "float", typeof(float)},
        { "int", typeof(int)},
        { "uint", typeof(uint)},
        { "long", typeof(long)},
        { "ulong", typeof(ulong)},
        { "object", typeof(object)},
        { "short", typeof(short)},
        { "ushort", typeof(ushort)},
        { "string", typeof(string)},
        { "void", typeof(void) }
    };

        public static Type GetType(string name) =>
            aliasDictionary.TryGetValue(name, out var result) ?
            result :
            Type.GetType(name) ??
            AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes()).FirstOrDefault(type => type.Name == name);


       
        public static string GetAssemblyName(string name) => GetAssemblyName(GetType(name));
        public static string GetAssemblyName(Type type) => type.Assembly.FullName;

     

        public static bool HasPublicConstructor(string name) => HasPublicConstructor(GetType(name));

     
        public static bool HasPublicConstructor(Type type) => type.GetConstructors().Length != 0;


        public static IEnumerable<string> GetPublicMethods(Type type)
            => type.GetMethods().Select(method => method.GetDeclaration());
                

        public static IEnumerable<string> GetPublicMethods(string name) => GetPublicMethods(GetType(name));



        public static IEnumerable<string> GetFields(Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Select(field =>
            $"{(field.IsPublic ? "public" : field.IsPrivate ? "private" : "protected")} {(field.IsStatic ? "static " : "")}{field.FieldType.GetFriendlyName()} " +
            $"{field.Name}");

        public static IEnumerable<string> GetFields(string name) => GetFields(GetType(name));


        public static IEnumerable<string> GetProperties(Type type) =>
          type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Select(property =>
            $"{property.PropertyType.GetFriendlyName()} {property.Name} {{{string.Join("; ", property.GetAccessors(true).Select(accesor => $"{(accesor.IsPublic ? "public" : accesor.IsPrivate ? "private" : "protected ")} {(accesor.ReturnType == typeof(void) ? "set" : "get")}"))};}}");



        public static IEnumerable<string> GetProperties(string name) => GetProperties(GetType(name));

        public static IEnumerable<string> GetInterfaces(Type type) => type.GetInterfaces().Select(@interface => @interface.GetFriendlyName());

        public static IEnumerable<string> GetInterfaces(string name) => GetInterfaces(GetType(name));


        public static IEnumerable<string> GetMethodsWithParamType(Type type, Type search) =>
            type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
            .Where(method => method.GetParameters().Any(parameter => parameter.ParameterType == search)).Select(method
                => method.GetDeclaration());

        public static IEnumerable<string> GetMethodsWithParamType(string name, string search) => GetMethodsWithParamType(GetType(name), GetType(search));

        public static object Create(Type type, params object[] arguments) => arguments.Length == 0? Activator.CreateInstance(type) : Activator.CreateInstance(type, arguments);


        public static object Create(string name, params object[] arguments) => Create(GetType(name), arguments);

        public static object Invoke<T>(T obj, string methodName, params string[] arguments)
        {
            foreach (var method in typeof(T).GetMethods().Where(method => method.Name == methodName))
            {
                var parameters = method.GetParameters();
                if (parameters.Length == arguments.Length)
                {
                    var convertedArgs = parameters.Zip(arguments).Select(parArg => Convert.ChangeType(parArg.Second, parArg.First.ParameterType));
                    if (convertedArgs.All(arg => arg != null))
                    {
                        return method.Invoke(obj, convertedArgs.ToArray());
                    }
                }
            }

            return null;
        }

        public static object Invoke<T>(T obj, string methodName, string fileName) => Invoke(obj, methodName, File.ReadLines(fileName).First().Split());



        public static void ToFile(string fileName, string className)
        {
            var type = GetType(className);

            using (var sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine($"Есть ли публичные конструкторы? {HasPublicConstructor(type)}");


                sw.WriteLine($"Публичные методы:");

                foreach(var method in GetPublicMethods(type))
                {
                    sw.WriteLine(method);
                }


                sw.WriteLine($"Реализованные интерфесы:");

                foreach (var @interface in GetInterfaces(type))
                {
                    sw.WriteLine(@interface);
                }


                sw.WriteLine($"Все методы и свойства: ");

                foreach (var method in GetFields(type).Concat(GetProperties(type)))
                {
                    sw.WriteLine(method);
                }

                sw.WriteLine($"Имя сборки: ");

                sw.WriteLine(GetAssemblyName(type));

             
            }

        }








    }
   public class Program
    {
       
        static void Main(string[] args)
        {

            var arrayList = (ArrayList)Reflector.Create("ArrayList");
            arrayList.Add(true);



            Console.WriteLine(Reflector.Invoke("1234more", "IndexOf", arguments: "4"));
            
            Reflector.ToFile("lalala.txt", "string");


             
           
         
      
       














            // Console.WriteLine(Reflector.Invoke("0444", "IndexOf", arguments: "4"));

            //  Console.WriteLine(Reflector.Invoke("0444", "IndexOf", fileName: "тут"));

        }
    }
}
