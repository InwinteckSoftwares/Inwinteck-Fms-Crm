// VapidKeyGenerator.cs
using System;
using WebPush;

public class VapidKeyGenerator
{
    public static void Main()
    {
        var keys = VapidHelper.GenerateVapidKeys();
        Console.WriteLine("Public Key: " + keys.PublicKey);
        Console.WriteLine("Private Key: " + keys.PrivateKey);
    }
}
