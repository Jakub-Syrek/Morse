using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Morse_code_crypter
{
    public static class DecrypterExtensions
    {
        const string spaceBetweenWords = "000000";
        const string spaceBetweenChars = "000";
        public static Span<bool> ConvertToBinary(this ReadOnlySpan<char> span)
        {
            Span<bool> res = new Span<bool>();
            foreach (char c in span)
            {
                res.Fill(Convert.ToBoolean(c));
            }
            return res;
        }
        public static Span<bool> ConvertToBinary(this StringBuilder stringBuilder)
        {
            Span<bool> res = new Span<bool>();
            foreach (char c in stringBuilder.ToString())
            {
                res.Fill(Convert.ToBoolean(c));
            }
            return res;
        }
        public static bool? ConvertCharToNullableBool(this char c)
        {
            bool? output = c switch
            {
                '1' => true,
                '0' => false,
                _ => null,
            };
            return output;
        }
        public static Span<bool?> ConvertStringToNullableBoolSpan(string s)
        {
            Span<bool?> span = new Span<bool?>();
            foreach (char c in s)
                span.Fill(c.ConvertCharToNullableBool());
            return span;
        }
        static bool ByteArrayCompare(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }
        public static unsafe void Write<T>(this Stream stream, T value)
        where T : unmanaged
        {
            var pointer = Unsafe.AsPointer(ref value);
            var span = new Span<byte>(pointer, sizeof(T));
            stream.Write(span);
        }
        public static void WriteSafe<T>(this Stream stream, T value)
             where T : unmanaged
        {
            var tSpan = MemoryMarshal.CreateSpan(ref value, 1);
            var span = MemoryMarshal.AsBytes(tSpan);
            stream.Write(span);
        }
        public static unsafe T Read<T>(this Stream stream)
            where T : unmanaged
        {
            var result = default(T);
            var pointer = Unsafe.AsPointer(ref result);
            var span = new Span<byte>(pointer, sizeof(T));
            stream.Read(span);
            return result;
        }
        public static T ReadSafe<T>(this Stream stream)
              where T : unmanaged
        {
            var result = default(T);
            var tSpan = MemoryMarshal.CreateSpan(ref result, 1);
            var span = MemoryMarshal.AsBytes(tSpan);
            stream.Read(span);
            return result;
        }
        public static int GetSamplingRate(this Span<char> span)
        {
            Span<char> sampleS = new Span<char>();
            for (int i = 1; i < span.Length; i++)
            {
                var l = i;

                for (int j = 0; j < l; j++)
                    sampleS.Fill('1');

                for (int j = 0; j < l; j++)
                    sampleS.Fill('0');

                for (int j = 0; j < l; j++)
                    sampleS.Fill('1');

                if (MemoryExtensions.Contains(span, sampleS, StringComparison.Ordinal))
                    return i;
            }
            return 0;
        }
        public static char ConvertMorseToChar(this string s)
        {
            char output = s switch
            {
                "000000" => ' ',
                _ => s switch
                {
                    "0" => 'E',
                    "1" => 'T',
                    "01" => 'A',
                    "11" => 'M',
                    "00" => 'I',
                    "10" => 'N',
                    "001" => 'U',
                    "100" => 'D',
                    "1000" => 'B',
                    "110" => 'G',
                    "101" => 'K',
                    "111" => 'O',
                    "010" => 'R',
                    "000" => 'S',
                    "011" => 'W',
                    "1010" => 'C',
                    "0010" => 'F',
                    "0000" => 'H',
                    "0111" => 'J',
                    "0100" => 'L',
                    "0110" => 'P',
                    "1101" => 'Q',
                    "0001" => 'V',
                    "1001" => 'X',
                    "1011" => 'Y',
                    "1100" => 'Z',
                    _ => ' ',
                }
            };
            return output;
        }
        public static ReadOnlySpan<char?> GetBitSwitch(this ReadOnlySpan<char> input)
        {
            Span<char?> resSpan = new Span<char?>();

            for (int i = 0; i < input.Length - 1; i++)
            {
                ReadOnlySpan<char> slice = input.Slice(i, 2);
                resSpan.Fill(slice.ToString() switch
                {
                    "10" => '0',
                    "11" => '1',
                    "00" => ' ',
                    _ => null,
                });

                i = slice.ToString() switch
                {
                    "10" => i++,
                    "11" => i + 3,
                    "00" => i++,
                    "01" => i++,
                };
            }
            return (ReadOnlySpan<char?>)resSpan;
        }
        public static int GetSamplingRate(this ReadOnlySpan<char> span)
        {
            Span<char> sampleS = new Span<char>();
            for (int i = 1; i < span.Length; i++)
            {
                var l = i;

                for (int j = 0; j < l; j++)
                    sampleS.Fill('1');

                for (int j = 0; j < l; j++)
                    sampleS.Fill('0');

                for (int j = 0; j < l; j++)
                    sampleS.Fill('1');

                if (MemoryExtensions.Contains(span, sampleS, StringComparison.Ordinal))
                    return i;
            }
            return 0;
        }
        public static bool ValidateAsFrase(this ReadOnlySpan<char> input) => MemoryExtensions.Contains(input, spaceBetweenWords, StringComparison.Ordinal);
        public static bool ValidateAsWord(this ReadOnlySpan<char> input) => MemoryExtensions.Contains(input, spaceBetweenChars, StringComparison.Ordinal);
        public static ReadOnlySpan<char> SampleBit(this ReadOnlySpan<char> input, int ratio)
        {
            Span<char> res = new Span<char>();
            for (int i = 0; i < input.Length; i+=ratio)
            {
                res.Fill(input[i]);
            }
            return (ReadOnlySpan<char>)res;
        }
            
            
            
            //=> string.Join("", Enumerable.Range(0, input.Length / ratio).Select(i => input.Slice(i * ratio, ratio)).First());
        //public static bool ValidateAsWord(ReadOnlySpan<char> input) => MemoryExtensions.Contains(input, spaceBetweenChars, StringComparison.Ordinal);
        public static Span<string> SplitByChars(this string input) => input.Split(spaceBetweenChars).AsSpan();
    }
    public struct TestCase
    {
        [JsonProperty("Item1")]
        public string Encrypted { get; set; }

        [JsonProperty("Item2")]
        public string Decrypted { get; set; }
    }
    public static class Helper
    {
        public const string zero = "10";
        public const string one = "11";
        public static List<TestCase> GetCasesDeserialized(string filePath)
        {
            List<TestCase> container = new List<TestCase>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                var obj = JsonConvert.DeserializeObject<TestCase>(line);
                container.Add(obj);
            }
            file.Close();
            return container;
        }
        //public static bool ValidateAsFrase(this ReadOnlySpan<char> input, ReadOnlySpan<char> other) => MemoryExtensions.Contains(input, other, StringComparison.Ordinal);
        //public static bool ValidateAsWord(this ReadOnlySpan<char> input, ReadOnlySpan<char> other) => MemoryExtensions.Contains(input, other, StringComparison.Ordinal);
        public static string SampleBit(this ReadOnlyMemory<char> input, int ratio) => string.Join("", Enumerable.Range(0, input.Length / ratio).Select(i => input.Slice(i * ratio, ratio)).First());
    }
    class Program
    {
        const string spaceBetweenWords = "000000";
        const string spaceBetweenChars = "000";
        const string path = @"c:\tmp\test.txt";

        public static Dictionary<char, String> Morse = new Dictionary<char, String>()
        {
            {'A' , "01"},
            {'B' , "1000"},
            {'C' , "1010"},
            {'D' , "100"},
            {'E' , "0"},
            {'F' , "0010"},
            {'G' , "110"},
            {'H' , "0000"},
            {'I' , "00"},
            {'J' , "0111"},
            {'K' , "101"},
            {'L' , "0100"},
            {'M' , "11"},
            {'N' , "10"},
            {'O' , "111"},
            {'P' , "0110"},
            {'Q' , "1101"},
            {'R' , "010"},
            {'S' , "000"},
            {'T' , "1"},
            {'U' , "001"},
            {'V' , "0001"},
            {'W' , "011"},
            {'X' , "1001"},
            {'Y' , "1011"},
            {'Z' , "1100"},
            {'0' , "11111"},
            {'1' , "01111"},
            {'2' , "00111"},
            {'3' , "00011"},
            {'4' , "00001"},
            {'5' , "00000"},
            {'6' , "10000"},
            {'7' , "11000"},
            {'8' , "11100"},
            {'9' , "11110"},
        };
        
       
        public static string GetBit(string input)
        {
            string tmp = $"{input}0";
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < tmp.Length - 1; i++)
            {
                if (tmp[i] == '1' && tmp[i + 1] == '0')
                {
                    stringBuilder.Append("0");
                    ++i;
                }
                if (tmp[i] == '1' && tmp[i + 1] == '1')
                {
                    stringBuilder.Append("1");
                    i = i + 3;
                }

                if (i < tmp.Length - 1)
                {
                    if (tmp[i] == '0')
                    {
                        if (tmp[i + 1] == '0')
                        {
                            stringBuilder.Append("-");
                            i = (i < tmp.Length - 1) ? i += 1 : i;
                        }
                    }

                }
            }
            var res = stringBuilder.ToString();
            return res;
        }

        //public static string SampleBit(string input, int ratio) => string.Join("", Enumerable.Range(0, input.Length / ratio).Select(i => input.Substring(i * ratio, ratio)[0]));        
        public static Span<string> SplitByWords(string input) => input.Split(spaceBetweenWords).AsSpan();
       
        
        public static char GetCharMorsedDict(string input) => Morse.ContainsValue(input) ? Morse.Where(item => item.Value.Equals(input)).Select(item => item.Key).FirstOrDefault() : ' ';
        
        public static string GetCharMorsedSwitch(string input)
        {
            StringBuilder localTmp = new StringBuilder(input);

            if (String.IsNullOrWhiteSpace(input))
                return " ";

            switch (localTmp[0])
            {
                case '0':
                    if (localTmp.Length > 1)
                    {
                        switch (localTmp[1])
                        {
                            case '0':
                                if (localTmp.Length > 2)
                                {
                                    switch (localTmp[2])
                                    {
                                        case '0':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "H";
                                                    case '1':
                                                        return "V";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "S";
                                        case '1':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "F";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "U";
                                        default:
                                            return " ";

                                    }
                                }
                                else
                                {
                                    return "I";
                                }
                            case '1':
                                if (localTmp.Length > 2)
                                {
                                    switch (localTmp[2])
                                    {
                                        case '0':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "L";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "R";

                                        case '1':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "P";
                                                    case '1':
                                                        return "J";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "W";
                                        default:
                                            return " ";

                                    }

                                }
                                else
                                    return "A";
                            default:
                                return " ";
                        }
                    }
                    else
                        return "E";

                case '1':
                    if (localTmp.Length > 1)
                    {
                        switch (localTmp[1])
                        {
                            case '0':
                                if (localTmp.Length > 2)
                                {
                                    switch (localTmp[2])
                                    {
                                        case '0':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "B";
                                                    case '1':
                                                        return "X";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "D";
                                        case '1':
                                            if (localTmp.Length > 3)
                                            {
                                                switch (localTmp[3])
                                                {
                                                    case '0':
                                                        return "C";
                                                    case '1':
                                                        return "Y";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "K";
                                        default:
                                            return " ";
                                    }
                                }
                                else
                                {
                                    return "N";
                                }
                            case '1':
                                if (localTmp.Length > 2)
                                {
                                    switch ((int)localTmp[2])
                                    {
                                        case '0':
                                            if (localTmp.Length > 3)
                                            {
                                                switch ((int)localTmp[3])
                                                {
                                                    case '0':
                                                        return "Z";
                                                    case '1':
                                                        return "Q";
                                                    default:
                                                        return " ";
                                                }
                                            }
                                            else
                                                return "G";

                                        case '1':
                                            return "O";
                                        default:
                                            return " ";

                                    }

                                }
                                else
                                    return "M";
                            default:
                                return " ";
                        }
                    }
                    else
                        return "T";
                default:
                    return " ";
            }
        }
       
        //public static int GetSamplingRate(string input)
        //{
        //    int shortestLengthReccurent = 1;
        //    int longestLengthReccurent = 0;
        //    List<int> ListReccurences = new List<int>();
        //    int currentLength = 1;

        //    for (int i = 0; i < input.Length - 1; i++)
        //    {
        //        if (input[i] == input[i + 1])
        //        {
        //            currentLength++;
        //            if (i == input.Length - 2)
        //                ListReccurences.Add(currentLength);  
        //        }
        //        else
        //        {
        //            ListReccurences.Add(currentLength);
        //            currentLength = 1;
        //        }
        //    }
        //    if (ListReccurences.Any())
        //    {
        //        shortestLengthReccurent = ListReccurences.Min();
        //        longestLengthReccurent = ListReccurences.Max();                
        //    }
        //    if (longestLengthReccurent != 0)  
        //    {
        //        return shortestLengthReccurent;
        //    }
        //    return 0;
        //}
        public static string ReturnDecodedSwitch(string input)
        {
            StringBuilder result = new StringBuilder();
            string[] wordsEncrypted;
            if (input[0] == '0' || input.Last() == '0')
                input = input.Trim('0');

            int samplingRate = GetSamplingRate(input); 
            input = (samplingRate != 0) ? Helper.SampleBit(input.AsMemory(), samplingRate) : input;
            wordsEncrypted = Helper.ValidateAsFrase(input.AsSpan(),spaceBetweenWords.AsSpan()) ? SplitByWords(input) : new string[] { input };
            wordsEncrypted.ToList().ForEach(x => SplitByChars(x).ToList().ForEach(y => result.Append(GetCharMorsedSwitch(GetBit(y)))));
            return result.ToString();
        }
        public static string ReturnDecodedDictionary(string input)
        {
            StringBuilder result = new StringBuilder();
            string[] wordsEncrypted;
            if (input[0] == '0' || input.Last() == '0')
                input = input.Trim('0');

            int samplingRate = GetSamplingRate(input);
            input = (samplingRate != 0) ? Helper.SampleBit(input.AsMemory(), samplingRate) : input;
            wordsEncrypted = Helper.ValidateAsFrase(input.AsSpan(), spaceBetweenWords.AsSpan()) ? SplitByWords(input) : new string[] { input };
            wordsEncrypted.ToList().ForEach(x => SplitByChars(x).ToList().ForEach(y => result.Append(GetCharMorsedDict(GetBit(y)))));
            return result.ToString();
        }
        public static string ReturnDecodedDictionaryStream(string input)
        {
            StringBuilder finalResult = new StringBuilder();
            StringBuilder localResult = new StringBuilder();

            if (input[0] == '0' || input.Last() == '0')
                input = input.Trim('0');

            ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(input.ToCharArray());
            int samplingRate = readOnlySpan.GetSamplingRate();
            input = (samplingRate != 0) ? Helper.SampleBit(input.AsMemory(), samplingRate) : input;

            if (Helper.ValidateAsFrase(input.AsSpan(), spaceBetweenWords.AsSpan()))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    localResult.Append(input[i]);

                    if (localResult.ToString().EndsWith(spaceBetweenChars))
                    {
                       localResult = localResult.Remove(localResult.Length - 3, 3);
                       finalResult.Append(GetCharMorsedDict(GetBit(localResult.ToString())));
                       localResult.Clear();                        
                    }
                    if (localResult.ToString().EndsWith(spaceBetweenWords) )
                    {
                        finalResult.Append($"{GetCharMorsedDict(GetBit(localResult.ToString()))} ");
                        localResult.Clear();
                    }
                }
            }           
            else if (input.AsSpan().ValidateAsWord(input.AsSpan()))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    localResult.Append(input[i]);

                    if (localResult.ToString().EndsWith(spaceBetweenChars))
                    {
                        localResult = localResult.Remove(localResult.Length - 3, 3);
                        finalResult.Append(GetCharMorsedDict(GetBit(localResult.ToString())));
                        localResult.Clear();
                    }                    
                }                                  
            }
            else            
              finalResult.Append(GetCharMorsedDict(GetBit(input)));

            return finalResult.ToString();
        }

        public static string ReturnDecodedDictionaryStreamNew(string input)
        {
            if (input[0] == '0' || input.Last() == '0')
                input = input.Trim('0');

            ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(input.ToCharArray());
            int samplingRate = readOnlySpan.GetSamplingRate();
            readOnlySpan = (samplingRate != 0) ? readOnlySpan.SampleBit(samplingRate) : readOnlySpan;

            StringBuilder stringBuilder = new StringBuilder();
            if (readOnlySpan.ValidateAsFrase())
            {
                Span<string> wordsSpan = SplitByWords(input);
                for (int i = 0; i < wordsSpan.Length; i++)
                {
                    Span<string> res = wordsSpan[i].SplitByChars();
                    for (int j = 0; j < res.Length; j++)
                    {
                        stringBuilder.Append($"{res[j].ConvertMorseToChar()} ");
                    }
                }
            }
            else if (readOnlySpan.ValidateAsWord())
            {
                Span<string> charSpan = input.SplitByChars();
                for (int j = 0; j < charSpan.Length; j++)
                {
                    stringBuilder.Append(charSpan[j].ConvertMorseToChar());
                }
            }
            else
            {
                stringBuilder.Append(input.ConvertMorseToChar());
            }         

            return stringBuilder.ToString();
        }

        public static double CalculateChange(long previous, long current)
        {
            if (previous == 0)
                throw new InvalidOperationException();

            var change = current - previous;
            return (double)change / previous;
        }
        public static string DoubleToPercentageString( double d) => "%" + (Math.Round(d, 2) * 100).ToString().Split('.').First();
        
        static void Main(string[] args)
        {
            

            List<long> results = new List<long>();
            List<string> textresults = new List<string>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var testCase in Helper.GetCasesDeserialized(path))
            {
                stopwatch.Restart();
                textresults.Add($"result NewSwitchCases={ReturnDecodedDictionaryStreamNew(testCase.Encrypted)} " +
                                  $"expected={testCase.Decrypted} time={stopwatch.Elapsed}");
                results.Add(stopwatch.ElapsedTicks);
                stopwatch.Restart();


                stopwatch.Restart();
                textresults.Add($"result NestedSwitchCases={ReturnDecodedSwitch(testCase.Encrypted).ToString()} " +
                                  $"expected={testCase.Decrypted} time={stopwatch.Elapsed}");
                results.Add(stopwatch.ElapsedTicks);
                stopwatch.Restart();
                
                textresults.Add($"result WordsSplitDictionary={ReturnDecodedDictionary(testCase.Encrypted).ToString()} " +
                                  $"expected={testCase.Decrypted} time={stopwatch.Elapsed}");
                results.Add(stopwatch.ElapsedTicks);
                stopwatch.Restart();
                
                textresults.Add($"result DictionaryStream={ReturnDecodedDictionaryStream(testCase.Encrypted).ToString()} " +
                                  $"expected={testCase.Decrypted} time={stopwatch.Elapsed}");
                results.Add(stopwatch.ElapsedTicks);
                var min = results.Min();
                var lowest = results.IndexOf(min);
                for (int i = 0; i < textresults.Count; i++)
                {
                    if (i == lowest)
                    {
                        Console.WriteLine($"{textresults[i]} best results");
                    }
                    else
                    {
                        Console.WriteLine($"{textresults[i]} {DoubleToPercentageString(CalculateChange(results[lowest], results[i]))} longer than best ");
                    }
                }
                Console.WriteLine();
                textresults.Clear();
                results.Clear();
            }
            

            Console.ReadLine();
        }

    }
}
