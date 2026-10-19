using System;
using System.Reactive.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();

        IObservable<int> numbers = Observable
            .Interval(TimeSpan.FromMilliseconds(500))
            .Select(value => random.Next(1, 101))
            .Where(number => number > 50)
            .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(10)));

        Console.WriteLine("Генерация случайных чисел больше 50 в течение 10 секунд:");

        using (ManualResetEventSlim finished = new ManualResetEventSlim(false))
        {
            using (IDisposable subscription = numbers.Subscribe(
                number => Console.WriteLine("Получено число: " + number),
                error =>
                {
                    Console.WriteLine("Ошибка: " + error.Message);
                    finished.Set();
                },
                () =>
                {
                    Console.WriteLine("Генерация завершена.");
                    finished.Set();
                }))
            {
                finished.Wait();
            }
        }
    }
}
