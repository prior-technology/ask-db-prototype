using System;
using System.Text.Json;
namespace AskDbConsole
{
    public class QuickTest
    {
        class Bob { public string Abc { get; set; } public string DoReMi { get; set; } }

        public void DoTest()
        {
            var bobs = new Bob[]
            {
                new Bob {Abc = "20", DoReMi = "abc\ndef"},
                new Bob {Abc = "21", DoReMi = "400"},
                new Bob {Abc = "22", DoReMi = "500"},
            };
            var opts = new JsonSerializerOptions();

            var s = JsonSerializer.Serialize(bobs,opts );
            Console.WriteLine(s);
            Console.WriteLine("end");
            Console.ReadLine();

        }
    }
}
