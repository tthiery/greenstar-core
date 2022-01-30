using Xunit;

namespace GreenStar.Core.Resources;

public class ResourceAmountTest
{
    [Fact]
    public void ResourceAmount_Empty()
    {
        // act
        var actual = new ResourceAmount();

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Empty(actual.Values);
    }

    [Fact]
    public void ResourceAmount_ImplicitConverter()
    {
        // act
        ResourceAmount actual = "Money: 100, Metal: 200";

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Collection(actual.Values,
            r => { Assert.Equal("Money", r.Resource); Assert.Equal(100, r.Value); },
            r => { Assert.Equal("Metal", r.Resource); Assert.Equal(200, r.Value); }
        );
    }

    // IGNORE: Does not fly because of no recursivity in records [Fact]
    public void ResourceAmount_Equality()
    {
        // act
        ResourceAmount a = "Money: 100, Metal: 200";
        ResourceAmount b = "Money: 100, Metal: 200";

        // assert
        Assert.True(a == b);
        Assert.Equal(a, b);
    }

    [Fact]
    public void ResourceAmount_AddSame()
    {
        // arrange
        ResourceAmount a = "Money: 100, Metal: 200";
        ResourceAmount b = "Money: 10, Metal: 20";

        // act
        ResourceAmount actual = a + b;

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Collection(actual.Values,
            r => { Assert.Equal("Money", r.Resource); Assert.Equal(110, r.Value); },
            r => { Assert.Equal("Metal", r.Resource); Assert.Equal(220, r.Value); }
        );
    }
    [Fact]
    public void ResourceAmount_SubtractSame()
    {
        // arrange
        ResourceAmount a = "Money: 100, Metal: 200";
        ResourceAmount b = "Money: 10, Metal: 20";

        // act
        ResourceAmount actual = a - b;

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Collection(actual.Values,
            r => { Assert.Equal("Money", r.Resource); Assert.Equal(90, r.Value); },
            r => { Assert.Equal("Metal", r.Resource); Assert.Equal(180, r.Value); }
        );
    }
    [Fact]
    public void ResourceAmount_Multiply()
    {
        // arrange
        ResourceAmount a = "Money: 100, Metal: 200";

        // act
        ResourceAmount actual = a * 2.5;

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Collection(actual.Values,
            r => { Assert.Equal("Money", r.Resource); Assert.Equal(250, r.Value); },
            r => { Assert.Equal("Metal", r.Resource); Assert.Equal(500, r.Value); }
        );
    }

    [Fact]
    public void ResourceAmount_AddDifferent()
    {
        // arrange
        ResourceAmount a = "Money: 100";
        ResourceAmount b = "Metal: 200";

        // act
        ResourceAmount actual = a + b;

        // assert
        Assert.Equal(string.Empty, actual.Name);
        Assert.Collection(actual.Values,
            r => { Assert.Equal("Money", r.Resource); Assert.Equal(100, r.Value); },
            r => { Assert.Equal("Metal", r.Resource); Assert.Equal(200, r.Value); }
        );
    }

}