namespace OS_Lab_2
{
    public class Menu
    {
        private static string[] functions =
            {
                "1. Запустить брут-форс пароля.",
                "2. Очистить консоль.",
                "3. Выход из программы."
            };
        public static void ShowMenu()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(functions[i]);
            }
        }

        public static void FinishOfTheFunction()
        {
            Console.WriteLine("Нажмите любую клавишу для продолжения...\n");
            Console.ReadKey();
        }
    }
}
