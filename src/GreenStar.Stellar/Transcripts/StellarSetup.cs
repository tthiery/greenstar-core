using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Cartography.Builder;
using GreenStar.TurnEngine;

using Microsoft.Extensions.DependencyInjection;

namespace GreenStar.Transcripts;

public class StellarSetup : SetupTranscript
{
    private readonly NameGenerator _nameGenerator;
    private readonly IServiceProvider _serviceProvider;
    private readonly StellarType _selectedStellarType;

    public StellarSetup(NameGenerator nameGenerator, IServiceProvider serviceProvider, StellarType selectedStellarType)
    {
        _nameGenerator = nameGenerator;
        _serviceProvider = serviceProvider;
        _selectedStellarType = selectedStellarType;
    }
    public override Task ExecuteAsync(Context context)
    {
        var t = Type.GetType($"GreenStar.Cartography.Builder.{_selectedStellarType.Name}StellarGenerator") ?? throw new InvalidOperationException("cannot find stellar generator type");

        var generator = ActivatorUtilities.CreateInstance(_serviceProvider, t) as IStellarGenerator ?? throw new InvalidOperationException("fail to instantiate stellar generator type");

        generator.Generate(context.ActorContext, GeneratorMode.PlanetOnly, _selectedStellarType.Arguments);

        return Task.CompletedTask;
    }

    public static StellarType[] FindAllStellarTypes()
    {
        var result = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => t.Name.EndsWith("StellarGenerator") && !t.Name.StartsWith("IStellarGenerator"))
                        .Select(t => new StellarType(
                            t.Name.Substring(0, t.Name.IndexOf("StellarGenerator")),
                            t.Name.Substring(0, t.Name.IndexOf("StellarGenerator")),
                            t.GetMethod("Generate")
                                ?.GetCustomAttributes<StellarGeneratorArgumentAttribute>()
                                .Select(a => new StellarGeneratorArgument(a.Name, a.DisplayName, a.Value))
                                .ToArray() ?? Array.Empty<StellarGeneratorArgument>()
                        ))
                        .ToArray();

        return result;
    }
}