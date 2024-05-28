using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace YAPCG.Engine.Physics
{
    public readonly struct Mass
    {
        public enum UnitType
        {
            KiloGrams,
            EarthMass,
        }

        private readonly double val;
        private readonly UnitType unit;

        public Mass(double val, UnitType unit)
        {
            this.val = val;
            this.unit = unit;
        }

        public double To(UnitType newUnit)
        {
            double meters = val * KiloGramPerUnit(unit);
            return meters / KiloGramPerUnit(newUnit);
        }

        private double KiloGramPerUnit(UnitType unit) => unit switch
        {
            UnitType.KiloGrams => 1.0,
            UnitType.EarthMass => 5_972_200_000_000_000_000_000_000.0,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

}