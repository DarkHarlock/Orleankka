using Castle.DynamicProxy;
using Example.Proxy.ServerContracts;
using Orleankka;
using Orleankka.Client;
using Orleankka.Core;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.Proxy.Client
{

    //some stuff for Castle.DynamicProxy integration
    internal class MethodInterceptor : IInterceptor
    {
        private readonly ActorRef _actor;
        public MethodInterceptor(ActorRef actor)
        {
            _actor = actor;
        }

        private abstract class ActorRefAskWrapper
        {
            public abstract void Call(IInvocation invocation, ActorRef actor);
        }
        private class ActorRefAskWrapper<T> : ActorRefAskWrapper
        {
            public override void Call(IInvocation invocation, ActorRef actor)
            {
                var message = invocation.Arguments[0];
                var ret = actor.Ask<T>(message);
                invocation.ReturnValue = ret;
            }
        }

        Dictionary<Type, ActorRefAskWrapper> cache = new Dictionary<Type, ActorRefAskWrapper>();
        private ActorRefAskWrapper GetFromCache(Type t)
        {
            if (!cache.ContainsKey(t))
                cache.Add(t, (ActorRefAskWrapper)Activator.CreateInstance(typeof(ActorRefAskWrapper<>).MakeGenericType(t)));
            return cache[t];
        }
        public void Intercept(IInvocation invocation)
        {
            var retType = invocation.Method.ReturnType;
            var ret = retType.GetGenericArguments()[0];

            var item = GetFromCache(ret);
            item.Call(invocation, _actor);
        }
    }

    //IActorSystem extension helper
    public static class ProxyActorSystemExtensions
    {
        private static readonly ProxyGenerator generator = new ProxyGenerator();
        /// <summary>
        /// Return a proxed server interface implementation
        /// </summary>
        /// <typeparam name="T">Server interface to be proxed</typeparam>
        /// <param name="system">Actor system</param>
        /// <param name="id">Actor id</param>
        /// <returns></returns>
        public static T ActorAs<T>(this IActorSystem system, string id) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new InvalidOperationException("Actor interface is not an interface");

            var path = ActorPath.From(typeof(T), id);
            if (path == ActorPath.Empty)
                throw new ArgumentException("Actor path is empty", "path");
            var actorRef = ActorRef.Deserialize(path);
            return generator.CreateInterfaceProxyWithoutTarget<T>(new MethodInterceptor(actorRef));
        }
    }

    class Program
    {
        public static void Main()
        {
            var t = typeof(IServerInterface); //Force load assembly
            Console.WriteLine("Running example. Booting cluster might take some time ...\n");

            var assembly = Assembly.GetExecutingAssembly();
            var config = new ClientConfiguration().LoadFromEmbeddedResource<Program>("Client.xml");

            var system = ActorSystem
                .Configure()
                .Client()
                .From(config)
                .Register(AppDomain.CurrentDomain.GetAssemblies())
                .Done();

            Run(system).Wait();

            Console.WriteLine("\nPress any key to terminate ...");
            Console.ReadKey(true);

            system.Dispose();
            Environment.Exit(0);
        }

        static async Task Run(IActorSystem system)
        {
            var a = system.ActorAs<IServerInterface>("A-123"); //get the actor proxy
            var b = system.ActorAs<INonUniformServerInterface>("B-456");

            var ret = await a.Handle(new Cmd1()); //Intellisense!!
            await a.Handle(new Cmd2());
            await a.Handle(new Cmd3());

            var ret2 = await b.DoCmd4(new Cmd4());
            await b.DoCmd5(new Cmd5());
            await b.DoCmd6(new Cmd6());
        }
    }
}
