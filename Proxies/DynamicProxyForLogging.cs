﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using ImpromptuInterface;

namespace Proxies
{
    // add ImpromptuInterface nuget for generating appropriate dynamic proxy methods.
    public interface IBankAccount
    {
        void Deposit(int amount);
        bool Withdraw(int amount);
        string ToString();
    }

    public class BankAccount : IBankAccount
    {
        private int balance;
        private int overdraftLimit = -500;

        public void Deposit(int amount)
        {
            balance += amount;
            Console.WriteLine($"Deposited ${amount}, balance is now {balance}");
        }

        public bool Withdraw(int amount)
        {
            if (balance - amount >= overdraftLimit)
            {
                balance -= amount;
                Console.WriteLine($"Withdrew ${amount}, balance is now {balance}");
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{nameof(balance)}: {balance}";
        }
    }

    public class Log<T> : DynamicObject
        where T : class, new()
    {
        private readonly T subject;

        private Dictionary<string, int> methodCallCount
            = new Dictionary<string, int>();

        public Log(T subject)
        {
            this.subject = subject;
        }

        public static I As<I>()
            where I : class
        {
            if(!typeof(I).IsInterface)
                throw new ArgumentNullException("I must be an interface type!");

            // impromptu dynamic method 
            return new Log<T>(new T()).ActLike();
        }

        // factory method to force dynamic object as if it were implementing a particular interface
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                Console.WriteLine($"Invoking {subject.GetType().Name}.{binder.Name} with arguments " +
                                  $"[{string.Join(", ", args)}]");

                if (methodCallCount.ContainsKey(binder.Name)) methodCallCount[binder.Name]++;
                else methodCallCount.Add(binder.Name, 1);

                result = subject.GetType().GetMethod(binder.Name).Invoke(subject, args);
                return true;
            }
            catch 
            {
                result = null;
                return false;
            }
        }

        // dynamic proxy
        public string Info
        {
            get
            {
                var sb = new StringBuilder();
                foreach (KeyValuePair<string, int> kv in methodCallCount)
                {
                    sb.AppendLine($"{kv.Key} called {kv.Value} times(s)");
                }

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return $"{Info}\n{subject}";
        }
    }

    public class DynamicProxyForLogging
    {
        // change to Main to run. 
        public static void none(string[] args)
        {
            var ba = Log<BankAccount>.As<IBankAccount>();
            ba.Deposit(100);
            ba.Withdraw(50);
            Console.WriteLine(ba);
        }
    }
}
