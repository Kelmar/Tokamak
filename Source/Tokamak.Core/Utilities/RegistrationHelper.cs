using System;

using Stashbox;
using Stashbox.Registration.Fluent;

namespace Tokamak.Core.Utilities
{
    public delegate void SimpleEvent();
    public delegate void SimpleEvent<T>(T arg);

    public static class RegistrationHelper
    {
        public static void TryRegister<T>(this IStashboxContainer container, object name = null)
            where T : class
        {
            if (container.IsRegistered<T>())
                return;

            container.Register<T>(name);
        }

        public static void TryRegister<T>(this IStashboxContainer container, Action<RegistrationConfigurator<T, T>> configurator)
            where T : class
        {
            if (container.IsRegistered<T>())
                return;

            container.Register<T>(configurator);
        }

        public static void TryRegister<TFrom>(this IStashboxContainer container, Type typeTo, Action<RegistrationConfigurator<TFrom, TFrom>> configurator = null)
            where TFrom : class
        {
            if (container.IsRegistered<TFrom>())
                return;

            container.Register(typeTo, configurator);
        }

        public static void TryRegister<TFrom, TTo>(this IStashboxContainer container, object name = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            if (container.IsRegistered<TFrom>())
                return;

            container.Register<TFrom, TTo>(name);
        }

        public static void TryRegister<TFrom, TTo>(this IStashboxContainer container, Action<RegistrationConfigurator<TFrom, TTo>> configurator)
            where TFrom : class
            where TTo : class, TFrom
        {
            if (container.IsRegistered<TFrom>())
                return;

            container.Register(configurator);
        }
    }
}
