

namespace Clover.Core.Validate
{
    
    
    
   public static class ValidationErrorCode
   {
      public const int StringIsEmpty = 101;
      public const int StringIsLength = 102;
      public const int StringIsLongerThan = 103;
      public const int StringIsShorterThan = 104;
      public const int StringMatchRegex = 105;
      public const int StringIsEmail = 106;
      public const int StringIsURL = 107;
      public const int StringIsDate = 108;
      public const int StringIsInteger = 109;
      public const int StringIsDecimal = 110;
      public const int StringHasALengthBetween = 111;
      public const int StringStartsWith = 112;
      public const int StringEndsWith = 113;
      public const int StringContains = 114;

      public const int NumericIsLessThanOrEqual = 201;
      public const int NumericIsGreaterThanOrEqual = 202;
      public const int NumericIsGreaterThan = 203;
      public const int NumericIsLessThan = 204;
      public const int NumericEquals = 205;
      public const int NumericBetween = 206;
      public const int NumericIsZero = 207;

      public const int DateIsNotAFutureDate = 301;
      public const int DateIsNotAPastDate = 302;
      public const int DateIsNotMinMaxValue = 303;
      public const int DateIsEarlierThan = 304;
      public const int DateIsLaterThan = 305;

      public const int BoolIsNotTrue = 401;
      public const int BoolIsNotFalse = 402;
   }
}
﻿