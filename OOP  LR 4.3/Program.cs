using System;
using System.Threading;

namespace OOP__LR_4._3
{
    class Program
    {
        delegate int RandomNumberDelegate();

        static void Main(string[] args)
        {
            RandomNumberDelegate[] delegates = new RandomNumberDelegate[5];

            for (int i = 0; i < delegates.Length; i++)
            {
                delegates[i] = () => new Random().Next(100);
            }
            RetryPolicy retryPolicy = new RetryPolicy(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));

            double average = RetryWithExponentialBackoff(() => CalculateAverage(delegates), retryPolicy);
            Console.WriteLine("Середнє арифметичне: " + average);
        }

        static double CalculateAverage(RandomNumberDelegate[] delegates)
        {
            int sum = 0;

            foreach (RandomNumberDelegate del in delegates)
            {
                sum += del();
            }

            double average = (double)sum / delegates.Length;
            return average;
        }

        static T RetryWithExponentialBackoff<T>(Func<T> action, RetryPolicy retryPolicy)
        {
            int retryCount = 0;
            Random random = new Random();

            while (true)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    if (retryCount >= retryPolicy.MaxRetryCount)
                    {
                        throw new Exception("Не вдалося виконати дію після повторних спроб.", ex);
                    }

                    int delay = CalculateDelay(retryPolicy, retryCount, random);
                    Thread.Sleep(delay);

                    retryCount++;
                }
            }
        }

        static int CalculateDelay(RetryPolicy retryPolicy, int retryCount, Random random)
        {
            int maxJitter = (int)(retryPolicy.MaxDelay.TotalMilliseconds * 0.1);
            int jitter = random.Next(-maxJitter, maxJitter + 1);
            double exponentialDelay = Math.Pow(2, retryCount) * retryPolicy.InitialDelay.TotalMilliseconds;
            int delay = (int)(exponentialDelay + jitter);

            return Math.Min(delay, (int)retryPolicy.MaxDelay.TotalMilliseconds);
        }
    }
    class RetryPolicy
    {
        public int MaxRetryCount { get; }
        public TimeSpan InitialDelay { get; }
        public TimeSpan MaxDelay { get; }

        public RetryPolicy(int maxRetryCount, TimeSpan initialDelay, TimeSpan maxDelay)
        {
            MaxRetryCount = maxRetryCount;
            InitialDelay = initialDelay;
            MaxDelay = maxDelay;
        }
    }
}
