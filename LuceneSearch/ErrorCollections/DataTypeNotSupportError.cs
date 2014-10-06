using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch.ErrorCollections
{
    public class DataTypeNotSupportError : BaseError
    {
        public override string ErrorCode()
        {
            return "1000101003";
        }

        public override string Message
        {
            get
            {
                return "Parameter Error.";
            }
        }
    }
}
