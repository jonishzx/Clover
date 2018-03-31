

using System;

namespace Clover.Core.Validate
{
    
    
    
    public enum ValidationLanguageEnum
    {
        
        
        
        [LanguageResourceFile("TNValidate.Languages.Languages-en.xml")]
        English = 0,
        
        
        
        
        [LanguageResourceFile("TNValidate.Languages.Languages-sv.xml")]
        Swedish = 1,

        
        
        
        [LanguageResourceFile("TNValidate.Languages.Languages-pt-BR.xml")]
        Portuguese = 2
    }

    
    
    
    
    public enum ErrorMode
    {
        
        
        
        OneErrorPerField,

        
        
        
        AllErrors
    }

    
    
    
    public enum ValidatorResultLevel
    {
        Error,
        Warning
    }

    
    
    
    internal class LanguageResourceFile : Attribute
    {
        
        
        
        
        public LanguageResourceFile(string resourceFile)
        {
            ResourceFile = resourceFile;
        }
        
        
        
        
        public string ResourceFile { get; private set; }
    }
}
