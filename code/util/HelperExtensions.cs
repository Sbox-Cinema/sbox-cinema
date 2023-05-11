using System.Linq;
using System;

public static class HelperExtensions
{
    public static string ToNiceBinary(this ulong number)
    {
        var pretty = "";
        var binary = Convert.ToString((long)number, 2);
        var hanging = binary.Length % 4;
        if (hanging > 0)
        {
            binary = binary.PadLeft(binary.Length + (4 - hanging), '0');
        }

        for (int i = 0; i < binary.Length; i++)
        {
            if (i > 0 && i % 4 == 0 && i < binary.Length - 1)
            {
                pretty += "|";
            }
            pretty += binary[i];
        }

        return pretty;
    }
}
