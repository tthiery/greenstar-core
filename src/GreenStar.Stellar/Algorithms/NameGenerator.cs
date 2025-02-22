using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Microsoft.Extensions.FileProviders;

namespace GreenStar.Algorithms;

public record NameItem(string Name, string Glory);

/// <summary>
/// 
/// MinorPlanet: List of planet names
/// List from http://www.ipa.nw.ru/PAGE/DEPFUND/LSBSS/englenam.htm
/// or http://cfa-www.harvard.edu/iau/lists/MPNames.html
/// </summary>
public class NameGenerator
{
    #region Fields
    /// <summary>
    /// List of all names
    /// </summary>
    private readonly Dictionary<string, List<NameItem>> _names = new();

    /// <summary>
    /// Random number generator
    /// </summary>
    private readonly Random rand = new Random();
    #endregion

    /// <summary>
    /// Generate a name generator
    /// </summary>
    /// /// <param name="db"></param>
    public NameGenerator Load(string category, IFileProvider fileProvider, string fileName)
    {
        using var fileStream = fileProvider.GetFileInfo(fileName).CreateReadStream();

        var names = JsonSerializer.Deserialize<NameItem[]>(fileStream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        if (names is not null)
        {
            _names.Add(category, new List<NameItem>(names));
        }

        fileStream.Close();

        return this;
    }

    /// <summary>
    /// Returns a new name. If names are exhausted, start enumerating them.
    /// </summary>
    /// <returns></returns>
    public NameItem GetNext(string category)
    {
        if (_names.TryGetValue(category, out var names) && names.Count > 0)
        {
            int sel = rand.Next(0, names.Count);

            var res = names[sel];

            names.RemoveAt(sel);

            return res;
        }
        else
        {
            return new NameItem("X-" + Guid.NewGuid().GetHashCode(), "Random");
        }
    }
}
