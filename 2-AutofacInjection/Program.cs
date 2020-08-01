using System;
using Autofac;
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
            var builder = new ContainerBuilder();
            builder.RegisterType<ConsoleOutput>().As<IOutput>();
            builder.RegisterType<TodayWriter>().As<IDateWriter>();
            Container = builder.Build();

            WriteDate();
        }

        public static void WriteDate()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                // this resolves the dependencies and return TodayWriter object
                var writer = scope.Resolve<IDateWriter>();
                writer.WriteDate();

                // to add more, add relevant methods in the Interface
                writer.WriteSpecificDate(DateTime.Now);
            }
        }

    }
}
