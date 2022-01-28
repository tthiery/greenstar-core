using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GreenStar.Core.Resources;

/// <summary>
/// A collection of resources
/// </summary>
public class ResourceAmount
{
    /// <summary>
    /// Creates a resource amount
    /// </summary>
    public ResourceAmount()
    {
        Values = new List<ResourceAmountItem>();
    }

    public ResourceAmount(params ResourceAmountItem[] items)
    {
        Values = new List<ResourceAmountItem>(items);
    }

    /// <summary>
    /// The name of this resource amount (e.g. a source)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The list of resources
    /// </summary>
    public List<ResourceAmountItem> Values { get; set; }

    /// <summary>
    /// Easy access methods with flyweight support
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int this[string name]
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null or empty", "name");
            }

            return this.Values.FirstOrDefault(x => x.Resource == name)?.Value ?? 0;
        }

        set
        {
            var item = this.Values.FirstOrDefault(x => x.Resource == name);

            if (item == null)
            {
                item = new ResourceAmountItem(name, 0);

                this.Values.Add(item);
            }

            item.Value = value;
        }
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
        var result = new ResourceAmount();

        foreach (var current in x.Values)
        {
            result[current.Resource] = current.Value;
        }

        foreach (var newValue in added.Values)
        {
            result[newValue.Resource] += newValue.Value;
        }

        return result;
    }

    /// <summary>
    /// Subtraction of two resource amounts
    /// </summary>
    /// <param name="x"></param>
    /// <param name="substract"></param>
    /// <returns></returns>
    public static ResourceAmount operator -(ResourceAmount x, ResourceAmount substract)
    {
        var result = new ResourceAmount();

        foreach (var current in x.Values)
        {
            result[current.Resource] = current.Value;
        }

        foreach (var newValue in substract.Values)
        {
            result[newValue.Resource] -= newValue.Value;
        }

        return result;
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
        var result = new ResourceAmount();

        foreach (var element in left.Values)
        {
            int newValue = Convert.ToInt32((element.Value * 1.0f) * multiplier);

            result[element.Resource] = newValue;
        }

        return result;
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
