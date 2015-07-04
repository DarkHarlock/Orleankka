using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Orleankka.Core
{
    static class ActorAssembly
    {
        public static void Reset()
        {
            ActorTypeCode.Reset();
            ActorPrototype.Reset();
            ActorEndpointDynamicFactory.Reset();
        }

        public static void Register(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                Register(assembly);
        }

        static void Register(Assembly assembly)
        {
            var actors = assembly
                .GetTypes()
                .Where(x => 
                    (!x.IsAbstract && typeof(Actor).IsAssignableFrom(x)) ||
                    (typeof(IActorProxyInterface).Namespace != x.Namespace && x.IsInterface && typeof(IActorProxyInterface).IsAssignableFrom(x))
                );

            foreach (var type in actors)
            {
                ActorTypeCode.Register(type);
                if (!type.IsInterface)
                {
                    ActorPrototype.Register(type);
                }
                ActorEndpointDynamicFactory.Register(type);
            }
        }
    }
}
