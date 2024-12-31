using System;
using WebPush;

namespace VapidKeyGeneratorApp
{
    class VapidKeyGenerator
    {
        static void Main(string[] args)
        {
            var keys = VapidHelper.GenerateVapidKeys();
            Console.WriteLine("Public Key: " + keys.PublicKey);
            Console.WriteLine("Private Key: " + keys.PrivateKey);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
