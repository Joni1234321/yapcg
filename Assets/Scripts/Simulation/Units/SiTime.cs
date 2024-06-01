using System;

namespace YAPCG.Simulation.Units
{

    public struct Mass
    {
        public float EarthMass;
        public Mass(float earthMass) => EarthMass = earthMass;
    }
    public struct SiTime
    {
        public float Days;
        public SiTime(float seconds) => Days = seconds / 86400f;
        
    }
    
    public struct TimeConverter
    {
        public static TimeConverter Zero() => new TimeConverter(0, UnitType.Seconds);

        public enum UnitType
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Weeks,
            Years
        }

        private float val;
        private UnitType unit;
        
        public TimeConverter(float val, UnitType unit)
        {
            this.val = val;
            this.unit = unit;
        }
        
        public float To(UnitType newUnit)
        {
            float meters = val * SecondsPerUnit(unit);
            return meters / SecondsPerUnit(newUnit);
        }

        public static implicit operator bool(TimeConverter timeConverter) => timeConverter.val != 0;

        private float SecondsPerUnit(UnitType unit) => unit switch
        {
            UnitType.Seconds => 1,
            UnitType.Minutes => 60,
            UnitType.Hours => 3600,
            UnitType.Days => 86400,
            UnitType.Weeks => 86400 * 7,
            UnitType.Years => 86400 * 365.2425f,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

}