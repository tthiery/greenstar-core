using System.Collections.Generic;
using GreenStar.Core.Traits;
using GreenStar.Stellar;

namespace GreenStar.Core.Cartography.Builder
{
    public class SolarSystemBuilder
    {
        public IEnumerable<Actor> Create()
        {
            var sun = new Sun();
            sun.Trait<Locatable>().Position = (0, 0);
            yield return sun;

            var mercury = new Planet();
            mercury.Trait<OrbitMovable>().Distance = (long)(0.38709893 * Distance.AstronomicalUnit);
            mercury.Trait<OrbitMovable>().CurrentDegree = 0;
            mercury.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 0.2408467);
            yield return mercury;

            var venus = new Planet();
            venus.Trait<OrbitMovable>().Distance = (long)(0.72333199 * Distance.AstronomicalUnit);
            venus.Trait<OrbitMovable>().CurrentDegree = 0;
            venus.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 0.61519726);
            yield return venus;

            var earth = new Planet();
            earth.Trait<OrbitMovable>().Distance = (long)(1.00000011 * Distance.AstronomicalUnit);
            earth.Trait<OrbitMovable>().CurrentDegree = 0;
            earth.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 1.0000174);
            yield return earth;

            var mars = new Planet();
            mars.Trait<OrbitMovable>().Distance = (long)(1.52366231 * Distance.AstronomicalUnit);
            mars.Trait<OrbitMovable>().CurrentDegree = 0;
            mars.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 1.8808476);
            yield return mars;

            var jupiter = new Planet();
            jupiter.Trait<OrbitMovable>().Distance = (long)(5.20336301 * Distance.AstronomicalUnit);
            jupiter.Trait<OrbitMovable>().CurrentDegree = 0;
            jupiter.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 11.862615);
            yield return jupiter;

            var saturn = new Planet();
            saturn.Trait<OrbitMovable>().Distance = (long)(9.53707032 * Distance.AstronomicalUnit);
            saturn.Trait<OrbitMovable>().CurrentDegree = 0;
            saturn.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 29.447498);
            yield return saturn;

            var uranus = new Planet();
            uranus.Trait<OrbitMovable>().Distance = (long)(19.19126393 * Distance.AstronomicalUnit);
            uranus.Trait<OrbitMovable>().CurrentDegree = 0;
            uranus.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 84.016846);
            yield return uranus;

            var neptune = new Planet();
            neptune.Trait<OrbitMovable>().Distance = (long)(30.06896348 * Distance.AstronomicalUnit);
            neptune.Trait<OrbitMovable>().CurrentDegree = 0;
            neptune.Trait<OrbitMovable>().SpeedDegree = (int)(360 / 164.79132);
            yield return neptune;
        }
    }
}