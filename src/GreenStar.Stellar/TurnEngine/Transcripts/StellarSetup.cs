using System;

using GreenStar.Algorithms;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class StellarSetup : SetupTranscript
{
    private readonly string _selectedStellarType;
    private readonly int[] _stellarArgs;

    public StellarSetup(string selectedStellarType, int[] stellarArgs)
    {
        _selectedStellarType = selectedStellarType;
        _stellarArgs = stellarArgs;
    }
    public override void Execute(Context context)
    {
        var t = Type.GetType("GreenStar.Algorithms.GeneratorAlgorithms, GreenStar.Stellar") ?? throw new InvalidOperationException("failed to find stellarstrategies");

        var method = t.GetMethod(_selectedStellarType) ?? throw new ArgumentException("generator method not found", nameof(_selectedStellarType));
        var args = new object[2 + _stellarArgs.Length];
        args[0] = context.ActorContext;
        args[1] = GeneratorMode.PlanetOnly;
        Array.Copy(_stellarArgs, 0, args, 2, _stellarArgs.Length);
        method.Invoke(null, args);
    }
}