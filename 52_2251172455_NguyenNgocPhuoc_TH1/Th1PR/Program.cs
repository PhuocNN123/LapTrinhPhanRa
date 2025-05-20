using System;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static int[] A;
    static List<int> primeCounts;
    static int N, k;
    static readonly object consoleLock = new object();

    static bool IsPrime(int num)
    {
        if (num < 2) return false;
        for (int i = 2; i * i <= num; i++)
        {
            if (num % i == 0) return false;
        }
        return true;
    }

    static void FindPrimes(object obj)
    {
        int threadIndex = (int)obj;
        int segmentSize = N / k;
        int start = threadIndex * segmentSize;
        int end = (threadIndex == k - 1) ? N : start + segmentSize;
        int localPrimeCount = 0;
        //lock (consoleLock)
        //{
        //    Console.WriteLine($"\nCác phần tử do luồng T{threadIndex + 1} xử lý:");
        //    for (int i = start; i < end; i++)
        //    {
        //        Console.Write(A[i] + " ");
        //    }
        //    Console.WriteLine();
        //}
        lock (consoleLock)
        {
            for (int i = start; i < end; i++)
            {
                if (IsPrime(A[i]))
                {
                    localPrimeCount++;
                    string time = DateTime.Now.ToString("HH:mm:ss.fff");
                    lock (consoleLock)
                    {
                        Console.WriteLine($"T{threadIndex + 1}: {A[i]} - {time}");
                    }
                }
            }
        }
        primeCounts[threadIndex] = localPrimeCount;
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Write("Nhập số lượng phần tử: ");
        N = int.Parse(Console.ReadLine());

        Console.Write("Nhập số lượng luồng k: ");
        k = int.Parse(Console.ReadLine());

        A = new int[N];
        Random rand = new Random();
        for (int i = 0; i < N; i++)
        {
            A[i] = rand.Next(2, 100);
        }
        A[N-1] = 11;
        Console.WriteLine("\nMảng A:");
        for (int i = 0; i < N; i++)
        {
            Console.Write(A[i] + " ");
        }
        Console.WriteLine();

        primeCounts = new List<int>(new int[k]);

        Thread[] threads = new Thread[k];
        for (int i = 0; i < k; i++)
        {
            threads[i] = new Thread(FindPrimes);
            threads[i].Start(i);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        int totalPrimes = 0;
        for (int i = 0; i < k; i++)
        {
            totalPrimes += primeCounts[i];
        }
        Console.WriteLine($"\nTổng số lượng số nguyên tố trong mảng A: {totalPrimes}");
    }
}