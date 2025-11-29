using Lib.Color;

namespace UnitTests;

[TestFixture]
public class ColorTests
{
    [SetUp]
    public void Setup()
    {
        // Not implemented yet   
    }

    [Test]
    public void Nibble_Parse()
    {
        // Setup
        var valid = "0123456789ABCDEFabcdef";
        var invalid = "/:@G`g"; // First ASCII character outside valid ranges: [0-9][A-F][a-f]

        // Assert valid inputs
        foreach (char c in valid)
            Assert.DoesNotThrow(() => Color.ParseNibble(c));

        // Assert invalid inputs
        foreach (char c in invalid)
            Assert.Throws<ArgumentException>(() => Color.ParseNibble(c));
    }

    [Test]
    public void String_Parse()
    {
        // Setup
        var nibbles = "0123456789ABCDEF";
        Dictionary<string, string> validShort = new()
        {
            { "#RGB", "#012" },
            { "RGB", "345" },
            { "#RGBA", "#6789" },
            { "RGBA", "AbCd" },
        };
        Dictionary<string, string> validLong = new()
        {
            { "#RRGGBB", "#Ef0123" },
            { "RRGGBB", "456789" },
            { "#RRGGBBAA", "#AbDeF012" },
            { "RRGGBBAA", "3456789a" },
        };
        Dictionary<string, string> invalid = new()
        {
            { "Length0", "" },
            { "Length1", "1" },
            { "Length2", "22" },
            { "Length5", "55555" },
            { "Length7", "7777777" },
            { "Length9", "999999999" },
            { "Invalid hexadecimal character", "XYZ" },
        };
        int Nibble(char c) => nibbles.IndexOf(char.ToUpper(c));
        void AssertColor(Color color, int r, int g, int b, int a, string msg)
        {
            Assert.Multiple(() =>
            {
                Assert.That(color.R, Is.EqualTo((uint)r), message: msg);
                Assert.That(color.G, Is.EqualTo((uint)g), message: msg);
                Assert.That(color.B, Is.EqualTo((uint)b), message: msg);
                Assert.That(color.A, Is.EqualTo((uint)a), message: msg);
            });
        }

        // Assert short inputs
        foreach (var pair in validShort)
        {
            var color = Color.Parse(pair.Value);
            var hex = pair.Value.TrimStart('#');
            var r = Nibble(hex[0]) << 4;
            var g = Nibble(hex[1]) << 4;
            var b = Nibble(hex[2]) << 4;
            var a = hex.Length == 4 ? Nibble(hex[3]) << 4 : 255;
            AssertColor(color, r, g, b, a, pair.Value);
        }

        // Assert long inputs
        foreach (var pair in validLong)
        {
            var color = Color.Parse(pair.Value);
            var hex = pair.Value.TrimStart('#');
            var r = Nibble(hex[0]) << 4 | Nibble(hex[1]);
            var g = Nibble(hex[2]) << 4 | Nibble(hex[3]);
            var b = Nibble(hex[4]) << 4 | Nibble(hex[5]);
            var a = hex.Length == 8 ? Nibble(hex[6]) << 4 | Nibble(hex[7]) : 255;
            AssertColor(color, r, g, b, a, pair.Value);
        }

        // Assert invalid inputs
        foreach (var pair in invalid)
            Assert.Throws<ArgumentException>(() => Color.Parse(pair.Value));
    }
}
