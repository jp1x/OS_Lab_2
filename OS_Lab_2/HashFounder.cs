using System.Security.Cryptography;
using System.Threading.Channels;
using System.Text;

namespace OS_Lab_2;

public class HashFounder
{
    private ChannelReader<string> Reader;
    private string HashValue;
    public static string[] hashValues =
    {
        "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad",
        "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
        "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f"
    };

    public HashFounder(ChannelReader<string> reader, byte hashNumber)
    {
        Reader = reader;
        HashValue = hashValues[hashNumber - 1].ToUpper();
        Task.WaitAll(Run());
    }

    public static void ShowHashes()
    {
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"\t{i + 1}: {hashValues[i]}");
        }
    }

    static public string FoundHash(string word)
    {
        SHA256 sha256Hash = SHA256.Create();
        byte[] sourceBytes = Encoding.ASCII.GetBytes(word);
        byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
        string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        return hash;
    }

    private async Task Run()
    {
        while (await Reader.WaitToReadAsync())
        {
            if (!Program.foundFlag)
            {
                var item = await Reader.ReadAsync();
                if (FoundHash(item.ToString()) == HashValue)
                {
                    Console.WriteLine("Пароль подобран - " + item);
                    Program.foundFlag = true;
                }
            }
            else
                return;
        }
    }
}
