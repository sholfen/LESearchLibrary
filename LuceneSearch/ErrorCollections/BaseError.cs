using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearch.ErrorCollections
{
    public abstract class BaseError : Exception
    {
        public abstract string ErrorCode();
    }
}
