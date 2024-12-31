using System;
using WebPush;

namespace VapidKeyGenerator2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Generate VAPID keys
            var keys = VapidHelper.GenerateVapidKeys();

            // Display the keys
            Console.WriteLine("VAPID Keys Generated Successfully!\n");
            Console.WriteLine("Public Key:");
            Console.WriteLine(keys.PublicKey);
            Console.WriteLine("\nPrivate Key:");
            Console.WriteLine(keys.PrivateKey);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
