

using System;

namespace Clover.Core.Validate
{
    
    
    
    
    public class BoolValidator : ValidatorBase<BoolValidator, bool>
    {
        
        
        
        
        
        
        
        public BoolValidator(bool value, string fieldName, Validator validatorObj)
            : base(value, fieldName, validatorObj)
        {
        }

        
        
        
        
        
        
        public BoolValidator IsTrue(string ErrorMessage)
        {
            SetResult(!Value, string.Format(ErrorMessage, FieldName), ValidationErrorCode.BoolIsNotTrue);
            return this;
        }

        
        
        
        
        public BoolValidator IsTrue()
        {
            IsTrue(ValidatorObj.LookupLanguageString("bool_IsTrue", NegateNextValidationResult));
            return this;
        }

        
        
        
        
        
        
        public BoolValidator IsFalse(string ErrorMessage)
        {
            SetResult(Value, string.Format(ErrorMessage, FieldName), ValidationErrorCode.BoolIsNotFalse);
            return this;
        }

        
        
        
        
        public BoolValidator IsFalse()
        {
            IsFalse(ValidatorObj.LookupLanguageString("bool_IsFalse", NegateNextValidationResult));
            return this;
        }
    }
}
