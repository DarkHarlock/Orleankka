using Castle.DynamicProxy;
using Orleankka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Proxy.ServerContracts
{
    [Serializable]
    public class Cmd1 { }
    [Serializable]
    public class Cmd2 { }
    [Serializable]
    public class Cmd3 { }

    [Serializable]
    public class Cmd4 { }
    [Serializable]
    public class Cmd5 { }
    [Serializable]
    public class Cmd6 { }

    public interface INonUniformServerInterface :
        INonUniformActorProxyInterface
    {
        Task<string> DoCmd4(Cmd4 cmd);
        Task<string> DoCmd5(Cmd5 cmd);
        Task<string> DoCmd6(Cmd6 cmd);
    }

    public interface IHandler<T, K>
    {
        Task<K> Handle(T message);
    }

    public interface IServerInterface :
        IActorProxyInterface,
        IHandler<Cmd1, string>,
        IHandler<Cmd2, string>,
        IHandler<Cmd3, string>
    {

    }
}
