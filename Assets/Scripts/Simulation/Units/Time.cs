using System;

namespace YAPCG.Simulation.Units
{
    public readonly struct Time
    {
        public static Time Zero() => new Time(0, UnitType.Seconds);

        public enum UnitType
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Weeks,
            Years
        }

        private readonly double val;
        private readonly UnitType unit;
        
        public Time(double val, UnitType unit)
        {
            this.val = val;
            this.unit = unit;
        }
        
        public double To(UnitType newUnit)
        {
            double meters = val * SecondsPerUnit(unit);
            return meters / SecondsPerUnit(newUnit);
        }

        public static implicit operator bool(Time time) => time.val != 0;

        private double SecondsPerUnit(UnitType unit) => unit switch
        {
            UnitType.Seconds => 1,
            UnitType.Minutes => 60,
            UnitType.Hours => 3600,
            UnitType.Days => 86400,
            UnitType.Weeks => 86400 * 7,
            UnitType.Years => 86400 * 365.2425,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

}