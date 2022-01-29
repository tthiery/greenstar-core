namespace GreenStar.Research;

public interface ITechnologyDefinitionLoader
{
    Technology[] GetTechnologiesByDefinition(string technologyDefinition);
}
