using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Primitives;
using PocketNetworker.Utils;
using System.Net.Sockets;
using System.Text;

namespace PocketNetworker.ViewModels;

public partial class PocketNetworkerViewModel : ObservableObject
{
    [ObservableProperty]
    string ipAddress;

    [ObservableProperty]
    string netmask;

    [ObservableProperty]
    string networkClass;

    [ObservableProperty]
    string ipAddrBinary;

    [ObservableProperty]
    string netmaskBinary;

    [ObservableProperty]
    string netmaskIpAddr;

    /// <summary>
    /// Calulate all field values.
    /// </summary>
    private void Calculate()
    {
        int[] octetIntArr = SplitIpAddress(IpAddress);
        string[] octetBinaryArr = new string[octetIntArr.Length];
        for (int i = 0; i < octetIntArr.Length; i++)
        {
            octetBinaryArr[i] = IntToBinary.ConvertToBinary(octetIntArr[i]);
        }

        NetworkClass = CalculateNetworkClass(octetIntArr[0]);

        IpAddrBinary = FormattedBinaryString(StringArrayToString(octetBinaryArr));

        NetmaskBinary = FormattedBinaryString(CalculateNetmaskBinaryString(Netmask));
        NetmaskIpAddr = FormattIntArrayToIpAddressString(BinaryStringToIntArray(NetmaskBinary));
    }

    /// <summary>
    /// Convert the provided binary string into an int array of ip address octets.
    /// </summary>
    /// <param name="binaryString"></param>
    /// <returns>int array of the IP address octets.</returns>
    public int[] BinaryStringToIntArray(String binaryString)
    {
        string[] bsArr = binaryString.Split('.');
        int[] intArr = new int[bsArr.Length];

        for (int i = 0; i < bsArr.Length; i++)
        {
            intArr[i] = BinaryToInt.ToInt(bsArr[i]);
        }

        return intArr;
    }


    /// <summary>
    /// Calculate the netmask binary string from the provided netmask CIDR value.
    /// </summary>
    /// <param name="netmaskString"></param>
    /// <returns>Binary string of the netmask. </returns>
    public string CalculateNetmaskBinaryString(String netmaskString)
    {
        int totalDigits = 32;
        int netmaskNum = int.Parse(netmaskString);
        
        return StringExtensions.Repeat("1", (Math.Max(0, netmaskNum))) + StringExtensions.Repeat("0", Math.Max(0, totalDigits - netmaskNum));
    }

    /// <summary>
    /// Calculate the Network Class based on the first octet of the IP Address.
    /// </summary>
    /// <param name="num"></param>
    /// <returns>String value of the network class. e.g.: 'A'</returns>
    public string CalculateNetworkClass(int num)
    {
        if (num >= 0 && num <= 127)
        {
            return "A";
        } else if (num >= 128 &&  num <= 191)
        {
            return "B";
        } else if (num >= 192 && num <= 223)
        {
            return "C";
        } else if (num >= 224 && num <= 239)
        {
            return "D";
        } else if (num >= 240 && num <= 255)
        {
            return "E";
        } else 
        { 
            return "Unknown"; 
        }
    }

    /// <summary>
    /// Formats the provided binary string to include octet decimals.
    /// </summary>
    /// <param name="binaryString"></param>
    /// <returns>Formatted binary string.</returns>
    public string FormattedBinaryString(string binaryString)
    {
        StringBuilder sb = new StringBuilder(binaryString);
        for (int i = 0; i < binaryString.Length; i++)
        {
            if (i == 8 || i == 17 || i == 26)
            {
                sb.Insert(i, ".");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Format the given int array octets into an IP address string.
    /// </summary>
    /// <param name="intArr"></param>
    /// <returns>String formatted ip address.</returns>
    public string FormattIntArrayToIpAddressString(int[] intArr)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < intArr.Length; i++)
        {
            sb.Append(intArr[i]);
            if (i < intArr.Length - 1)
            {
                sb.Append(".");
            }
        }

        return sb.ToString();
    }


    /// <summary>
    /// Split the provided IP Address string into an int array.
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns>int[] otf the IP Address octets.</returns>
    /// <exception cref="ArgumentException"></exception>
    public int[] SplitIpAddress(string ipAddress)
    {
        string[] octets = ipAddress.Split(".");
        if (octets.Length < 4 || octets.Length > 4)
        {
            throw new ArgumentException($"The split length of the ip address was {octets.Length}.");
        }

        int[] valArr = new int[octets.Length];

        for (int i = 0; i < octets.Length; i++)
        {
            int val = int.Parse(octets[i]);
            if (val < 0 || val > 255)
            {
                throw new ArgumentException($"Illegal value {val} in IP Address.");
            }
            else
            {
                valArr[i] = val;
            }
        }

  
        return valArr;
    }

    /// <summary>
    /// Construct a binary string based on the string array values.
    /// </summary>
    /// <param name="stringArr"></param>
    /// <returns></returns>
    public String StringArrayToString(string[] stringArr)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < stringArr.Length; i++)
        {
            sb.Append(stringArr[i]);
        }

        return sb.ToString();
    }
}
