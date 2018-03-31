

using System;
using System.Text.RegularExpressions;

namespace Clover.Core.Validate
{
                    public class StringValidator : ValidatorBase<StringValidator, string>
    {
                                                                public StringValidator(string value, string fieldName, Validator validatorObj) : base(value, fieldName, validatorObj)
        {
        }

                                                        public StringValidator IsEmpty(string ErrorMessage)
        {
            SetResult((Value ?? string.Empty).Length != 0, string.Format(ErrorMessage, FieldName), ValidationErrorCode.StringIsEmpty);
            return this;
        }

                                        public StringValidator IsEmpty()
        {
            IsEmpty(ValidatorObj.LookupLanguageString("string_IsEmpty", NegateNextValidationResult));
            return this;
        }

                                                                public StringValidator IsLength(int RequiredLength, string ErrorMessage)
        {
            SetResult(Value.Length != RequiredLength, string.Format(ErrorMessage, FieldName, RequiredLength.ToString()), ValidationErrorCode.StringIsLength);
            return this;
        }

                                                                public StringValidator IsLength(int RequiredLength)
        {
            IsLength(RequiredLength, ValidatorObj.LookupLanguageString("string_IsLength", NegateNextValidationResult));
            return this;
        }

                                                                public StringValidator IsLongerThan(int MinLength, string ErrorMessage)
        {
            SetResult(Value.Length <= MinLength, string.Format(ErrorMessage, FieldName, MinLength.ToString()), ValidationErrorCode.StringIsLongerThan);
            return this;
        }

                                                                public StringValidator IsLongerThan(int MinLength)
        {
            IsLongerThan(MinLength, ValidatorObj.LookupLanguageString("string_IsLongerThan", NegateNextValidationResult));
            return this;
        }

                                                                public StringValidator IsShorterThan(int MaxLength, string ErrorMessage)
        {
            SetResult(Value.Length >= MaxLength, string.Format(ErrorMessage, FieldName, MaxLength.ToString()), ValidationErrorCode.StringIsShorterThan);
            return this;
        }

                                                                public StringValidator IsShorterThan(int MaxLength)
        {
            IsShorterThan(MaxLength, ValidatorObj.LookupLanguageString("string_IsShorterThan", NegateNextValidationResult));
            return this;
        }

                                                                public StringValidator MatchRegex(string RegularExpression, string ErrorMessage)
        {
            Regex Reg = new Regex(RegularExpression);
            SetResult(!Reg.IsMatch(Value), ErrorMessage, ValidationErrorCode.StringMatchRegex);
            return this;
        }

                                                                        public StringValidator MatchRegex(string RegularExpression, RegexOptions regexOptions, string ErrorMessage)
        {
            Regex Reg = new Regex(RegularExpression, regexOptions);
            SetResult(!Reg.IsMatch(Value), ErrorMessage, ValidationErrorCode.StringMatchRegex);
            return this;
        }

                                                                        public StringValidator IsEmail(string ErrorMessage)
        {
            Regex Reg = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+$", RegexOptions.IgnoreCase);
            SetResult(!Reg.IsMatch(Value), string.Format(ErrorMessage, FieldName, Value.ToString()), ValidationErrorCode.StringIsEmail);
            return this;
        }
        
                                                        public StringValidator IsEmail()
        {
            IsEmail(ValidatorObj.LookupLanguageString("string_IsEmail", NegateNextValidationResult));
            return this;
        }

                                                                        public StringValidator IsURL(string ErrorMessage)
        {
            Regex Reg = new Regex(@"^\w+://(?:[\w-]+(?:\:[\w-]+)?\@)?(?:[\w-]+\.)+[\w-]+(?:\:\d+)?[\w- ./?%&=\+]*$", RegexOptions.IgnoreCase);
            SetResult(!Reg.IsMatch(Value), string.Format(ErrorMessage, FieldName, Value.ToString()), ValidationErrorCode.StringIsURL);
            return this;
        }

                                                        public StringValidator IsURL()
        {
            IsURL(ValidatorObj.LookupLanguageString("string_IsURL", NegateNextValidationResult));
            return this;
        }

                                                                        public StringValidator IsDate(string ErrorMessage)
        {
            DateTime Date;
            SetResult(!DateTime.TryParse(Value, out Date), string.Format(ErrorMessage, FieldName, Value.ToString()), ValidationErrorCode.StringIsDate);
            return this;
        }

                                                        public StringValidator IsDate()
        {
            IsDate(ValidatorObj.LookupLanguageString("string_IsDate", NegateNextValidationResult));
            return this;
        }

                                                                        public StringValidator IsInteger(string ErrorMessage)
        {
            int tmp;
            SetResult(!int.TryParse(Value, out tmp), string.Format(ErrorMessage, FieldName, Value.ToString()), ValidationErrorCode.StringIsInteger);
            return this;
        }

                                                        public StringValidator IsInteger()
        {
            IsInteger(ValidatorObj.LookupLanguageString("string_IsInteger", NegateNextValidationResult));
            return this;
        }

                                                                        public StringValidator IsDecimal(string ErrorMessage)
        {
            decimal tmp;
            SetResult(!decimal.TryParse(Value, out tmp), string.Format(ErrorMessage, FieldName, Value.ToString()), ValidationErrorCode.StringIsDecimal);
            return this;
        }

                                                        public StringValidator IsDecimal()
        {
            IsDecimal(ValidatorObj.LookupLanguageString("string_IsDecimal", NegateNextValidationResult));
            return this;
        }

                                                                                                public StringValidator HasALengthBetween(int MinLength, int MaxLength, string ErrorMessage)
        {
            SetResult(Value.Length < MinLength || Value.Length > MaxLength, string.Format(ErrorMessage, FieldName, MinLength, MaxLength), ValidationErrorCode.StringHasALengthBetween);
            return this;
        }

                                                                                public StringValidator HasALengthBetween(int MinLength, int MaxLength)
        {
            HasALengthBetween(MinLength, MaxLength, ValidatorObj.LookupLanguageString("string_HasALengthBetween", NegateNextValidationResult));
            return this;
        }

                                                                                public StringValidator StartsWith(string StartValue, string ErrorMessage)
        {
            SetResult(!Value.StartsWith(StartValue), string.Format(ErrorMessage, FieldName, StartValue), ValidationErrorCode.StringStartsWith);
            return this;
        }

                                                                public StringValidator StartsWith(string StartValue)
        {
            StartsWith(StartValue, ValidatorObj.LookupLanguageString("string_StartsWith", NegateNextValidationResult));
            return this;
        }

                                                                                public StringValidator EndsWith(string EndValue, string ErrorMessage)
        {
            SetResult(!Value.EndsWith(EndValue), string.Format(ErrorMessage, FieldName, EndValue), ValidationErrorCode.StringEndsWith);
            return this;
        }

                                                                public StringValidator EndsWith(string EndValue)
        {
            EndsWith(EndValue, ValidatorObj.LookupLanguageString("string_EndsWith", NegateNextValidationResult));
            return this;
        }

                                                                                public StringValidator Contains(string CompareValue, string ErrorMessage)
        {
            SetResult(!Value.Contains(CompareValue), string.Format(ErrorMessage, FieldName, CompareValue), ValidationErrorCode.StringContains);
            return this;
        }

                                                                public StringValidator Contains(string CompareValue)
        {
            Contains(CompareValue, ValidatorObj.LookupLanguageString("string_Contains", NegateNextValidationResult));
            return this;
        }
    }
}
