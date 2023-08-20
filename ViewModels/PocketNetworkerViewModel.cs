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

    [ObservableProperty]
    string wildcardBinary;

    [ObservableProperty]
    string wildcardIpAddr;

    [ObservableProperty]
    string networkBinary;

    [ObservableProperty]
    string networkIpAddr;

    [ObservableProperty]
    string broadcastBinary;

    [ObservableProperty]
    string broadcastIpAddr;

    [ObservableProperty]
    string hostsPerNet;

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
        WildcardBinary = FormattedBinaryString(CalculateWilcardBinaryString(Netmask));
        WildcardIpAddr = FormattIntArrayToIpAddressString(BinaryStringToIntArray(WildcardBinary));
        NetworkBinary = LogicalANDing(IpAddrBinary, NetmaskBinary);
        NetworkIpAddr = FormattIntArrayToIpAddressString(BinaryStringToIntArray(NetworkBinary));
        BroadcastBinary = LogicalORing(IpAddrBinary, WildcardBinary);
        BroadcastIpAddr = FormattIntArrayToIpAddressString(BinaryStringToIntArray(BroadcastBinary));
        HostsPerNet = CalculateHostsPerNet(NetmaskBinary).ToString();
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
    /// Calculate the number of max usable hosts based on the netmask.
    /// </summary>
    /// <param name="netmaskBinary"></param>
    /// <returns>int value of max number of usable hosts (2^n - 2).</returns>
    public int CalculateHostsPerNet(string netmaskBinary)
    {
        int maxHosts = CalculateMaxHosts(netmaskBinary);
        return maxHosts - 2;
    }

    /// <summary>
    /// Calculate the number of max host based on the netmask.
    /// </summary>
    /// <param name="netmaskBinary"></param>
    /// <returns>int value of max number of hosts 2^n.</returns>
    public int CalculateMaxHosts(string netmaskBinary)
    {
        int emptyBits = 0;
        for (int i = 0; i < netmaskBinary.Length; i++) 
        {
            if (netmaskBinary[i] == '0')
            {
                emptyBits += 1;
            }
        }

        return (int)Math.Pow(2, emptyBits);
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
    /// Calculate the wildcard binary string from the provided netmask.
    /// </summary>
    /// <param name="netmaskString"></param>
    /// <returns>Wildcard binary string.</returns>
    public string CalculateWilcardBinaryString(string netmaskString)
    {
        int totalDigits = 32;
        int netmaskNum = int.Parse(netmaskString);

        return StringExtensions.Repeat("0", (Math.Max(0, netmaskNum))) + StringExtensions.Repeat("1", Math.Max(0, totalDigits - netmaskNum));
    }

    /// <summary>
    /// Formats the provided binary string to include octet decimals.
    /// </summary>
    /// <param name="binaryString"></param>
    /// <returns>Formatted binary string.</returns>
    public string FormattedBinaryString(string binaryString)
    {
        StringBuilder sb = new(binaryString);
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
        StringBuilder sb = new();
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
    /// Calculate the bitwise AND of two binary strings.
    /// </summary>
    /// <param name="bs1"></param>
    /// <param name="bs2"></param>
    /// <returns>String output of bitwise AND.</returns>
    public string LogicalANDing(string bs1, string bs2)
    {
        StringBuilder stringBuilder = new();
        for (int i = 0; i < bs1.Length; i++)
        {
            // '0' converts the char to int.
            stringBuilder.Append(bs1[i] - '0' & bs2[i] - '0');
        }


        return stringBuilder.ToString();
    }

    /// <summary>
    /// Calculate the bitwise OR of two binary strings.
    /// </summary>
    /// <param name="bs1"></param>
    /// <param name="bs2"></param>
    /// <returns>String output of bitwise OR.</returns>
    public string LogicalORing(string bs1, string bs2)
    {
        StringBuilder stringBuilder = new();
        for (int i = 0; i < bs1.Length; i++)
        {
            // '0' converts the char to int.
            stringBuilder.Append(bs1[i] - '0' | bs2[i] - '0');
        }

        return stringBuilder.ToString();
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
        StringBuilder sb = new();
        for (int i = 0; i < stringArr.Length; i++)
        {
            sb.Append(stringArr[i]);
        }

        return sb.ToString();
    }
}
