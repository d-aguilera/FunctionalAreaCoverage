using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace FunctionalAreaCoverage.Helpers
{
    [DebuggerStepThrough]
    internal static class TraceHelper
    {
        private static TraceSource Default { get; } =
            new TraceSource("default");

        private static BooleanSwitch ArgumentsStringShowValuesSwitch { get; } =
            new BooleanSwitch("ArgumentsStringShowValues", null);

        private static BooleanSwitch ArgumentsEnumerableShowValuesSwitch { get; } =
            new BooleanSwitch("ArgumentsEnumerableShowValues", null);

        public static void WriteLine(string message)
        {
            Default.TraceEvent(TraceEventType.Verbose, 0, message);
        }

        public static void Info(string message = "")
        {
            Default.TraceEvent(TraceEventType.Information, 0, message);
        }

        public static void Run(Expression<Action> expr)
        {
            Start(expr, out var name);
            try
            {
                expr.Compile()();
            }
            finally
            {
                End(name);
            }
        }

        public static T Run<T>(Expression<Func<T>> expr)
        {
            Start(expr, out var name);
            try
            {
                return expr.Compile()();
            }
            finally
            {
                End(name);
            }
        }

        private static void Start<T>(Expression<T> expr, out string name)
        {
            if (expr.NodeType != ExpressionType.Lambda)
            {
                throw new NotSupportedException($"Expression of type '{expr.NodeType}' not supported.");
            }
            var mce = (MethodCallExpression) expr.Body;

            name = mce.Method.Name;
            if (name.EndsWith("Impl"))
            {
                name = name.Substring(0, name.Length - "Impl".Length);
            }

            WriteLine($"Start: {name}");
            foreach (var arg in mce.Arguments)
            {
                var fe = (MemberExpression) arg;
                WriteLine($"       Arg: {fe.Type.Name} {fe.Member.Name} = {GetValue(fe)}");
            }
            Trace.IndentLevel++;
        }

        private static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            var value = getter();

            switch (value)
            {
                case string s:
                    return FormatString(s);
                case IEnumerable<string> e:
                    return FormatEnumerable(e);
                default:
                    return value;
            }
        }

        private static string FormatString(string value)
        {
            var count = $"({value.Length} chars)";

            if (!ArgumentsStringShowValuesSwitch.Enabled)
            {
                return count;
            }

            if (value.Length > 512)
            {
                return value.Substring(0, 512) + $"... {count}";
            }

            return value;
        }

        private static string FormatEnumerable<T>(IEnumerable<T> value)
        {
            if (value == null)
            {
                return "null";
            }

            var enumerable = value as T[] ?? value.ToArray();
            var type = value.ToString();
            var count = $"({enumerable.Length} items)";

            if (!ArgumentsEnumerableShowValuesSwitch.Enabled)
            {
                return $"{type} {count}";
            }

            var values = enumerable.Select(v => $"{{ {v} }}");
            return $"{type} {count} {{ {string.Join(", ", values)} }}";
        }

        private static void End(string name)
        {
            Trace.IndentLevel--;
            WriteLine($"End: {name}");
        }
    }

    // ReSharper disable once UnusedMember.Global
    public class ColorConsoleTraceListener : ConsoleTraceListener
    {
        private Dictionary<TraceEventType, ConsoleColor> EventColor { get; } = new Dictionary<TraceEventType, ConsoleColor>();

        public ColorConsoleTraceListener()
        {
            EventColor.Add(TraceEventType.Verbose, ConsoleColor.DarkGray);
            EventColor.Add(TraceEventType.Information, ConsoleColor.Gray);
            EventColor.Add(TraceEventType.Warning, ConsoleColor.Yellow);
            EventColor.Add(TraceEventType.Error, ConsoleColor.DarkRed);
            EventColor.Add(TraceEventType.Critical, ConsoleColor.Red);
            EventColor.Add(TraceEventType.Start, ConsoleColor.DarkCyan);
            EventColor.Add(TraceEventType.Stop, ConsoleColor.DarkCyan);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEvent(eventCache, source, eventType, id, "{0}", message);
        }
 
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = GetEventColor(eventType, originalColor);
            try
            {
                // ReSharper disable once InvertIf
                if (Filter == null || Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                {
                    var message = args.Length > 0
                        ? string.Format(CultureInfo.InvariantCulture, format, args)
                        : format
                        ;
                    WriteLine(message);
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
 
        private ConsoleColor GetEventColor(TraceEventType eventType, ConsoleColor defaultColor)
        {
            return !EventColor.ContainsKey(eventType) ? defaultColor : EventColor[eventType];
        }
    }
}
