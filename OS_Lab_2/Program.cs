using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Channels;
using System.Security.Cryptography;
using System.Text;

namespace OS_Lab_2;

public class Program
{
    //const string path = @"D:\Repos\OS_Lab_2\hashes.txt";
    public static bool foundFlag = false;
    public static void Main(string[] args)
    {
        string action;

        while (true)
        {
            Menu.ShowMenu();
            Console.Write("Выберите действие, которое хотите совершить: ");
            action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    HashFounder.ShowHashes();
                    Console.Write("Выберите Хеш-значение, по которому будет произведен подбор: ");
                    byte hashNumber = byte.Parse(Console.ReadLine());

                    //string[] readText = File.ReadAllLines(path);
                    //string hashValue = readText[hashNumber - 1].ToUpper();

                    Console.Write("Введите количество потоков для подбора: ");
                    int streamCount = int.Parse(Console.ReadLine());
                    Console.WriteLine("Подбор пароля начался...");

                    Channel<string> channel = Channel.CreateBounded<string>(streamCount);
                    Stopwatch time = new();
                    time.Reset();
                    time.Start();

                    var passPicker = Task.Run(() => { new PassPicker(channel.Writer); });
                    Task[] streams = new Task[streamCount + 1];
                    streams[0] = passPicker;

                    for (int i = 1; i < streamCount + 1; i++)
                    {
                        streams[i] = Task.Run(() => { new HashFounder(channel.Reader, hashNumber); });
                    }

                    Task.WaitAny(streams);
                    time.Stop();
                    Console.WriteLine("Время, затраченное на подбор: " + time.Elapsed);
                    Menu.FinishOfTheFunction();
                    foundFlag = false;
                    break;
                case "2":
                    Console.Clear();
                    break;
                case "3":
                    Console.WriteLine("Выход из программы...");
                    return;
                default:
                    Console.WriteLine("Неизвестный номер меню!");
                    Console.ReadLine();
                    break;
            }
        }
    }

    //инициализация словаря из прописных латинских букв
    public static Dictionary<int, char> InitiateDict()
    {
        Dictionary<int, char> dict = new();
        for (int i = 97, count = 0; i < 123; i++, count++)
        {
            dict.Add(count, (char)i);
        }
        return dict;
    }

    //получение хеша из полученного пароля
    public static string GetCheckSum(string strValue)
    {
        string sha256ToString = string.Empty;

        byte[] bytes = Encoding.GetEncoding("utf-8").GetBytes(strValue);
        var sha = new SHA256Managed();
        byte[] checkSum = sha.ComputeHash(bytes);
        sha256ToString = BitConverter.ToString(checkSum).Replace("-", string.Empty);

        return sha256ToString;
    }

    //подбор пятибуквенного пароля, получение его хеша и проверка этого хеша на сходство с одним из заданных
    public static void Thread()
    {
        string strValue1 = "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad";
        string strValue2 = "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b";
        string strValue3 = "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f";
        Dictionary<int, char> dict = InitiateDict();

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                for (int k = 0; k < 26; k++)
                {
                    for (int l = 0; l < 26; l++)
                    {
                        for (int m = 0; m < 26; m++)
                        {
                            string str = (dict[i].ToString() + dict[j].ToString() + dict[k].ToString() + dict[l].ToString() + dict[m].ToString()).ToLower();
                            if (GetCheckSum(str) == strValue1 || GetCheckSum(str) == strValue2 || GetCheckSum(str) == strValue3)
                            {
                                Console.WriteLine(str);
                            }
                        }
                    }
                }
            }
        }
    }

    //подбор пятибуквенных паролей через 1 цикл, а не 5 вложенных
    //а также попытка распараллелить его через встроенный метод
    //которая увенчалась провалом, потому что иногда полученные слова могут повторяться
    //из-за чего некоторых слов вовсе может не быть
    public static void Cycle()
    {
        char fifth, fourth, third, second, first;
        string value;
        for (int i = 0; i < 11881376; i++) // 11881376
        {
            fifth = (char)(i % 26 + 65);
            fourth = (char)((i / 26) % 26 + 65);
            third = (char)(((i / 26) / 26) % 26 + 65);
            second = (char)((((i / 26) / 26) / 26) % 26 + 65);
            first = (char)((((i / 26) / 26) / 26) / 26 + 65);

            value = (first.ToString() + second.ToString() + third.ToString() + fourth.ToString() + fifth.ToString()).ToLower();

            Console.WriteLine(value);
        }

        ParallelOptions threads = new();
        threads.MaxDegreeOfParallelism = 100;

        Parallel.For(0, 5, threads, i =>
        {
            fifth = (char)(i % 26 + 65);
            fourth = (char)((i / 26) % 26 + 65);
            third = (char)(((i / 26) / 26) % 26 + 65);
            second = (char)((((i / 26) / 26) / 26) % 26 + 65);
            first = (char)((((i / 26) / 26) / 26) / 26 + 65);

            value = (first.ToString() + second.ToString() + third.ToString() + fourth.ToString() + fifth.ToString()).ToLower();

            Console.WriteLine(value);
        });
    }
}
