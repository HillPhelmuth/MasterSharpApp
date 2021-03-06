﻿using System.Collections.Generic;
using System.Dynamic;

namespace MasterSharpOpen.Shared.CodeModels
{
    public static class CodeSnippets
    {
        public static readonly Dictionary<string, string> CollectionsSnippets = new Dictionary<string, string>
        {
            {"ArrayList", ARRAYLIST}, {"Stack", STACK}, {"Queue", QUEUE},
            {"HashTable", HASHTABLE},{"List", LIST},{"Dictionary", DICTIONARY}
        };
        public static readonly Dictionary<string, string> StringsSnippets = new Dictionary<string, string>
        {
            {"Concatenation", CONCATENATION},{"String.Format", FORMAT},{"Interpolation", INTERPOLATION},{"SubString", SUBSTRING},{"Array to string", ARRAYTOSTRING},{"String to array", STRINGTOARRAY}
        };

        public static readonly Dictionary<string, string> ConditionalSnippets = new Dictionary<string, string>
        {
            {"If Statement", IFCONDITIONAL}, {"If-Else Statement", IFELSE}, {"If-Else if-Else Statement", ELSEIF},
            {"Switch Statement", SWITCH}, {"For Loop", FORLOOP}, {"Foreach Loop", FOREACHLOOP}
        };

        public static readonly Dictionary<string, string> ConsoleSnippets = new Dictionary<string, string>
        {
            {"Console App", HelloWorld},{"Console with user input", ConsoleInput},{"Console multiple Writelines",ConsoleMutlitpeWrites}
        };

        public static readonly Dictionary<string, string> ExtensionSnippets = new Dictionary<string, string>
        {
            {"Extend String To Word Count", StringToWordCount}, {"Extend Double to Description", DoubleToText},
            {"Extend String to Index list", StringToCharIndexList}, {"Extend Generic and Shuffle", ShuffleGeneric}

        };
        private const string ARRAYLIST =
            "ArrayList al = new ArrayList();\nal.Add(1);\nal.Add(\"Example\");\nal.Add(true);\nreturn al;";

        private const string STACK =
            "Stack st = new Stack();\nst.Push(1);\nst.Push(2);\nst.Push(3);\nreturn st;";

        private const string QUEUE = "Queue qt = new Queue();\nqt.Enqueue(1);\nqt.Enqueue(2);\nqt.Enqueue(3);\nreturn qt;";

        private const string HASHTABLE =
            "Hashtable ht = new Hashtable();\nht.Add(\"001\",\".Net\");\nht.Add(\"002\",\"C#\");\nht.Add(\"003\",\"ASP.Net\");\nreturn ht;";

        private const string LIST = "List<string> list = new List<string>();\nlist.Add(\"item 1\");\nlist.Add(\"item 2\");\nlist.Add(\"item 3\");\nreturn list;";
        private const string DICTIONARY = "Dictionary<int, string> dict = new Dictionary<int, string>();\ndict.Add(1, \"item 1\");\ndict.Add(2, \"item 2\");\ndict.Add(3, \"item 3\");\nreturn dict;";
        private const string CONCATENATION =
            "string nowDateTime = \"Date: \" + DateTime.Now.ToString(\"D\");\n" +
            "string firstName = \"Gob\";\nstring lastName = \"Bluth\";\nstring age = \"33\";\n" + 
            "string authorDetails = firstName + \" \" + lastName + \" is \" + age + \" years old.\";\nreturn authorDetails;";
        private const string FORMAT = "string name = \"George Bluth\";\nint age = 33;\n" +
                                      "string authorInfo = string.Format(\"{0} is {1} years old.\", name, age.ToString());\nreturn authorInfo;";
        private const string INTERPOLATION = "string name = \"George Bluth\";\nint age = 33;\n" +
                                             "string authorInfo = string.Format($\"{name} is {age} years old.\");\nreturn authorInfo;";
        private const string SUBSTRING = "string authorInfo = \"Buster Bluth is 33 years old.\";\n" +
                                         "int startPosition = authorInfo.IndexOf(\"is \") + 1;\n" +
                                         "string age = authorInfo.Substring(startPosition +2, 2 );\nreturn age;";

        private const string ARRAYTOSTRING =
            "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nreturn name;";

        private const string STRINGTOARRAY = "string sentence = \"Mahesh Chand is an author and founder of C# Corner\";\n" +
                                             "char[] charArr = sentence.ToCharArray();\nreturn charArr;";

        private const string IFCONDITIONAL = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nif (name.Length > 5)\n{\n\tname = $\"{name} is sometimes easy\";\n}\nreturn name;";
        private const string IFELSE = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nif (name.Length > 6)\n{\n\tname = $\"{name} is sometimes easy\";\n}\nelse\n{\n\tname = $\"{name} is sometimes hard\";\n}\nreturn name;\n";
        private const string ELSEIF = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nint nameLength = name.Length;\nif (nameLength > 7)\n{\n\tname = $\"{name} is sometimes easy\";\n}\nelse if (nameLength > 6)\n{\n\tname = $\"{name} is sometimes hard\";\n}\nelse{\n\tname = $\"{name} is always c#\";\n}\nreturn name;";
        private const string SWITCH = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nint nameLength = name.Length;\nswitch (nameLength)\n{\n\tcase 7:\n\t\tname = $\"{name} is sometimes easy\";\n\t\tbreak;\n\tcase 8:\n\t\tname = $\"{name} is sometimes hard\";\n\t\tbreak;\n\tdefault:\n\t\tname = $\"{name} is always c#\";\n\t\tbreak;\n}\nreturn name;";
        private const string FORLOOP = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = new string(chars);\nfor (int i = 0; i < chars.Length; i++)\n{\n\tname = name + \"!\";\n}\nreturn name;";
        private const string FOREACHLOOP = "char[] chars = { 'C', 'S', 'h', 'a', 'r', 'p' };\nstring name = \"\";\nforeach (var str in chars)\n{\n\tname += str;\n}\nreturn name;\n";
        private const string HelloWorld = "using System;\n\nclass Program\n{\n\tpublic static void Main()\n\t{\n\t\tConsole.WriteLine(\"Hello World\");\n\t}\n}";

        private const string ConsoleInput = "using System;\n\nclass Program\n{\n\tpublic static void Main()\n\t{\n\t\tstring input = Console.ReadLine();\n\t\tstring additional = \" was your input\";\n\t\tConsole.WriteLine(input);\n\t\tstring newOutput = input + additional;\n\t\tConsole.WriteLine(newOutput);\n\t}\n}";

        private const string ConsoleMutlitpeWrites =
            "using System;\n\nclass Program\n{\n\tpublic static void Main()\n\t{\n\t\tstring input = \"Foo\";\n\t\tstring additional = \" was not your input\";\n\t\tConsole.WriteLine(input);\n\t\tstring newOutput = input + additional;\n\t\tConsole.WriteLine(newOutput);\n\t}\n}";

        private const string ShuffleGeneric = "public static void Shuffle<T>(this IList<T> cards)\n{\n\tvar rng = new Random();\n\tint n = cards.Count;\n\twhile (n > 1)\n\t{\n\t\tn--;\n\t\tint k = rng.Next(n + 1);\n\t\tT value = cards[k];\n\t\tcards[k] = cards[n];\n\t\tcards[n] = value;\n\t}\n}\n\nvar list = new List<int> {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15};\nlist.Shuffle();\nreturn list;";
        private const string DoubleToText = "public static string ExtDoubleToText(this double inp)\n{\n\tif (inp > .8)\n\t\treturn \"very likely\";\n\tif (inp > .5 && inp <= .8)\n\t\treturn \"somewhat likely\";\n\tif (inp > .25 && inp <= .5)\n\t\treturn \"somewhat unlikely\";\n\treturn \"very unlikely\";\n}\nvar myodds = .7;\nreturn myodds.ExtDoubleToText();";
        private const string StringToWordCount = "public static int WordCount(this string words)\n{\n\tvar stringToArray = words.Split(' ');\n\tvar wordCount = stringToArray.Length;\n\treturn wordCount;\n}\nreturn \"How many words are here?\".WordCount();";

        private const string StringToCharIndexList =
            "public static List<int> AllIndexesOf(this string str, string value)\n{\n\tif (string.IsNullOrEmpty(value))\n\t\treturn new List<int>();\n\tvar indexes = new List<int>();\n\tfor (int index = 0; ; index += value.Length)\n\t{\n\t\tindex = str.IndexOf(value, index, StringComparison.Ordinal);\n\t\tif (index == -1)\n\t\t\treturn indexes;\n\t\tindexes.Add(index);\n\t}\n}\nreturn \"i like it\".AllIndexesOf(\"i\");";
    }
}
