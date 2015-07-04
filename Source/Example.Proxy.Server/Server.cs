using Example.Proxy.ServerContracts;
using Orleankka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Proxy.Server
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ProxyActorTypeCodeAttribute : Attribute, ITypeCodeAttribute
    {
        public ProxyActorTypeCodeAttribute(Type actorProxyInterface)
        {
            _code = actorProxyInterface.FullName;
        }

        private readonly string _code;
        public string Code
        {
            get { return _code; }
        }
    }

    [ProxyActorTypeCode(typeof(IServerInterface))]
    public class UniformServer : Actor, IServerInterface
    {
        public Task<string> Handle(Cmd1 message)
        {
            return Task.FromResult("Cmd1");
        }

        public Task<string> Handle(Cmd2 message)
        {
            return Task.FromResult("Cmd2");
        }

        public Task<string> Handle(Cmd3 message)
        {
            return Task.FromResult("Cmd3");
        }
    }


    [ActorTypeCode("Example.Proxy.ServerContracts.INonUniformServerInterface")]
    public class NonUniformServer : Actor, INonUniformServerInterface
    {
        public Task<string> DoCmd4(Cmd4 cmd)
        {
            return Task.FromResult("Cmd4");
        }

        public Task<string> DoCmd5(Cmd5 cmd)
        {
            return Task.FromResult("Cmd5");
        }

        public Task<string> DoCmd6(Cmd6 cmd)
        {
            return Task.FromResult("Cmd6");
        }
    }
}
