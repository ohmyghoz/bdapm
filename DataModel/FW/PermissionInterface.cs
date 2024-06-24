using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    
    public static class PermissionInterfaceInclude
    {
        public static bool CheckInterfacePermission(List<string> permList, string permissionKodeCsv, DataEntities.PermissionMessageType msg = DataEntities.PermissionMessageType.ThrowInvalidOperationException)
        {
            bool hasAccess = false;
            foreach (string permKode in permissionKodeCsv.Split(','))
            {
                if (permList.Contains(permKode))
                    hasAccess = true;
            }

            if (hasAccess)
                return true;
            else
            {
                if (msg == DataEntities.PermissionMessageType.ThrowInvalidOperationException)
                {
                    string temp = "You don't have permission to access this function.";
                    temp += Environment.NewLine + "Function Code = " + permissionKodeCsv;
                    throw new InvalidOperationException(temp);
                }                
                return false;
            }
        }
    }

}