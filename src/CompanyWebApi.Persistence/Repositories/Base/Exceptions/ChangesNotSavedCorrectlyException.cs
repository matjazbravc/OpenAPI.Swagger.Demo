using System;

namespace CompanyWebApi.Persistence.Repositories.Base.Exceptions
{
    public class ChangesNotSavedCorrectlyException : Exception
    {
        public ChangesNotSavedCorrectlyException(Type entity) : base($"Could not saved changes correctly in {entity} entity")
        {

        }
    }
}
