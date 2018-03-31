

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml.Linq;

namespace Clover.Core.Validate
{
    
    
    
    
    
    public class Validator
    {
        
        
        
        private readonly ValidationLanguageEnum ValidationLanguage;

        
        
        
        private readonly IList<ValidatorResult> validatorResults = new List<ValidatorResult>();

        
        
        
        
        
        public Validator()
        {
            Mode = ErrorMode.OneErrorPerField;
            ValidationLanguage = ValidationLanguageEnum.English;
        }

        
        
        
        
        
        
        public Validator(ValidationLanguageEnum validationLanguage)
        {
            ValidationLanguage = validationLanguage;
            Mode = ErrorMode.OneErrorPerField;
        }


        
        
        
        
        
        
        
        public Validator(ValidationLanguageEnum validationLanguage, ErrorMode mode)
        {
            ValidationLanguage = validationLanguage;
            Mode = mode;
        }

        
        
        
        
        public IList<ValidatorResult> ValidatorResults
        {
            get { return validatorResults; }
        }

        
        
        
        public ErrorMode Mode { get; set; }

        
        
        
        
        public void Clear()
        {
            ValidatorResults.Clear();
        }

        
        
        
        
        
        public int ErrorCount()
        {
            return ValidatorResults.Where(x => x.Level == ValidatorResultLevel.Error).Count();
        }

        
        
        
        
        
        public int WarningCount()
        {
            return ValidatorResults.Where(x => x.Level == ValidatorResultLevel.Warning).Count();
        }

        
        
        
        
        
        public bool HasErrors()
        {
            return ErrorCount() != 0;
        }

        
        
        
        
        
        public bool HasWarnings()
        {
            return WarningCount() != 0;
        }

        
        
        
        
        
        
        
        public BoolValidator That(bool value, string fieldName)
        {
            return new BoolValidator(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<int> That(int value, string fieldName)
        {
            return new NumericValidator<int>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<uint> That(uint value, string fieldName)
        {
            return new NumericValidator<uint>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<short> That(short value, string fieldName)
        {
            return new NumericValidator<short>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<ushort> That(ushort value, string fieldName)
        {
            return new NumericValidator<ushort>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<long> That(long value, string fieldName)
        {
            return new NumericValidator<long>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<ulong> That(ulong value, string fieldName)
        {
            return new NumericValidator<ulong>(value, fieldName, this);
        }
        
        
        
        
        
        
        
        
        public NumericValidator<byte> That(byte value, string fieldName)
        {
            return new NumericValidator<byte>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<sbyte> That(sbyte value, string fieldName)
        {
            return new NumericValidator<sbyte>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<decimal> That(decimal value, string fieldName)
        {
            return new NumericValidator<decimal>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<float> That(float value, string fieldName)
        {
            return new NumericValidator<float>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public NumericValidator<double> That(double value, string fieldName)
        {
            return new NumericValidator<double>(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public StringValidator That(string value, string fieldName)
        {
            return new StringValidator(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public DateValidator That(DateTime value, string fieldName)
        {
            return new DateValidator(value, fieldName, this);
        }

        
        
        
        
        
        
        
        public void AddValidationError(string Message, string FieldName, int errorCode)
        {
            AddValidationError(Message, FieldName, ValidatorResultLevel.Error, errorCode);
        }

        
        
        
        
        
        
        
        
        public void AddValidationError(string Message, string FieldName, ValidatorResultLevel Level, int errorCode)
        {
            
            if (Mode == ErrorMode.OneErrorPerField)
            {
                
                foreach (var Error in ValidatorResults)
                    if (Error.FieldName == FieldName)
                        return;
            }

            
            ValidatorResults.Add(new ValidatorResult(Message, FieldName, Level, errorCode));
        }


        
        
        
        
        
        
        
        internal string LookupLanguageString(string KeyName, bool Negated)
        {
            return LangCache.FetchItem(ValidationLanguage, Negated ? "not_" + KeyName : KeyName);
        }
    }
}
