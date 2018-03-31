

using System;

namespace Clover.Core.Validate
{
    
    
    
    
    
    
    public class NumericValidator<TValue> : ValidatorBase<NumericValidator<TValue>, TValue> where TValue : struct, IComparable<TValue>, IEquatable<TValue>
    {
        
        
        
        
        
        
        
        public NumericValidator(TValue value, string fieldName, Validator validatorObj) : base(value, fieldName, validatorObj)
        {
        }

        
        
        
        
        
        
        
        public NumericValidator<TValue> IsLessThanOrEqual(TValue lessThanValue, string ErrorMessage)
        {
            SetResult(Value.CompareTo(lessThanValue) > 0, string.Format(ErrorMessage, FieldName, lessThanValue.ToString()), ValidationErrorCode.NumericIsLessThanOrEqual);
            return this;
        }

        
        
        
        
        
        public NumericValidator<TValue> IsLessThanOrEqual(TValue lessThanValue)
        {
            IsLessThanOrEqual(lessThanValue, ValidatorObj.LookupLanguageString("int_IsLessThanOrEqual", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public NumericValidator<TValue> IsGreaterThanOrEqual(TValue GreaterThanValue, string ErrorMessage)
        {
            SetResult(Value.CompareTo(GreaterThanValue) < 0, string.Format(ErrorMessage, FieldName, GreaterThanValue.ToString()), ValidationErrorCode.NumericIsGreaterThanOrEqual);
            return this;
        }

        
        
        
        
        
        public NumericValidator<TValue> IsGreaterThanOrEqual(TValue GreaterThanValue)
        {
            IsGreaterThanOrEqual(GreaterThanValue, ValidatorObj.LookupLanguageString("int_IsGreaterThanOrEqual", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public NumericValidator<TValue> IsGreaterThan(TValue GreaterThanValue, string ErrorMessage)
        {
            SetResult(Value.CompareTo(GreaterThanValue) <= 0, string.Format(ErrorMessage, FieldName, GreaterThanValue.ToString()), ValidationErrorCode.NumericIsGreaterThan);
            return this;
        }       
        
        
        
        
        
        
        public NumericValidator<TValue> IsGreaterThan(TValue GreaterThanValue)
        {
            IsGreaterThan(GreaterThanValue, ValidatorObj.LookupLanguageString("int_IsGreaterThan", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public NumericValidator<TValue> IsLessThan(TValue LessThanValue, string ErrorMessage)
        {
            SetResult(Value.CompareTo(LessThanValue) >= 0, string.Format(ErrorMessage, FieldName, LessThanValue.ToString()), ValidationErrorCode.NumericIsLessThan);
            return this;
        }

        
        
        
        
        
        public NumericValidator<TValue> IsLessThan(TValue LessThanValue)
        {
            IsLessThan(LessThanValue, ValidatorObj.LookupLanguageString("int_IsLessThan", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public NumericValidator<TValue> Equals(TValue EqualValue, string ErrorMessage)
        {
            SetResult(!Value.Equals(EqualValue), string.Format(ErrorMessage, FieldName, EqualValue.ToString()), ValidationErrorCode.NumericEquals);
            return this;
        }

        
        
        
        
        
        public NumericValidator<TValue> Equals(TValue EqualValue)
        {
            Equals(EqualValue, ValidatorObj.LookupLanguageString("int_Equals", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        
        public NumericValidator<TValue> Between(TValue StartValue, TValue EndValue, string ErrorMessage)
        {
            SetResult((Value.CompareTo(StartValue) < 0 || Value.CompareTo(EndValue) > 0), string.Format(ErrorMessage, FieldName, StartValue.ToString(), EndValue.ToString()), ValidationErrorCode.NumericBetween);
            return this;
        }        
        
        
        
        
        
        
        
        public NumericValidator<TValue> Between(TValue StartValue, TValue EndValue)
        {
            Between(StartValue, EndValue, ValidatorObj.LookupLanguageString("int_Between", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        public NumericValidator<TValue> IsZero(string ErrorMessage)
        {
            SetResult(!Value.Equals(new TValue()), string.Format(ErrorMessage, FieldName), ValidationErrorCode.NumericIsZero);
            return this;
        }
        
        
        
        
        
        public NumericValidator<TValue> IsZero()
        {
            IsZero(ValidatorObj.LookupLanguageString("int_IsZero", NegateNextValidationResult));
            return this;
        }
    }
}