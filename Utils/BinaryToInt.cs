namespace PocketNetworker.Utils;

public class BinaryToInt
{
    /// <summary>
    /// Converts the provided binary string to an Int32 value.
    /// </summary>
    /// <param name="binaryString"></param>
    /// <returns>Int32 value.</returns>
    public static int ToInt(string binaryString)
    {
        return Convert.ToInt32(binaryString, 2);
    }
}