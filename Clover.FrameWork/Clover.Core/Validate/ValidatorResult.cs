

namespace Clover.Core.Validate
{
    
    
    
    
    public class ValidatorResult
    {
        
        
        
        
        
        
        
        
        public ValidatorResult(string ErrorMessage, string fieldName, ValidatorResultLevel level, int errorCode)
        {
            ValidationMessage = ErrorMessage;
            FieldName = fieldName;
            Level = level;
            ErrorCode = errorCode;
        }

        
        
        
        public string ValidationMessage { get; private set; }

        
        
        
        public string FieldName { get; private set; }

        
        
        
        public ValidatorResultLevel Level { get; private set; }

        
        
        
        public int ErrorCode { get; private set; }
    }
}