using System;
using Autofac;
using Autofac.Core;

namespace AutofacTutorial1
{
    /// <summary>
    /// interface to write output to the console
    /// </summary>
    public interface IOutput
    {
        void Write(string content);
    }

    /// <summary>
    /// contains implementation of IOutput
    /// </summary>
    public class ConsoleOutput : IOutput
    {
        public void Write(string content)
        {
            Console.WriteLine(content);
        }
    }

    /// <summary>
    /// writes date
    /// </summary>
    public interface IDateWriter
    {
        void WriteDate();

        void WriteSpecificDate(DateTime dateTime);
    }

    public class TodayWriter : IDateWriter
    {
        IOutput output;
        public DateTime dateTime;

        public TodayWriter()
        {

        }

        public TodayWriter(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public TodayWriter(IOutput output)
        {
            this.output = output;
        }
        public void WriteDate()
        {
            this.output.Write(DateTime.Today.ToString());
        }

        /// <summary>
        /// write specific date
        /// </summary>
        /// <param name="dateTime"></param>
        public void WriteSpecificDate(DateTime dateTime)
        {
            this.output.Write(dateTime.ToString());
        }
    }

    class Program
    {
        private static IContainer Container { get; set; }
        public static void Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Enter experiment key\n");
                Console.WriteLine(
                    "1. Reflection Register\n2. Instance Register\n3. Lambda Register\n" +
                    "4. Other Info\n5. AsSelf Register\n6. Pass parameters during registration\n7. Exit\n");
                int input = Convert.ToInt32(Console.ReadLine());
                ContainerBuilder builder;
                switch (input)
                {
                    case 1:
                        builder = new ContainerBuilder();
                        Console.WriteLine("Registering components via reflection on type");
                        builder.RegisterType<ConsoleOutput>().As<IOutput>();
                        builder.RegisterType<TodayWriter>().As<IDateWriter>();
                        Container = builder.Build();
                        WriteDate();
                        break;
                    case 2:
                        builder = new ContainerBuilder();
                        Console.WriteLine("Registering components via instances");
                        var consoleOutputObject = new ConsoleOutput();
                        var todayWriterObject = new TodayWriter(consoleOutputObject);
                        builder.RegisterInstance(consoleOutputObject).As<IOutput>();
                        builder.RegisterInstance(todayWriterObject).As<IDateWriter>();
                        Container = builder.Build();
                        WriteDate();
                        break;
                    case 3:
                        builder = new ContainerBuilder();
                        Console.WriteLine("Registering components via lambda expressions");
                        builder.Register(c => new ConsoleOutput()).As<IOutput>();
                        builder.Register(c => new TodayWriter(c.Resolve<IOutput>())).As<IDateWriter>();
                        Container = builder.Build();
                        WriteDate();
                        break;
                    case 4:
                        Console.WriteLine("Other relevant info");
                        Console.WriteLine("1. only concrete type components can be registered via reflection type components. Since behind the scene Autofac is creating instance which cant only be created for concrete type and not abstract type/classes");
                        Console.WriteLine("2. to expose components we have to tell Autofac which services that component exposes.");
                        Console.WriteLine();
                        break;
                    case 5:
                        builder = new ContainerBuilder();
                        Console.WriteLine("Exposing component as a service as well along-with other services");
                        builder.RegisterType<TodayWriter>().As<IDateWriter>().AsSelf();
                        builder.RegisterType<ConsoleOutput>().As<IOutput>();
                        Container = builder.Build();
                        WriteDateAsSelf();
                        break;
                    case 6:
                        builder = new ContainerBuilder();
                        builder.Register(c => new ConsoleOutput()).As<IOutput>();
                        Console.WriteLine("Enter type of parameter/lambda to use");
                        int type = Convert.ToInt16(Console.ReadLine());
                        if (type == 1)
                        {
                            // register as lambda exp (pass)
                            builder.Register(c => new TodayWriter(DateTime.Now)).As<IDateWriter>();

                            // 2nd way of registering via lambda
                            builder.Register(c => new TodayWriter(c.Resolve<IOutput>())).As<IDateWriter>();
                        }
                        else if (type == 2)
                        {
                            // 2nd-way: use named parameter
                            builder.RegisterType<TodayWriter>()
                                .As<IDateWriter>()
                                .WithParameter(new NamedParameter("current date", DateTime.Now));
                        }
                        else if (type == 3)
                        {
                            // use typed parameter
                            builder.RegisterType<TodayWriter>()
                                .As<IDateWriter>()
                                .WithParameter(new TypedParameter(typeof(int), 32));
                        }
                        else
                        {
                            // use resolved parameter
                            builder.RegisterType<TodayWriter>()
                                .As<IDateWriter>()
                                .WithParameters(new Parameter[]
                                {
                                    new ResolvedParameter(
                                        (pi, ctx) => pi.ParameterType == typeof(int),
                                        (pi, ctx) => ctx.Resolve<IOutput>())
                                });
                        }

                        Container = builder.Build();
                        WriteDate();
                        break;
                    default:
                        return;
                }
            }
        }

        public static void WriteDate()
        {
            try
            {
                using (var scope = Container.BeginLifetimeScope())
                {
                    // this resolves the dependencies and return TodayWriter object
                    var writer = scope.Resolve<IDateWriter>();
                    writer.WriteDate();

                    // to add more, add relevant methods in the Interface
                    writer.WriteSpecificDate(DateTime.Now);
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception thrown: {e.Message}");
            }
        }

        public static void WriteDateAsSelf()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                Console.WriteLine($"Is IDateWriter registered: {scope.IsRegistered<IDateWriter>()}");
                Console.WriteLine($"Is IDateWriter registered: {scope.IsRegistered<TodayWriter>()}");
                
                // this resolves the dependencies and return TodayWriter object
                var writer = scope.Resolve<IDateWriter>();
                writer.WriteDate();
                writer.WriteSpecificDate(DateTime.Now);
                Console.WriteLine();

                // resolve the service as todaywriter since we register as both component and service
                writer = scope.Resolve<TodayWriter>();
                writer.WriteDate();

                // to add more, add relevant methods in the Interface
                writer.WriteSpecificDate(DateTime.Now);
                Console.WriteLine();
            }
        }
    }
}