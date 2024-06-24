using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.DataModel
{

    public class WSMapper
    {
      
        public static void CopyFieldValues(dynamic from, dynamic onto, string[] includedFields = null, string[] excludedFields = null)
        {
            List<string> splt = null;
            List<string> excl = null;
            if (includedFields != null) splt = includedFields.ToList();
            if (excludedFields != null) excl = excludedFields.ToList();

            var propInfo = from.GetType().GetProperties();
            foreach (var item in propInfo)
            {
                if ((splt == null || splt.Contains(item.Name)) && (excl == null || !excl.Contains(item.Name)))
                {
                    onto.GetType().GetProperty(item.Name).SetValue(onto, item.GetValue(from, null), null);
                }
            }
        }
        public static void CopyFieldValues(dynamic from, dynamic onto, string includedFieldsCsv = null, string excludedFieldsCsv = null)
        {
            string[] splt = null;
            if (includedFieldsCsv != null)
                splt = includedFieldsCsv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


            string[] excl = null;
            if (excludedFieldsCsv != null)
                excl = excludedFieldsCsv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            CopyFieldValues(from, onto, splt, excl);
        }
    }
}
