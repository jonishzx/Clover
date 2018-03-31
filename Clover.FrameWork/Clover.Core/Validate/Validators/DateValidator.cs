

using System;
using System.Text.RegularExpressions;

namespace Clover.Core.Validate
{
    
    
    
    
    public class DateValidator : ValidatorBase<DateValidator, DateTime>
    {
        
        
        
        
        
        
        
        public DateValidator(DateTime value, string fieldName, Validator validatorObj) : base(value, fieldName, validatorObj)
        {
        }

        
        
        
        
        
        
        
        public DateValidator IsNotAFutureDate(string errorMessage)
        {
            SetResult(Value > DateTime.Now, string.Format(errorMessage, FieldName), ValidationErrorCode.DateIsNotAFutureDate);
            return this;
        }

        
        
        
        
        
        public DateValidator IsNotAFutureDate()
        {
            IsNotAFutureDate(ValidatorObj.LookupLanguageString("date_IsNotAFutureDate", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public DateValidator IsNotAPastDate(string errorMessage)
        {
            SetResult(Value < DateTime.Now, string.Format(errorMessage, FieldName), ValidationErrorCode.DateIsNotAPastDate);
            return this;
        }

        
        
        
        
        
        public DateValidator IsNotAPastDate()
        {
            IsNotAPastDate(ValidatorObj.LookupLanguageString("date_IsNotAPastDate", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        public DateValidator IsNotMinMaxValue(string ErrorMessage)
        {
            SetResult((Value == DateTime.MinValue || Value == DateTime.MaxValue), string.Format(ErrorMessage, FieldName), ValidationErrorCode.DateIsNotMinMaxValue);
            return this;
        }

        
        
        
        
        public DateValidator IsNotMinMaxValue()
        {
            IsNotMinMaxValue(ValidatorObj.LookupLanguageString("date_IsNotMinMaxValue", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public DateValidator IsEarlierThan(DateTime CheckDateValue, string ErrorMessage)
        {
            SetResult(Value >= CheckDateValue, string.Format(ErrorMessage, FieldName, CheckDateValue), ValidationErrorCode.DateIsEarlierThan);
            return this;
        }

        
        
        
        
        
        public DateValidator IsEarlierThan(DateTime CheckDateValue)
        {
            IsEarlierThan(CheckDateValue, ValidatorObj.LookupLanguageString("date_IsEarlierThan", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        
        public DateValidator IsLaterThan(DateTime CheckDateValue, string ErrorMessage)
        {
            SetResult(Value <= CheckDateValue, string.Format(ErrorMessage, FieldName, CheckDateValue), ValidationErrorCode.DateIsLaterThan);
            return this;
        }

        
        
        
        
        
        public DateValidator IsLaterThan(DateTime CheckDateValue)
        {
            IsLaterThan(CheckDateValue, ValidatorObj.LookupLanguageString("date_IsLaterThan", NegateNextValidationResult));
            return this;
        }
    }
}