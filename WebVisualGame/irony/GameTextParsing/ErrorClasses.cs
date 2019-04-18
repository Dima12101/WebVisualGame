using System;
using System.Collections.Generic;
using System.Text;

namespace GameTextParsing
{
    // this class for user's error in source code
    public class SourceCodeError : ApplicationException
    {
        public SourceCodeError(string message) : base(message)
        {

        }

        public SourceCodeError()
        {

        }
    }

    // this type of exception for errors in business logic
    public class BusinessLogicError : ApplicationException
    {
        public BusinessLogicError(string message) : base(message)
        {

        }

        public BusinessLogicError()
        {

        }
    }
}
