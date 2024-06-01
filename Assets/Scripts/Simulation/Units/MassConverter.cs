using System;

namespace YAPCG.Simulation.Units
{
    public readonly struct MassConverter
    {
        public enum UnitType
        {
            KiloGrams,
            EarthMass,
        }

        private readonly float _value;
        private readonly UnitType _unit;

        public MassConverter(float value, UnitType unit)
        {
            this._value = value;
            this._unit = unit;
        }

        public float To(UnitType newUnit)
        {
            float meters = _value * KiloGramPerUnit(_unit);
            return meters / KiloGramPerUnit(newUnit);
        }

        private float KiloGramPerUnit(UnitType unit) => unit switch
        {
            UnitType.KiloGrams => 1.0f,
            UnitType.EarthMass => 5_972_200_000_000_000_000_000_000.0f,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

}