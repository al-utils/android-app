using al_utils_app.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace al_utils_app
{
    internal class Mutation
    {
        public async static Task<bool> DeleteActivity(Activity activity)
        {
            var query = $@"mutation {{
  DeleteActivity(id:{activity.Id}) {{
    deleted
  }}
}}";
            Response r = await Request.RequestDataAsync(query, new Dictionary<string, object>());
            if (r != null)
            {
                var isDeleted = r.Data.DeleteActivity.Deleted;
                if (isDeleted)
                    return true;
                return false;
            }
            else
            {
                Debug.WriteLine("Error: Activity not deleted");
                return false;
            }
        }

    }
}
