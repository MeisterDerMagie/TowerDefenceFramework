using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_TowerDefense.Steinhauer
{
    public abstract class Consideration
    {
        public float Weight;
        public string ContextKey;
        public abstract float Evaluate(Context context);
    }

    public class ConstantValueConsideration : Consideration {
        private float _constantValue;
        public ConstantValueConsideration(float constantValue)
        {
            _constantValue = constantValue;
        }
        public override float Evaluate(Context _)
        {
            return _constantValue;
        }
    }

    public class CompositeConsideration : Consideration
    {
        public List<Consideration> Considerations;
        public bool AllMustBeNonZero;

        public CompositeConsideration(List<Consideration> considerations, bool allMustBeNonZero)
        {
            Considerations = considerations;
            AllMustBeNonZero = allMustBeNonZero;
        }

        public override float Evaluate(Context context)
        {
            if (Considerations == null || Considerations.Count == 0)
                return 0f;
            float result = Considerations[0].Evaluate(context);
            for (int i = 1; i < Considerations.Count; i++)
            {
                float value = Considerations[i].Evaluate(context);
                if (value < 0f)
                    return 0f;
                else if (value == 0f && AllMustBeNonZero)
                    return 0f;
                result += value;
            }
            return result / Considerations.Count;
        }
    }

    public class FloatBasedConsideration : Consideration
    {
        private float _minValue, _maxValue;
        public float MinValue
        {
            get => _minValue; set => _minValue = value;
        }
        public float MaxValue
        {
            get => _maxValue; set => _maxValue = value;
        }

        private bool _inverted;
        private float _k;
        private CurveType _curveType;

        public FloatBasedConsideration(string contextKey, float weight, float minValue, float maxValue, CurveType curveType, bool inverted, float k)
        {
            ContextKey = contextKey;
            Weight = weight;
            _minValue = minValue;
            _maxValue = maxValue;
            _curveType = curveType;
            _inverted = inverted;
            _k = k;
        }

        public override float Evaluate(Context context)
        {
            if (!context.ConsiderationValues.TryGetValue(ContextKey, out int value))
                return 0.01f;
            double result = EvaluateValueBasedOnCurve((float)value, _minValue, _maxValue, _curveType, _inverted, _k);
            return (float)result * Weight;
        }

        public static float EvaluateValueBasedOnCurve(float x, float min, float max, CurveType curveType, bool inverted, float k = 5f)
        {
            float result = -1;

            if (curveType == CurveType.Linear)
                result = LinearFunction(NormalizeX(x, min, max));
            else if (curveType == CurveType.Exponential)
                result = ExponentialFunction(NormalizeX(x, min, max), k);
            else if (curveType == CurveType.Logarithmic)
                result = LogarithmicFunction(NormalizeX(x, min, max), k);

            if (result > -1 && inverted)
                result = 1 - result;
            return result;
        }

        public static float NormalizeX(float x, float min, float max)
        {
            //if (x < min) x = min;
            //if (x > max) x = max;

            return (x - min) / (max - min);
        }

        public static float LinearFunction(float xNorm)
        {
            return xNorm;
        }

        public static float ExponentialFunction(float xNorm, float k)
        {
            return (float)(1 - Math.Exp(-k * xNorm));
        }

        public static float LogarithmicFunction(float xNorm, float k)
        {
            return (float)(Math.Log(1 + k * xNorm) / Math.Log(1 + k));
        }
    }

    public class BoolBasedConsideration : Consideration
    {
        private int _targetValue;
        private float _valueIfTrue;
        private float _valueIfFalse;

        public BoolBasedConsideration(string contextKey, float weight, int targetValue, float valueIfTrue, float valueIfFalse)
        {
            ContextKey = contextKey;
            Weight = weight;
            _targetValue = targetValue;
            _valueIfTrue = valueIfTrue;
            _valueIfFalse = valueIfFalse;
        }

        public override float Evaluate(Context context)
        {
            if (!context.ConsiderationValues.TryGetValue(ContextKey, out int value))
                return 0.01f;

            if (value == _targetValue)
                return _valueIfTrue * Weight;
            return _valueIfFalse * Weight;
        }
    }

    public class CompareTwoValuesConsideration : Consideration
    {
        private string _valueA;
        private string _valueB;
        private float _valuesAreTheSame;
        private float _aIsBigger;
        private float _bIsBigger;

        public CompareTwoValuesConsideration(float weight, string valueA, string valueB, float valuesAreTheSame, float aIsBigger, float bIsBigger)
        {
            Weight = weight;
            _valueA = valueA;
            _valueB = valueB;
            _valuesAreTheSame = valuesAreTheSame;
            _aIsBigger = aIsBigger;
            _bIsBigger = bIsBigger;
        }

        public override float Evaluate(Context context)
        {
            if (!context.ConsiderationValues.TryGetValue(_valueA, out int valueA))
                return 0.1f;
            if (!context.ConsiderationValues.TryGetValue(_valueB, out int valueB))
                return 0.1f;

            if (valueA == valueB)
                return _valuesAreTheSame * Weight;
            if (valueA > valueB)
                return _aIsBigger * Weight;
            return _bIsBigger * Weight;
        }
    }

    public enum CurveType { Linear, Exponential, Logarithmic };
}
