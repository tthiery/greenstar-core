using GreenStar.Persistence;

namespace GreenStar.Traits;

public class IdealConditions : Trait
{
    public double IdealTemperature { get; set; }
    public double IdealGravity { get; set; }

    public override void Load(IPersistenceReader reader)
    {
        IdealTemperature = reader.Read<double>(nameof(IdealTemperature));
        IdealGravity = reader.Read<double>(nameof(IdealGravity));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write<double>(nameof(IdealTemperature), IdealTemperature);
        writer.Write<double>(nameof(IdealGravity), IdealGravity);
    }
}