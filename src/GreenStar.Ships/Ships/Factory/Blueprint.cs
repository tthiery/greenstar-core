using System;

using GreenStar.Core.Resources;
using GreenStar.Core.Traits;

namespace GreenStar.Ships.Factory;

public record Blueprint(string ClassName, string ClassType, ResourceAmount Costs, Capable Capable);
