using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Resources;

public record ResourceAmountItem(string Resource, int Value);

/// <summary>
/// A collection of resources
/// </summary>
public record ResourceAmount(string Name, ResourceAmountItem[] Values)
{
    public ResourceAmount(params ResourceAmountItem[] items)
        : this(string.Empty, items)
    { }

    public static ResourceAmount Empty => new ResourceAmount();

    public ResourceAmount WithResource(string name, int value)
    {
        var newResources = this.Values;

        var newItem = new ResourceAmountItem(name, value);

        if (Values.Any(r => r.Resource == name))
        {
            newResources = Values.Select(r => (r.Resource == name) ? newItem : r).ToArray();
        }
        else
        {
            newResources = new ResourceAmountItem[Values.Length + 1];
            Array.Copy(Values, newResources, Values.Length);
            newResources[Values.Length] = newItem;
        }

        return this with
        {
            Values = newResources,
        };
    }

    public int this[string name]
    {
        get => this.Values.FirstOrDefault(x => x.Resource == name)?.Value ?? 0;
    }

    #region Math Support
    /// <summary>
    /// Addition of two resource amounts
    /// </summary>
    /// <param name="x"></param>
    /// <param name="added"></param>
    /// <returns></returns>
    public static ResourceAmount operator +(ResourceAmount x, ResourceAmount added)
    {
        var resourceNames = x.Values.Select(r => r.Resource).Union(added.Values.Select(r => r.Resource)).Distinct();
        var newResources = resourceNames.Select(n => new ResourceAmountItem(n, x[n] + added[n])).ToArray();

        return x with
        {
            Values = newResources,
        };
    }

    /// <summary>
    /// Subtraction of two resource amounts
    /// </summary>
    /// <param name="x"></param>
    /// <param name="substract"></param>
    /// <returns></returns>
    public static ResourceAmount operator -(ResourceAmount x, ResourceAmount substract)
    {
        var resourceNames = x.Values.Select(r => r.Resource).Union(substract.Values.Select(r => r.Resource)).Distinct();
        var newResources = resourceNames.Select(n => new ResourceAmountItem(n, x[n] - substract[n])).ToArray();

        return x with
        {
            Values = newResources,
        };
    }

    /// <summary>
    /// Smaller Than
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator <(ResourceAmount left, ResourceAmount right)
    {
        bool result = left.Values.Select(x => x.Resource).Union(right.Values.Select(x => x.Resource)).Distinct()
            .All(r => left[r] < right[r]);

        return result;
    }

    /// <summary>
    /// Greather Than
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator >(ResourceAmount left, ResourceAmount right)
    {
        bool result = left.Values.Select(x => x.Resource).Union(right.Values.Select(x => x.Resource)).Distinct()
            .All(r => left[r] > right[r]);

        return result;
    }

    /// <summary>
    /// Multiplication
    /// </summary>
    /// <param name="left"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public static ResourceAmount operator *(ResourceAmount left, double multiplier)
    {
        var newResources = left.Values.Select(r => new ResourceAmountItem(r.Resource, Convert.ToInt32((r.Value * 1.0f) * multiplier))).ToArray();

        return left with
        {
            Values = newResources,
        };
    }
    #endregion

    #region Represent the resource amount as text
    /// <summary>
    /// Visualize the resource amount
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => string.Join(", ", Values.Select(v => $"{v.Resource}: {v.Value}"));

    /// <summary>
    /// And returns the value to a resource amount
    /// </summary>
    public static implicit operator ResourceAmount(string value)
        => new ResourceAmount(value.Split(',').Select(v => v.Split(':')).Select(v => new ResourceAmountItem(v[0].Trim(), int.Parse(v[1]))).ToArray());
    #endregion
}
