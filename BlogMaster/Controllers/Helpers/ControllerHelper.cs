using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogMaster.Controllers.Helpers
{
    public class ControllerHelper
    {
        internal static Dictionary<string, List<string>> GetErrors(ModelStateDictionary state)
        {
            Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();

            if (state.ErrorCount > 0)
            {
                foreach (var entry in state)
                {
                    List<string> errors = entry.Value.Errors.Select(x => x.ErrorMessage).ToList();

                    Errors.Add(entry.Key, errors);
                }
            }


            return Errors;


        }
    }
}
