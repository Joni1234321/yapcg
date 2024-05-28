using System;

namespace YAPCG.Simulation.Units
{
    public readonly struct Length
    {
        public static readonly Length ZERO = new Length(0, UnitType.Meters);

        public enum UnitType
        {
            Meters,
            KiloMeters,
            AstronomicalUnits
        }

        private readonly double val;
        private readonly UnitType unit;

        public Length(double val, UnitType unit)
        {
            this.val = val;
            this.unit = unit;
        }

        public double To(UnitType newUnit)
        {
            double meters = val * MeterPerUnit(unit);
            return meters / MeterPerUnit(newUnit);
        }

        private double MeterPerUnit(UnitType unit) => unit switch
        {
            UnitType.Meters => 1,
            UnitType.KiloMeters => 1000,
            UnitType.AstronomicalUnits => 149_597_870_691,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        public static Length operator +(Length a, Length b)
        {
            return new Length(a.To(UnitType.Meters) + b.To(UnitType.Meters), UnitType.Meters);
        }

        public static Length operator /(Length a, double b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException();
            }

            return new Length(a.To(UnitType.Meters) / b, UnitType.Meters);
        }
    }
}