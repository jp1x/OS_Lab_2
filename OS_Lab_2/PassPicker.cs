using System.Threading.Channels;
using System.Threading.Tasks;

namespace OS_Lab_2
{
    public class PassPicker
    {
        private ChannelWriter<string> Writer;

        public PassPicker(ChannelWriter<string> writer)
        {
            Writer = writer;
            Task.WaitAll(Run());
        }

        private async Task Run()
        {
            while (await Writer.WaitToWriteAsync())
            {
                char[] word = new char[5];

                for (int i = 97; i < 123; i++)
                {
                    word[0] = (char)i;
                    for (int j = 97; j < 123; j++)
                    {
                        word[1] = (char)j;
                        for (int k = 97; k < 123; k++)
                        {
                            word[2] = (char)k;
                            for (int l = 97; l < 123; l++)
                            {
                                word[3] = (char)l;
                                for (int m = 97; m < 123; m++)
                                {
                                    word[4] = (char)m;

                                    if (!Program.foundFlag)
                                    {
                                        await Writer.WriteAsync(new string(word));
                                    }
                                    else
                                    {
                                        Writer.Complete();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
