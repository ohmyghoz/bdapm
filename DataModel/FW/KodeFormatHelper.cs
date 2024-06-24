using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.DataModel
{
    public class KodeFormatHelper
    {
        private DataEntities db;
        private FWKodeFormat xMe;
        public KodeFormatHelper(DataEntities db, string kofID)
        {
            this.db = db;
            xMe = db.FWKodeFormat.Find(kofID);
        }

        public FWKodeCounter GetNewCounter(List<KofParamValue> @params)
        {
            var c = db;
            string kdcnPrefix = "";
            FWKodeCounter kdcn = null;
            foreach (var detail in xMe.FWKodeDetail.Where(x => x.KodTipe == "Parameter" & x.KodParamAsCounter == true).OrderBy(x => x.KodUrut))
            {
                string kode = detail.KodParamKode;
                var find = @params.Where(x => x.Kode == kode);
                if (find.Count() > 0)
                {
                    kdcnPrefix += kode + "=" + find.First().Value + ";";
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot generate. Parameter {0} is not supplied.", detail.KodParamKode));
                }
            }

            var query = from q in xMe.FWKodeCounter where q.KdcnPrefix == kdcnPrefix select q;

            //belum pernah ada counter ini, insert deh
            if (query.Count() == 0)
            {
                kdcn = new FWKodeCounter();
                kdcn.KdcnCounter = xMe.KofStart;
                kdcn.KdcnPrefix = kdcnPrefix;
                kdcn.KdcnLastReset = DateTime.Now;
                kdcn.FWKodeFormat = xMe;
                c.FWKodeCounter.Add(kdcn);
            }
            else
            {
                kdcn = query.First();               
                //cek dulu kapan harus direset berdasarkan terakhir kali reset
                var lastReset = kdcn.KdcnLastReset;
                var nextReset = DateTime.Now;
                var resetTime = xMe.KofResetTime;
                
                if (xMe.KofResetTp == "Daily")
                {
                    nextReset = lastReset.AddDays(xMe.KofResetInterval);
                    nextReset = new DateTime(nextReset.Year, nextReset.Month, nextReset.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);                    
                }
                else if (xMe.KofResetTp == "Monthly")
                {
                    nextReset = lastReset.AddMonths(xMe.KofResetInterval);
                    nextReset = new DateTime(nextReset.Year, nextReset.Month, resetTime.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);
                }
                else if (xMe.KofResetTp == "Yearly")
                {
                    nextReset = lastReset.AddYears(xMe.KofResetInterval);
                    nextReset = new DateTime(nextReset.Year, resetTime.Month, resetTime.Day, resetTime.Hour, resetTime.Minute, resetTime.Second);
                }

                if (DateTime.Now > nextReset)
                {
                    kdcn.KdcnLastReset = DateTime.Now;
                    kdcn.KdcnCounter = xMe.KofStart;
                }
                else
                {
                    kdcn.KdcnCounter += xMe.KofIncrement;
                }
            }
            return kdcn;
        }


        private string ProcessDetail(FWKodeDetail detail, List<KofParamValue> @params, FWKodeCounter kdcn)
        {
            string output = "";
            switch (detail.KodTipe)
            {
                case "Char":
                    output = detail.KodChar;
                    break;
                case "Counter":
                    output = kdcn.KdcnCounter.ToString().PadLeft(xMe.KofCounterLength, "0"[0]);
                    break;
                case "Parameter":
                    var query = from q in @params.Where(x => x.Kode == detail.KodParamKode) select q;
                    if (query.Count() == 0)
                    {
                        throw new InvalidOperationException(string.Format("Cannot generate. Parameter {0} is not supplied.", detail.KodParamKode));
                    }
                    else
                    {
                        output = query.First().Value;
                    }
                    break;
                case "YY":
                    output = string.Format("{0:yy}", DateTime.Now);
                    break;
                case "YYYY":
                    output = string.Format("{0:yyyy}", DateTime.Now);
                    break;
                case "MM":
                    output = string.Format("{0:MM}", DateTime.Now);
                    break;
                case "DD":
                    output = string.Format("{0:dd}", DateTime.Now);
                    break;
                case "MM Roman":
                    int mth = DateTime.Now.Month;
                    switch (mth)
                    {
                        case 1:
                            output = "I";
                            break;
                        case 2:
                            output = "II";
                            break;
                        case 3:
                            output = "III"; break;
                        case 4:
                            output = "IV"; break;
                        case 5:
                            output = "V"; break;
                        case 6:
                            output = "VI"; break;
                        case 7:
                            output = "VII"; break;
                        case 8:
                            output = "VIII"; break;
                        case 9:
                            output = "IX"; break;
                        case 10:
                            output = "X"; break;
                        case 11:
                            output = "XI"; break;
                        case 12:
                            output = "XII"; break;
                    }
                    break;
            }

            if (detail.KodLength.HasValue)
            {
                if (output.Length > detail.KodLength)
                {
                    return output.Substring(0, detail.KodLength.Value);
                }
                else if (output.Length < detail.KodLength)
                {
                    return output.PadRight(detail.KodLength.Value, " "[0]);
                }
            }
            return output;
        }


        public string GetNewKode(List<KofParamValue> @params, bool doSaveChanges = true)
        {
            var c = db;
            var kdcn = GetNewCounter(@params);

            string output = "";
            foreach (var detail in xMe.FWKodeDetail.OrderBy(x => x.KodUrut))
            {
                output += ProcessDetail(detail, @params, kdcn);
            }
            kdcn.KdcnLast = output;
            kdcn.KdcnLastUpdate = DateTime.Now;
            if (doSaveChanges)
            {
                c.SaveChanges();
            }
            return output;
        }
    }

    public class KofParamValue
    {
        public string Kode;

        public string Value;
        /// <summary>
        /// Example Calling : GenerateList(kode1,value1,kode2,value2,...)
        /// </summary>
        /// <param name="paramInCsv"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<KofParamValue> GenerateList(params string[] paramInCsv)
        {
            int pasangCount = paramInCsv.Count() / 2;
            List<KofParamValue> lst = new List<KofParamValue>();
            for (var i = 0; i <= pasangCount - 1; i++)
            {
                lst.Add(new KofParamValue
                {
                    Kode = paramInCsv[i * 2],
                    Value = paramInCsv[i * 2 + 1]
                });
            }
            return lst;
        }
    }
}
