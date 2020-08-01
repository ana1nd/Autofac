using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    class Program
    {
        static void Main(string[] args)
        {
            TodayWriter todayWriter = new TodayWriter(new ConsoleOutput());
            todayWriter.WriteDate();
        }
    }
}
