using PleasantPOC.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PleasantPOC
{
    public static partial class Utils
    {
        public const string TemplateFilePathKey = "TemplateFile";
        public const string FilePathKey = "FilePlacement";
        [GeneratedRegex("^#.*#$")]
        public static partial Regex IsValidKeyRegex();
        public static FormUrlEncodedContent FormUrlEncodedContent(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            List<KeyValuePair<string, string>> values = new();
            foreach (PropertyInfo property in properties)
            {
                FormDataAttribute? attribute = property
                    .GetCustomAttribute<FormDataAttribute>();

                if (attribute is null)
                    continue;

                values.Add(new(attribute.Name, property.GetValue(obj) as string ?? string.Empty));
            }

            return new FormUrlEncodedContent(values);
        }
        public static string GetPasswordFromConsole()
        {
            var pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            Console.WriteLine();

            return pass;
        }
    }

}
