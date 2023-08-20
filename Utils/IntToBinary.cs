namespace PocketNetworker.Utils;

public class IntToBinary
{
    private static string binaryString;

    /// <summary>
    /// Convert the given number to a binary string (without padding).
    /// </summary>
    /// <param name="num"></param>
    private static void ToBinary(int num)
    {
        binaryString = Convert.ToString(num, 2);
    }

    /// <summary>
    /// Add padding (zeros) to the binary string.
    /// </summary>
    private static void AddPadding()
    {
        int len = 9 - binaryString.Length;
        binaryString = binaryString.PadLeft(len, '0');
    }

    /// <summary>
    /// Convert the provided number to a binary string.
    /// </summary>
    /// <param name="num"></param>
    /// <returns>Binary string of 8 characters long of the provided number.</returns>
    public static string ConvertToBinary(int num)
    {
        ToBinary(num);
        AddPadding();
        //AddBlocks();

        return binaryString;
    }
}
