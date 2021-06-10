using System;
using Zenject;
using MessagePipe.Zenject;

namespace MessagePipe
{
    public static partial class DiContainerExtensions
    {
        // original is ServiceCollectionExtensions, trimed openegenerics register.

        public static MessagePipeOptions BindMessagePipe(this DiContainer builder)
        {
            return BindMessagePipe(builder, _ => { });
        }

        public static MessagePipeOptions BindMessagePipe(this DiContainer builder, Action<MessagePipeOptions> configure)
        {
            MessagePipeOptions options = null;
            var proxy = new DiContainerProxy(builder);
            proxy.AddMessagePipe(x =>
            {
                configure(x);
                options = x;
            });

            builder.Bind<IServiceProvider>().To<DiContainerProviderProxy>().AsCached();

            return options;
        }

        /// <summary>Register IPublisher[TMessage] and ISubscriber[TMessage](includes Async/Buffered) to container builder.</summary>
        public static DiContainer BindMessageBroker<TMessage>(this DiContainer builder, MessagePipeOptions options)
        {
            var services = new DiContainerProxy(builder);

            // keyless PubSub
            services.Add<MessageBrokerCore<TMessage>>();
            services.Add<IPublisher<TMessage>, ISubscriber<TMessage>, MessageBroker<TMessage>>();

            // keyless PubSub async
            services.Add<AsyncMessageBrokerCore<TMessage>>();
            services.Add<IAsyncPublisher<TMessage>, IAsyncSubscriber<TMessage>, AsyncMessageBroker<TMessage>>();

            // keyless buffered PubSub
            services.Add<BufferedMessageBrokerCore<TMessage>>();
            services.Add<IBufferedPublisher<TMessage>, IBufferedSubscriber<TMessage>, BufferedMessageBroker<TMessage>>();

            // keyless buffered PubSub async
            services.Add<BufferedAsyncMessageBrokerCore<TMessage>>();
            services.Add<IBufferedAsyncPublisher<TMessage>, IBufferedAsyncSubscriber<TMessage>, BufferedAsyncMessageBroker<TMessage>>();

            return builder;
        }

        /// <summary>Register IPublisher[TKey, TMessage] and ISubscriber[TKey, TMessage](includes Async) to container builder.</summary>
        public static DiContainer BindMessageBroker<TKey, TMessage>(this DiContainer builder, MessagePipeOptions options)
        {
            var services = new DiContainerProxy(builder);

            // keyed PubSub
            services.Add<MessageBrokerCore<TKey, TMessage>>();
            services.Add<IPublisher<TKey, TMessage>, ISubscriber<TKey, TMessage>, MessageBroker<TKey, TMessage>>();

            // keyed PubSub async
            services.Add<AsyncMessageBrokerCore<TKey, TMessage>>();
            services.Add<IAsyncPublisher<TKey, TMessage>, IAsyncSubscriber<TKey, TMessage>, AsyncMessageBroker<TKey, TMessage>>();

            return builder;
        }

        /// <summary>Register IRequestHandler[TRequest, TResponse](includes All) to container builder.</summary>
        public static DiContainer BindRequestHandler<TRequest, TResponse, THandler>(this DiContainer builder, MessagePipeOptions options)
            where THandler : IRequestHandlerCore<TRequest, TResponse>
        {
            var services = new DiContainerProxy(builder);

            services.Add<IRequestHandlerCore<TRequest, TResponse>, THandler>();
            if (!builder.HasBinding<IRequestHandler<TRequest, TResponse>>())
            {
                services.Add<IRequestHandler<TRequest, TResponse>, RequestHandler<TRequest, TResponse>>();
                services.Add<IRequestAllHandler<TRequest, TResponse>, RequestAllHandler<TRequest, TResponse>>();
            }
            return builder;
        }

        /// <summary>Register IAsyncRequestHandler[TRequest, TResponse](includes All) to container builder.</summary>
        public static DiContainer BindAsyncRequestHandler<TRequest, TResponse, THandler>(this DiContainer builder, MessagePipeOptions options)
            where THandler : IAsyncRequestHandlerCore<TRequest, TResponse>
        {
            var services = new DiContainerProxy(builder);

            services.Add<IAsyncRequestHandlerCore<TRequest, TResponse>, THandler>();
            if (!builder.HasBinding<IAsyncRequestHandler<TRequest, TResponse>>())
            {
                services.Add<IAsyncRequestHandler<TRequest, TResponse>, AsyncRequestHandler<TRequest, TResponse>>();
                services.Add<IAsyncRequestAllHandler<TRequest, TResponse>, AsyncRequestAllHandler<TRequest, TResponse>>();
            }
            return builder;
        }

        public static DiContainer BindMessageHandlerFilter<T>(this DiContainer builder)
            where T : class, IMessageHandlerFilter
        {
            if (!builder.HasBinding<T>())
            {
                builder.Bind<T>().AsTransient();
            }
            return builder;
        }

        public static DiContainer BindAsyncMessageHandlerFilter<T>(this DiContainer builder)
            where T : class, IAsyncMessageHandlerFilter
        {
            if (!builder.HasBinding<T>())
            {
                builder.Bind<T>().AsTransient();
            }
            return builder;
        }

        public static DiContainer BindRequestHandlerFilter<T>(this DiContainer builder)
            where T : class, IRequestHandlerFilter
        {
            if (!builder.HasBinding<T>())
            {
                builder.Bind<T>().AsTransient();
            }
            return builder;
        }

        public static DiContainer BindAsyncRequestHandlerFilter<T>(this DiContainer builder)
            where T : class, IAsyncRequestHandlerFilter
        {
            if (!builder.HasBinding<T>())
            {
                builder.Bind<T>().AsTransient();
            }
            return builder;
        }
    }
}