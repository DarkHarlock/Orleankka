using Example.Proxy.ServerContracts;
using Orleankka;
using Orleankka.Cluster;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.Proxy.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = typeof(IServerInterface); //Force load assembly
            Console.WriteLine("Running example. Booting cluster might take some time ...\n");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var config = new ClusterConfiguration().LoadFromEmbeddedResource<Program>("Server.xml");

            var system = ActorSystem
                .Configure()
                .Cluster()
                .Register(assemblies)
                .From(config)
                .Done();

            Console.WriteLine("\nPress any key to terminate ...");
            Console.ReadKey(true);

            system.Dispose();
            Environment.Exit(0);
        }
    }
}
