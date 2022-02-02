using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GreenStar.Algorithms;

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
    private static List<string> names;

    /// <summary>
    /// Random number generator
    /// </summary>
    private Random rand = new Random();
    #endregion

    /// <summary>
    /// Generate a name generator
    /// </summary>
    /// <param name="db"></param>
    public NameGenerator(string db)
    {
        if (names == null)
        {
            names = new List<string>();
            Regex ex = new Regex(@".*\((\d*)\) (.*)");
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("GreenStar.Elements.SpaceObjects.Stellar.Generator.MinorPlanet.txt"))
            {
                using (var reader = new StreamReader(s))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        Match m = ex.Match(line);

                        int nr = Convert.ToInt32(m.Groups[1].Value);

                        string name = m.Groups[2].Value;

                        if (nr > 0 && name != null)
                        {
                            names.Add(name);
                        }
                    }
                    reader.Close();
                }
            }
        }
    }

    /// <summary>
    /// Returns a new name. If names are exhausted, start enumerating them.
    /// </summary>
    /// <returns></returns>
    public string GetNext()
    {
        if (names.Count > 0)
        {
            int sel = rand.Next(0, names.Count);

            string res = names[sel];

            names.RemoveAt(sel);

            return res;
        }
        else
        {
            return "X-" + Guid.NewGuid().GetHashCode();
        }
    }
}
