using System;

namespace CompanyWebApi.Persistence.Repositories.Base.Exceptions
{
    public class NullDataException : Exception
    {
        public NullDataException() : base("Could not get any data.")
        {

        }
    }
}
