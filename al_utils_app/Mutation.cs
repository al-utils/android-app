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
            query = $@"mutation ($listEntryId: Int) {{
  SaveMediaListEntry(id:$listEntryId,mediaId:1,status:CURRENT) {{
    id
    status
  }}
}}";
            Response r = await Request.RequestDataAsync(query, new Dictionary<string, object>());
            //if (r != null)
            //{
            //    Debug.WriteLine(r);
            //    var isDeleted = r.Data.DeleteActivity.Deleted;
            //    if (isDeleted)
            //        return true;
            //    return false;
            //}
            //else
            //{
            //    Debug.WriteLine("Error: Activity not deleted");
            //    return false;
            //}
            return false;
        }

    }
}
