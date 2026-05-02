using Resend;
using System;
using System.Reflection;

namespace Inspector
{
    public class Program
    {
        public static void Main()
        {
            var type = typeof(EmailMessage);
            Console.WriteLine($"Properties for {type.Name}:");
            foreach (var prop in type.GetProperties())
            {
                Console.WriteLine($"- {prop.Name} ({prop.PropertyType.Name})");
            }
        }
    }
}
