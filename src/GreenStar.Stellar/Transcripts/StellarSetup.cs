using System;

using GreenStar.Algorithms;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class StellarSetup : SetupTranscript
{
    private readonly NameGenerator _nameGenerator;
    private readonly string _selectedStellarType;
    private readonly int[] _stellarArgs;

    public StellarSetup(NameGenerator nameGenerator, string selectedStellarType, int[] stellarArgs)
    {
        _nameGenerator = nameGenerator;
        _selectedStellarType = selectedStellarType;
        _stellarArgs = stellarArgs;
    }
    public override void Execute(Context context)
    {
        var t = typeof(GeneratorAlgorithms);

        var method = t.GetMethod(_selectedStellarType) ?? throw new ArgumentException("generator method not found", nameof(_selectedStellarType));
        var args = new object[3 + _stellarArgs.Length];
        args[0] = context.ActorContext;
        args[1] = GeneratorMode.PlanetOnly;
        args[2] = _nameGenerator;
        Array.Copy(_stellarArgs, 0, args, 3, _stellarArgs.Length);
        method.Invoke(null, args);
    }
}