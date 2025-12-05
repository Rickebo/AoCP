using System.Text.Json;
using Lib.Grids;
using ColorStruct = Lib.Color.Color;
using ColorConstants = Lib.Color.Colors;

namespace Lib.Color.Tests;

public class ColorTests
{
    [Test]
    public void ParseNibble_SupportsDigitsAndHexLetters()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ColorStruct.ParseNibble('5'), Is.EqualTo(5u));
            Assert.That(ColorStruct.ParseNibble('A'), Is.EqualTo(10u));
        });

        Assert.Throws<ArgumentException>(() => ColorStruct.ParseNibble('x'));
    }

    [Test]
    public void Parse_SupportsShortAndLongFormats()
    {
        var shortForm = ColorStruct.Parse("#abc");
        var longForm = ColorStruct.Parse("#11223344");

        Assert.Multiple(() =>
        {
            Assert.That(shortForm.R, Is.EqualTo(0xA0));
            Assert.That(shortForm.G, Is.EqualTo(0xB0));
            Assert.That(shortForm.B, Is.EqualTo(0xC0));
            Assert.That(shortForm.A, Is.EqualTo(0xFF));

            Assert.That(longForm.R, Is.EqualTo(0x11));
            Assert.That(longForm.G, Is.EqualTo(0x22));
            Assert.That(longForm.B, Is.EqualTo(0x33));
            Assert.That(longForm.A, Is.EqualTo(0x44));
        });
    }

    [Test]
    public void FromArgb_ReordersAlphaToLeastSignificantByte()
    {
        var color = ColorStruct.FromArgb(0x11223344);

        Assert.Multiple(() =>
        {
            Assert.That(color.R, Is.EqualTo(0x22));
            Assert.That(color.G, Is.EqualTo(0x33));
            Assert.That(color.B, Is.EqualTo(0x44));
            Assert.That(color.A, Is.EqualTo(0x11));
        });
    }

    [Test]
    public void FromRange_ClampsBetweenZeroAndOne()
    {
        Assert.Multiple(() =>
        {
            Assert.That(ColorStruct.FromRange(1.5), Is.EqualTo((byte)255));
            Assert.That(ColorStruct.FromRange(-0.5), Is.EqualTo((byte)0));
        });
    }

    [Test]
    public void FromAndWith_CombineDifferentComponentSources()
    {
        var created = ColorStruct.From(r: 0x10, green: 0.5, b: 0x30, a: 0x40);
        var updated = created.With(green: 1.0, blue: 0.0);

        Assert.Multiple(() =>
        {
            Assert.That(created.R, Is.EqualTo(0x10));
            Assert.That(created.G, Is.EqualTo(0x7F));
            Assert.That(created.B, Is.EqualTo(0x30));
            Assert.That(created.A, Is.EqualTo(0x40));

            Assert.That(updated.G, Is.EqualTo(0xFF));
            Assert.That(updated.B, Is.EqualTo(0x00));
        });
    }

    [Test]
    public void MultiplyAndAdd_AdjustComponentIntensities()
    {
        var baseColor = ColorStruct.From(red: 0.5, green: 0.25, blue: 0.0);

        var multiplied = baseColor.Multiply(red: 2, green: 2);
        var added = baseColor.Add(red: 0.25, green: 0.5, alpha: -0.5);

        Assert.Multiple(() =>
        {
            Assert.That(multiplied.R, Is.EqualTo(0xFE));
            Assert.That(multiplied.G, Is.EqualTo(0x7E));

            Assert.That(added.R, Is.EqualTo(0xBE));
            Assert.That(added.G, Is.EqualTo(0xBE));
            Assert.That(added.A, Is.EqualTo(0x7F));
        });
    }

    [Test]
    public void FromHslAndHsv_CreateExpectedColors()
    {
        var gray = ColorStruct.FromHsl(0, 0, 0.5);
        var red = ColorStruct.FromHsv(0, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(gray.R, Is.EqualTo(0x7F));
            Assert.That(gray.G, Is.EqualTo(0x7F));
            Assert.That(gray.B, Is.EqualTo(0x7F));

            Assert.That(red.R, Is.EqualTo(0xFF));
            Assert.That(red.G, Is.EqualTo(0x00));
            Assert.That(red.B, Is.EqualTo(0x00));
        });
    }

    [Test]
    public void Generate_IsDeterministicForSameSeed()
    {
        var first = ColorStruct.Generate(0.1, 3).ToList();
        var second = ColorStruct.Generate(0.1, 3).ToList();
        var randomSized = ColorStruct.Generate(2).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(first, Has.Count.EqualTo(3));
            Assert.That(second, Is.EqualTo(first).AsCollection);
            Assert.That(randomSized, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Heatmap_InterpolatesValuesAcrossGrid()
    {
        var grid = new ArrayGrid<int>(2, 2);
        grid[0, 0] = 0;
        grid[1, 0] = 10;
        grid[0, 1] = 20;
        grid[1, 1] = 30;

        var from = ColorConstants.Black;
        var to = ColorConstants.White;

        var heatmap = ColorStruct.Heatmap(grid, from, to);

        Assert.Multiple(() =>
        {
            Assert.That(heatmap[0, 0], Is.EqualTo(from));
            Assert.That(heatmap[1, 1], Is.EqualTo(to));
            Assert.That(heatmap[0, 1].R, Is.EqualTo(170).Within(1));
        });
    }

    [Test]
    public void Between_InterpolatesByPercentOrRange()
    {
        var half = ColorStruct.Between(ColorConstants.Black, ColorConstants.White, 0.5);
        var ranged = ColorStruct.Between(ColorConstants.Black, ColorConstants.White, 5, 0, 10);

        Assert.Multiple(() =>
        {
            Assert.That(half.R, Is.EqualTo(128));
            Assert.That(ranged, Is.EqualTo(half));
        });
    }

    [Test]
    public void StringRepresentations_AreFormattedCorrectly()
    {
        var color = ColorStruct.From(r: 0x12, g: 0x34, b: 0x56, a: 0x78);

        Assert.Multiple(() =>
        {
            Assert.That(color.ToRgbaString(), Is.EqualTo("#12345678"));
            Assert.That(color.ToRgbString(), Is.EqualTo("#123456"));
            Assert.That(color.ToArgbString(), Is.EqualTo("#78123456"));
            Assert.That(color.ToString(), Is.EqualTo(color.ToRgbaString()));
        });
    }

    [Test]
    public void EqualityAndOperators_WorkAcrossColorsAndScalars()
    {
        var baseColor = ColorStruct.From(r: 0x10, g: 0x20, b: 0x30, a: 0x40);
        var same = ColorStruct.From(r: 0x10, g: 0x20, b: 0x30, a: 0x40);
        var brightened = baseColor + 0.5;
        var doubled = baseColor * 2;
        var combined = baseColor | ColorConstants.White;

        Assert.Multiple(() =>
        {
            Assert.That(baseColor == same, Is.True);
            Assert.That(baseColor != brightened, Is.True);
            Assert.That(brightened.R, Is.EqualTo(0x8F));
            Assert.That(doubled.G, Is.EqualTo(0x40));
            Assert.That((~baseColor).Value, Is.EqualTo(~baseColor.Value));
            Assert.That(combined.Value, Is.EqualTo(baseColor.Value | ColorConstants.White.Value));
        });
    }

    [Test]
    public void JsonConverter_RoundTripsColor()
    {
        var color = ColorConstants.CornflowerBlue;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new ColorJsonConverter());

        var json = JsonSerializer.Serialize(color, options);
        var parsed = JsonSerializer.Deserialize<ColorStruct>(json, options);

        Assert.That(parsed, Is.EqualTo(color));
    }
}

