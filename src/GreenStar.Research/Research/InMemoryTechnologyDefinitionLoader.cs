using System;

namespace GreenStar.Research;

public class InMemoryTechnologyDefinitionLoader : ITechnologyDefinitionLoader
{
    private readonly Func<Technology[]> _techFactory;

    public InMemoryTechnologyDefinitionLoader(Func<Technology[]> techFactory)
    {
        _techFactory = techFactory;
    }

    public Technology[] GetTechnologiesByDefinition(string technologyDefinition)
        => _techFactory();
}