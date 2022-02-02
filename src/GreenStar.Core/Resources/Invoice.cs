using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GreenStar.Resources;

/// <summary>
/// A collection of resource amounts for the purpose of billing
/// </summary>
public class Invoice
{
    /// <summary>
    /// The name for the invoice displayed to the user
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public List<ResourceAmount> Items { get; } = new List<ResourceAmount>();

    /// <summary>
    /// The total sum of all resource amounts
    /// </summary>
    public ResourceAmount Total
    {
        get
        {
            var result = new ResourceAmount();

            foreach (ResourceAmount item in this.Items)
            {
                result += item;
            }

            return result;
        }
    }

    /// <summary>
    /// Formalize a report out of the Invoice.
    /// </summary>
    /// <returns></returns>
    public string ToReport()
    {
        var builder = new StringBuilder();

        foreach (var amount in this.Items)
        {
            builder.Append(string.Format(CultureInfo.InvariantCulture, "{0} {1}", amount.Name, amount.ToString()));
            builder.Append(Environment.NewLine);
        }

        builder.Append(Environment.NewLine);
        builder.Append(string.Format(CultureInfo.InvariantCulture, "Total {0}", this.Total.ToString()));

        return builder.ToString();
    }
}
