

using System;
using System.Text.RegularExpressions;

namespace Clover.Core.Validate
{
    
    
    
    
    
    
    
    public abstract class ValidatorBase<TValidator, TValue> where TValidator : ValidatorBase<TValidator, TValue>
    {
        
        
        
        
        
        
        
        protected ValidatorBase(TValue value, string fieldName, Validator validatorObj)
        {
            Value = value;
            FieldName = fieldName;
            ValidatorObj = validatorObj;
        }

        
        
        
        
        public TValue Value { get; set; }

        
        
        
        public string FieldName { get; private set; }

        
        
        
        protected Validator ValidatorObj { get; set; }

        
        
        
        protected bool NegateNextValidationResult { get; set; }

        
        
        
        protected ValidatorResultLevel NextFailureResultLevel = ValidatorResultLevel.Error;

        
        
        
        protected int? NextErrorCode = null;

        
        
        
        
        
        
        public void SetResult(bool Result, string ErrorMsg)
        {
            SetResult(Result, ErrorMsg, 0);
        }

        
        
        
        
        
        
        
        public void SetResult(bool Result, string ErrorMsg, int ErrorCode)
        {
            if (Result ^ NegateNextValidationResult)
                ValidatorObj.AddValidationError(ErrorMsg, FieldName, NextFailureResultLevel,
                    NextErrorCode ?? ErrorCode);

            
            NegateNextValidationResult = false;
            NextFailureResultLevel = ValidatorResultLevel.Error;
            NextErrorCode = null;
        }

        
        
        
        
        
        
        
        
        
        public TValidator Is(Predicate<TValue> Predicate, string ErrorMessage)
        {
            SetResult(!Predicate(Value), ErrorMessage);
            return (TValidator)this;
        }

        
        
        
        
        
        public TValidator Not()
        {
            NegateNextValidationResult = true;
            return (TValidator)this;
        }

        
        
        
        
        
        
        public TValidator WarnUnless()
        {
            NextFailureResultLevel = ValidatorResultLevel.Warning;
            return (TValidator)this;
        }

        
        
        
        
        
        public TValidator WithErrorCode(int code)
        {
           NextErrorCode = code;
           return (TValidator)this;
        }
    }
}
