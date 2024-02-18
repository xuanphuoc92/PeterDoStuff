using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeterDoStuff.Database
{
    public class Command
    {
        public string Expression { get; private set; } = "";
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();

        private Command() { }
        
        public static Command New() => new Command();

        public static Command New(string expression, params object[] parameters)
        {
            var command = New();
            command.AppendLine(expression, parameters);
            return command; 
        }

        public Command AppendLine(string expression, object[] parameters)
        {
            Expression += RecordAndFormat(expression, parameters);
            return this;
        }

        private string RecordAndFormat(string expression, object[] parameters)
        {
            var keys = RecordParameters(parameters);
            return string.Format(expression, keys);
        }

        private string[] RecordParameters(object[] parameters)
        {
            //if (parameters.Any() == false)
            //    return new string[0];

            List<string> keys = new List<string>();
            foreach (object parameter in parameters)
            {
                //// For SQL Server, the null parameter does not work.
                //// Therefore, it is better to hard-code null parameter value into the SQL script.
                //if (parameter == null)
                //{
                //    keys.Add("null");
                //    continue;
                //}

                var key = "@p" + Parameters.Count;
                Parameters.Add(key, parameter);
                keys.Add(key);
            }

            return keys.ToArray();
        }
    }
}
