using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace EventTaskSelector
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите задание (1-3):");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Task1();
                    break;
                case "2":
                    Task2();
                    break;
                case "3":
                    Task3();
                    break;
                default:
                    Console.WriteLine("Некорректный выбор.");
                    break;
            }
        }

        static void Task1()
        {

            Timer timer = new Timer();
            Clock clock = new Clock();
            Counter counter = new Counter();

            timer.Tick += clock.OnTick;
            timer.Tick += counter.OnTick;

            Console.WriteLine("Таймер работает 5 секунд...");
            timer.Start(5);

            Console.WriteLine($"Счётчик: {counter.Value}");
        }

        class Timer
        {
            public event EventHandler Tick; // делегат потом название события

            public void Start(int seconds)
            {
                for (int i = 0; i < seconds; i++)
                {
                    Thread.Sleep(1000);
                    Tick?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        class Clock
        {
            public void OnTick(object sender, EventArgs e)
            {
                Console.WriteLine($"Текущее время: {DateTime.Now:T}");
            }
        }

        class Counter
        {
            public int Value { get; private set; }

            public void OnTick(object sender, EventArgs e)
            {
                Value++;
                Console.WriteLine($"Счётчик увеличен: {Value}");
            }
        }

      
static void Task2()
    {
        BankAccount account = new BankAccount();
        Logger logger = new Logger("balance_changes.log");

        account.BalanceChanged += logger.LogBalanceChange;

        account.Deposit(500);
        account.Withdraw(200);
        account.Withdraw(100);
        account.Deposit(300);

        Console.WriteLine($"Финальный баланс: {account.Balance}");
        Console.WriteLine("Изменения баланса записаны в файл.");
    }

    class BankAccount
    {
        public decimal Balance { get; private set; }

        public event Action<decimal> BalanceChanged;

        public void Deposit(decimal amount)
        {
            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }

        public void Withdraw(decimal amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine("Недостаточно средств!");
                return;
            }
            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
        }
    }

    class Logger
    {
        private string _filePath;

        public Logger(string filePath)
        {
            _filePath = filePath;

            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        public void LogBalanceChange(decimal newBalance)
        {
            string log = $"Баланс изменён: {newBalance} (время: {DateTime.Now:T})";
            Console.WriteLine(log);
            File.AppendAllText(_filePath, log + Environment.NewLine);
        }
    }


    static void Task3()
        {

            Button button = new Button("Нажми меня");

            EventHandler handler1 = (sender, e) => Console.WriteLine("Обработчик 1: Кнопка нажата!");
            EventHandler handler2 = (sender, e) => Console.WriteLine("Обработчик 2: Кнопка нажата!");
            EventHandler handler3 = (sender, e) => Console.WriteLine("Обработчик 3: Кнопка нажата!");
            EventHandler handler4 = (sender, e) => Console.WriteLine("Обработчик 4: Кнопка нажата!");

            button.Click += handler1;
            button.Click += handler2;
            button.Click += handler3;
            button.Click += handler4; 

            button.OnClick();
        }

        class Button
        {
            public string Text { get; set; }
            private List<EventHandler> _subscribers = new List<EventHandler>();
            private const int MaxSubscribers = 3;

            public event EventHandler Click
            {
                add
                {
                    if (_subscribers.Contains(value))
                    {
                        Console.WriteLine("Этот обработчик уже подписан.");
                        return;
                    }

                    if (_subscribers.Count >= MaxSubscribers)
                    {
                        Console.WriteLine("Превышено максимальное количество подписчиков.");
                        return;
                    }

                    _subscribers.Add(value);
                }
                remove
                {
                    if (_subscribers.Remove(value))
                        Console.WriteLine("Обработчик отписан.");
                }
            }

            public Button(string text)
            {
                Text = text;
            }

            public void OnClick()
            {
                Console.WriteLine($"Кнопка '{Text}' нажата.");
                foreach (var subscriber in _subscribers)
                    subscriber?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
