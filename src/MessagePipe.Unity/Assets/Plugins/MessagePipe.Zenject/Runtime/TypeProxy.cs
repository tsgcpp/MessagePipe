using MessagePipe.Zenject;
using System;
using Zenject;
using Zenject.Internal;

namespace MessagePipe.Zenject
{
    internal struct DiContainerProxy : IServiceCollection
    {
        readonly DiContainer builder;

        public DiContainerProxy(DiContainer builder)
        {
            this.builder = builder;
        }

        public void AddTransient(Type type)
        {
            builder.Bind(type).AsTransient();
        }

        public void TryAddTransient(Type type)
        {
            if (!builder.HasBinding(type))
            {
                builder.Bind(type).AsTransient();
            }
        }

        public void AddSingleton<T>(T instance)
        {
            builder.BindInstance(instance).AsSingle();
        }

        public void AddSingleton(Type type)
        {
            builder.Bind(type).AsSingle();
        }

        public void Add<T>()
        {
            builder.Bind<T>().AsCached();
        }

        public void Add<TService, TImplementation>()
            where TImplementation : TService
        {
            builder.Bind<TService>().To<TImplementation>().AsCached();
        }

        public void Add<TService1, TService2, TImplementation>()
            where TImplementation : TService1, TService2
        {
            builder
                .Bind(typeof(TService1), typeof(TService2))
                .To<TImplementation>()
                .AsCached();
        }
    }

    [Preserve]
    public sealed class DiContainerProviderProxy : IServiceProvider
    {
        DiContainer container;

        [Preserve]
        public DiContainerProviderProxy(DiContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }
    }
}

namespace MessagePipe
{
    public static partial class DiContainerExtensions
    {
        public static IServiceProvider AsServiceProvider(this DiContainer container)
        {
            return new DiContainerProviderProxy(container);
        }
    }
}
