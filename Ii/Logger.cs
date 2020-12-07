using System;
using System.IO;

namespace Ii
{

    public class Logger
    {

        public enum Targets
        {
            None,
            Console,
            File,
        }

        private string fileName;

        private Action<string> SetFileName;
        private Func<string> GetFileName;
        private ILogger logger;
        public Logger(Targets target = Targets.Console, string fileName = "Log.txt")
        {
            Target = target;
            FileName = fileName;
        }

        public void Log(Exception exception)
        {
            if (Target != Targets.None)
            {
                logger.Log($"{DateTime.Now}, Exception: {exception}");
            }
        }

        public void Log(string info)
        {
            if (Target != Targets.None)
            {
                logger.Log($"{DateTime.Now}, Info: {info}");
            }

        }


        public string FileName
        {
            get => GetFileName();
            set => SetFileName(value);
        }
        Targets target;


        public Targets Target
        {
            get => target;
            set
            {

                if ((value == Targets.File) && !(logger is FileLogger))
                {
                    logger = new FileLogger(FileName);


                    SetFileName = newFileName => (logger as FileLogger).FileName = newFileName;

                    GetFileName = () => (logger as FileLogger).FileName;


                    target = Targets.File;

                    return;
                }


                if ((value == Targets.Console) && !(logger is ConsoleLogger))
                {

                    logger = new ConsoleLogger();


                    SetFileName = newFileName => fileName = newFileName;

                    GetFileName = () => fileName;


                    target = Targets.Console;

                    return;
                }

                target = value;

            }
        }
        interface ILogger
        {

            public void Log(string msg);
        }

        class ConsoleLogger : ILogger
        {
            public void Log(string message) => Console.WriteLine(message + Environment.NewLine);

        };
        class FileLogger : ILogger
        {
            public FileLogger(string fileName) => FileName = fileName;
            public string FileName { get; set; }
            public void Log(string message)
            {
                using (var sw = File.AppendText(FileName))
                {
                    sw.WriteLine(message + Environment.NewLine);
                }
            }

        };
    }


}