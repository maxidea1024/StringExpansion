using System;

namespace StringExpansion
{
    class Program
    {
        public class DummyVarProvider : IVarProvider
        {
            public string GetVar(string name)
            {
                switch (name)
                {
                    case "var1":
                        return "weight";
                    case "var2":
                        return "age";
                    case "val1":
                        return "1000";
                    default:
                        return null;
                }
            }
        }

        static private void Test(string title, string source, StringExpansionOptions options)
        {
            string result = StringExpander.Expand(source, options);

            Console.WriteLine();
            Console.WriteLine($"[{title}]");
            Console.WriteLine($"   source: \"{source}\"");
            Console.WriteLine($"   result: \"{result}\"");
        }

        static void Main(string[] args)
        {
            // %(...) 스타일
            {
                var options = new StringExpansionOptions
                {
                    VarProvider = new DummyVarProvider()
                };

                // 빈문자열 대입.
                Test("Empty", "", options);

                // var1, val1 값을 가져와 치환.
                Test("Simple", "%var1=%val1", options);

                // var4가 없으면 var3을 사용하고 그것도 없으면 var2를 사용.
                Test("Alternatives", "%(var4:%(var3:%var2))", options);
            }

            // ${...} 스타일
            {
                var options = new StringExpansionOptions
                {
                    VarProvider = new DummyVarProvider(),
                    VarDelimiterChar = '$',
                    BeginBraceChar = '{',
                    EndBraceChar = '}'
                };

                // 빈문자열 대입.
                Test("Empty", "", options);

                // var1, val1 값을 가져와 치환.
                Test("Simple", "$var1=$val1", options);

                // var4가 없으면 var3을 사용하고 그것도 없으면 var2를 사용.
                Test("Alternatives", "${var4:${var3:$var2}}", options);
            }
        }
    }
}
