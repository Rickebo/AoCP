using Lib.Extensions;
using Lib.Parsing;
using System.Globalization;
using System.Text;

namespace LibUnitTests;

[TestFixture]
internal class ParsingTests
{
    private readonly string _brackets = "<{[()]}>";
    private readonly string _symbols = "!@#£¤$%&/=+?´`-:;|§½'*^~¨_,. ";
    private readonly string _special = "\"\r\n";
    private readonly string _nordicCharacters = "åäöÅÄÖ";
    private readonly string _standardLower = "abcdefghijklmnopqrstuvxyz";
    private readonly string _standardUpper = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
    private readonly string _numbers = "0123456789";

    [Test]
    public void Number_Parse()
    {
        // Setup
        List<int> validInt = [];
        List<uint> validUInt = [];
        List<long> validLong = [];
        List<ulong> validULong = [];
        List<double> validDouble = [];
        List<decimal> validDecimal = [];

        static char PickChar(string str, int index) => str[MathExtensions.Modulo(index, str.Length)];
        string Garbage(int i)
        {
            StringBuilder sb = new();
            sb.Append(PickChar(_brackets, i));
            sb.Append(PickChar(_symbols, i));
            sb.Append(PickChar(_special, i));
            sb.Append(PickChar(_nordicCharacters, i));
            sb.Append(PickChar(_standardLower, i));
            sb.Append(PickChar(_standardUpper, i));
            return sb.ToString();
        }

        string intString = string.Empty;
        for (int i = -50; i < 50; i++)
        {
            intString += i;
            intString += Garbage(i);
            validInt.Add(i);
            validUInt.Add((uint)Math.Abs(i));
            validLong.Add(i);
            validULong.Add((ulong)Math.Abs(i));
        }

        string decimalStringDot = string.Empty;
        NumberFormatInfo nfiDot = new()
        {
            NumberDecimalSeparator = "."
        };
        string decimalStringComma = string.Empty;
        NumberFormatInfo nfiComma = new()
        {
            NumberDecimalSeparator = ","
        };
        for (decimal d = -10m; d < 10m; d += 0.2m)
        {
            decimalStringDot += d.ToString(nfiDot);
            decimalStringComma += d.ToString(nfiComma);
            decimalStringDot += Garbage((int)(d / 0.2m));
            decimalStringComma += Garbage((int)(d / 0.2m));
            validDouble.Add((double)d);
            validDecimal.Add(d);
        }

        // Assert int
        List<int> intValues = [.. Parser.GetValues<int>(intString)];
        Assert.That(intValues, Is.EquivalentTo(validInt));

        // Assert uint
        List<uint> uintValues = [.. Parser.GetValues<uint>(intString)];
        Assert.That(uintValues, Is.EquivalentTo(validUInt));

        // Assert long
        List<long> longValues = [.. Parser.GetValues<long>(intString)];
        Assert.That(longValues, Is.EquivalentTo(validLong));

        // Assert ulong
        List<ulong> ulongValues = [.. Parser.GetValues<ulong>(intString)];
        Assert.That(ulongValues, Is.EquivalentTo(validULong));

        // Assert double with dot separator
        List<double> doubleValues = [.. Parser.GetValues<double>(decimalStringDot, ".")];
        Assert.That(doubleValues, Is.EquivalentTo(validDouble));

        // Assert double with comma separator
        List<double> doubleValuesComma = [.. Parser.GetValues<double>(decimalStringComma, ",")];
        Assert.That(doubleValuesComma, Is.EquivalentTo(validDouble));

        // Assert decimal with dot separator
        List<decimal> decimalValues = [.. Parser.GetValues<decimal>(decimalStringDot, ".")];
        Assert.That(doubleValues, Is.EquivalentTo(validDecimal));

        // Assert decimal with comma separator
        List<decimal> decimalValuesComma = [.. Parser.GetValues<decimal>(decimalStringComma, ",")];
        Assert.That(doubleValuesComma, Is.EquivalentTo(validDecimal));

        // Assert invalid comma separator
        Assert.DoesNotThrow(() => Parser.GetValues<int>("", ":"));
        Assert.DoesNotThrow(() => Parser.GetValues<uint>("", ":"));
        Assert.DoesNotThrow(() => Parser.GetValues<long>("", ":"));
        Assert.DoesNotThrow(() => Parser.GetValues<ulong>("", ":"));
        Assert.Throws<NotSupportedException>(() => Parser.GetValues<double>("", ":"));
        Assert.Throws<NotSupportedException>(() => Parser.GetValues<decimal>("", ":"));

        // Assert invalid type
        Assert.Throws<NotSupportedException>(() => Parser.GetValues<bool>(""));

        // Assert empty string
        Assert.Multiple(() =>
        {
            Assert.That(Parser.GetValues<int>(""), Is.Empty);
            Assert.That(Parser.GetValues<uint>(""), Is.Empty);
            Assert.That(Parser.GetValues<long>(""), Is.Empty);
            Assert.That(Parser.GetValues<ulong>(""), Is.Empty);
            Assert.That(Parser.GetValues<double>(""), Is.Empty);
            Assert.That(Parser.GetValues<double>("", ","), Is.Empty);
            Assert.That(Parser.GetValues<decimal>(""), Is.Empty);
            Assert.That(Parser.GetValues<decimal>("", ","), Is.Empty);
        });
    }
}
