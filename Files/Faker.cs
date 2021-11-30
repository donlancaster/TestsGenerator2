using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakerLib.Interfaces;
using FakerLib.Generators;
using System.Reflection;


namespace FakerLib
{
    public class Faker : IFaker
    {
        private readonly List<Type> circularReferencesEncounter;

        private Dictionary<Type, IGenerator> generators;

        public Faker()
        {
            generators = new Dictionary<Type, IGenerator>
            {
                { typeof(bool), new BoolGenerator()},
                { typeof(char), new CharGenerator()},
                { typeof(double), new DoubleGenerator()},
                { typeof(int), new IntGenerator()},
                { typeof(string), new StringGenerator()},
            };
            PluginLoader loader = new PluginLoader(generators);
            loader.LoadPluginGeneratorsFromFiles();
            //      Console.WriteLine(generators.Count);
            circularReferencesEncounter = new List<Type>();
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        public object Create(Type type)
        {
            object instance;

            if (TryGenerateAbstract(type, out instance))
                return instance;

            if (TryGenerateKnown(type, out instance))
                return instance;

            if (TryGenerateEnum(type, out instance))
                return instance;

            if (TryGenerateArray(type, out instance))
                return instance;

            if (TryList(type, out instance))
                return instance;

            if (TryGenerateClass(type, out instance))
                return instance;

            return default;
        }

        public bool TryGenerateAbstract(Type type, out object instance)
        {
            instance = default;

            if (!type.IsAbstract)
                return false;

            return true;
        }

        private bool TryGenerateArray(Type type, out object instance)
        {
            instance = null;

            if (!type.IsArray)
                return false;

            instance = (new ArrayGenerator(this, type)).Create();

            return true;
        }

        private bool TryGenerateKnown(Type type, out object instance)
        {
            instance = null;
            if (generators.TryGetValue(type, out IGenerator generator))
            {
                instance = generator.Create();
                return true;
            }

            return false;
        }

        private bool TryGenerateEnum(Type type, out object instance)
        {
            instance = null;

            if (!type.IsEnum)
                return false;

            Array values = type.GetEnumValues();
            Random random = new Random();

            instance = values.GetValue(random.Next(0, values.Length));

            return true;
        }

        private bool TryList(Type type, out object instance)
        {
            instance = null;
            if (!type.IsGenericType)
                return false;

            if (!(type.GetGenericTypeDefinition() == typeof(List<>)))
                return false;

            var innerTypes = type.GetGenericArguments();
            Type gType = innerTypes[0];

            int count = new Random().Next(1, 20);
            instance = Activator.CreateInstance(type);
            object[] arr = new object[1];
            for (int i = 0; i < count; ++i)
            {
                arr[0] = Create(gType);
                type.GetMethod("Add").Invoke(instance, arr); //list.add(instance)
            }

            return true;
        }

        private bool TryGenerateClass(Type type, out object instance)
        {
            instance = null;

            if (!type.IsClass && !type.IsValueType)
                return false;

            if (circularReferencesEncounter.Contains(type))
            {
                instance = default;
                return true;
            }

            circularReferencesEncounter.Add(type);

            if (TryConstruct(type, out instance))
            {
                GenerateFillProps(instance, type);
                GenerateFillFields(instance, type);

                circularReferencesEncounter.Remove(type);

                return true;
            }

            return false;
        }

        private bool TryConstruct(Type type, out object instance)
        {
            instance = null;

            if (TryGetMaxParamsConstructor(type, out ConstructorInfo ctn))
            {
                var prms = GenerateConstructorParams(ctn);

                instance = ctn.Invoke(prms);

                return true;
            }

            return false;
        }

        private bool TryGetMaxParamsConstructor(Type type, out ConstructorInfo constructor)
        {
            constructor = null;

            var constructors = type.GetConstructors();

            if (constructors.Length == 0)
                return false;
            Array.Sort(constructors, Comparer<ConstructorInfo>.Create(
                (c1, c2) =>
                c2.GetParameters().Length.CompareTo(c1.GetParameters().Length)));

            foreach (var con in constructors)
            {
                if (con.IsPublic)
                {
                    try
                    {
                        
                      con.Invoke(GenerateConstructorParams(con));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    
                    constructor = con;
                }
            }

            if (constructor == null)
                return false;

            return true;
        }

        private void GenerateFillProps(object instance, Type type)
        {
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                if (prop.GetSetMethod() == null)
                    continue;

                prop.SetValue(instance, Create(prop.PropertyType));
            }
        }

        private void GenerateFillFields(object instance, Type type)
        {
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                if (!field.IsPublic)
                    continue;

                field.SetValue(instance, Create(field.FieldType));
            }
        }

        private object[] GenerateConstructorParams(ConstructorInfo constructor)
        {
            var prms = constructor.GetParameters();

            object[] generated = new object[prms.Length];

            for (int i = 0; i < prms.Length; i++)
            {
                var p = prms[i];

                generated[i] = Create(p.ParameterType);
            }

            return generated;
        }
    }
}
