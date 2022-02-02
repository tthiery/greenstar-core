using System;

using GreenStar.Resources;
using GreenStar.Traits;

namespace GreenStar.Ships.Factory;

public record Blueprint(string ClassName, string ClassType, ResourceAmount Costs, Capable Capable);
