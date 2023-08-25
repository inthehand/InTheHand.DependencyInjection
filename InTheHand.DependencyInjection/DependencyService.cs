//-----------------------------------------------------------------------
// <copyright file="DependencyService.cs" company="In The Hand Ltd">
//   Copyright (c) 2023 In The Hand Ltd, All rights reserved.
//   This source code is licensed under the MIT License
// </copyright>
//-----------------------------------------------------------------------

namespace InTheHand.DependencyInjection
{
    /// <summary>
    /// Static class that provides the <see cref="Get{T}(DependencyFetchTarget)"/> factory method for retrieving platform-specific implementations of the specified type T.
    /// </summary>
    /// <remarks>Ported from the Xamarin.Forms implementation. Redundant renderer specific implementation details have been removed.</remarks>
    public static partial class DependencyService
    {
        static readonly object s_dependencyLock = new object();

        static readonly List<Type> DependencyTypes = new List<Type>();
        static readonly Dictionary<Type, DependencyData> DependencyImplementations = new Dictionary<Type, DependencyData>();

        /// <summary>
        /// Returns the platform-specific implementation of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to fetch.</typeparam>
        /// <param name="fetchTarget">The dependency fetch target.</param>
        /// <returns>T</returns>
        public static T? Get<T>(DependencyFetchTarget fetchTarget = DependencyFetchTarget.GlobalInstance) where T : class
        {
            DependencyData dependencyImplementation;
            lock (s_dependencyLock)
            {
                Type targetType = typeof(T);
                if (!DependencyImplementations.TryGetValue(targetType, out dependencyImplementation))
                {
                    Type implementor = FindImplementor(targetType);
                    DependencyImplementations[targetType] = (dependencyImplementation = implementor != null ? new DependencyData { ImplementorType = implementor } : null);
                }
            }

            if (dependencyImplementation == null)
                return null;

            if (fetchTarget == DependencyFetchTarget.GlobalInstance)
            {
                if (dependencyImplementation.GlobalInstance == null)
                {
                    lock (dependencyImplementation)
                    {
                        if (dependencyImplementation.GlobalInstance == null)
                        {
                            dependencyImplementation.GlobalInstance = ActivateWithParameters(dependencyImplementation.ImplementorType);
                        }
                    }
                }
                return (T)dependencyImplementation.GlobalInstance;
            }

            return (T)ActivateWithParameters(dependencyImplementation.ImplementorType);
        }

        private static object ActivateWithParameters(Type implementorType)
        {
            // resolve constructor parameters
            List<object> parameters = new List<object>();
            var ci = implementorType.GetConstructors().FirstOrDefault();
            foreach (var pt in ci.GetParameters())
            {
                parameters.Add(Get(pt.ParameterType));
            }
            return Activator.CreateInstance(implementorType, parameters.ToArray());
        }

        internal static object Get(Type targetType, DependencyFetchTarget fetchTarget = DependencyFetchTarget.GlobalInstance)
        {
            DependencyData dependencyImplementation;
            lock (s_dependencyLock)
            {
                if (!DependencyImplementations.TryGetValue(targetType, out dependencyImplementation))
                {
                    Type implementor = FindImplementor(targetType);
                    DependencyImplementations[targetType] = (dependencyImplementation = implementor != null ? new DependencyData { ImplementorType = implementor } : null);
                }
            }

            if (dependencyImplementation == null)
                return null;

            if (fetchTarget == DependencyFetchTarget.GlobalInstance)
            {
                if (dependencyImplementation.GlobalInstance == null)
                {
                    lock (dependencyImplementation)
                    {
                        if (dependencyImplementation.GlobalInstance == null)
                        {
                            dependencyImplementation.GlobalInstance = Activator.CreateInstance(dependencyImplementation.ImplementorType);
                        }
                    }
                }
                return dependencyImplementation.GlobalInstance;
            }

            // resolve constructor parameters
            List<object> parameters = new List<object>();
            var ci = dependencyImplementation.ImplementorType.GetConstructors().FirstOrDefault();
            foreach (var pt in ci.GetParameters())
            {
                parameters.Add(Get(pt.ParameterType));
            }
            return Activator.CreateInstance(dependencyImplementation.ImplementorType, parameters);
        }

        /// <summary>
        /// Registers the platform-specific implementation of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to register.</typeparam>
        public static void Register<T>() where T : class
        {
            Type type = typeof(T);
            if (!DependencyTypes.Contains(type))
                DependencyTypes.Add(type);
        }

        /// <summary>
        /// Registers the platform-specific implementation of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to register.</typeparam>
        /// <typeparam name="TImpl">The implementation to register.</typeparam>
        public static void Register<T, TImpl>() where T : class where TImpl : class, T
        {
            Type targetType = typeof(T);
            Type implementorType = typeof(TImpl);
            if (!DependencyTypes.Contains(targetType))
                DependencyTypes.Add(targetType);

            lock (s_dependencyLock)
                DependencyImplementations[targetType] = new DependencyData { ImplementorType = implementorType };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public static void RegisterSingleton<T>(T instance) where T : class
        {
            Type targetType = typeof(T);
            Type implementorType = typeof(T);
            if (!DependencyTypes.Contains(targetType))
                DependencyTypes.Add(targetType);

            lock (s_dependencyLock)
                DependencyImplementations[targetType] = new DependencyData { ImplementorType = implementorType, GlobalInstance = instance };
        }

        static Type FindImplementor(Type target) =>
            DependencyTypes.FirstOrDefault(t => target.IsAssignableFrom(t));



        class DependencyData
        {
            public object? GlobalInstance { get; set; }

            public Type? ImplementorType { get; set; }
        }
    }
}