using System;

using Xunit;

namespace GreenStar;

public class BasicDataSerializerTest
{
    [Fact]
    public void BasicDataSerializer_GuidArray()
    {
        // act
        Assert.True(BasicDataSerializer.TryDeserialize<Guid[]>("f0996973-63b1-40d8-9ca7-d299211488e6,f0996973-63b1-40d8-9ca7-d299211488e7", out var result));

        Assert.Equal(Guid.Parse("f0996973-63b1-40d8-9ca7-d299211488e6"), result[0]);
        Assert.Equal(Guid.Parse("f0996973-63b1-40d8-9ca7-d299211488e7"), result[1]);
    }
}