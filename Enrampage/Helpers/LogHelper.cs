using Enrampage.Models;
using System;

namespace Enrampage.Helpers
{
    public static class LogHelper
    {
        public static void Log(Exception Ex)
        {
            using (var context = new EnrampageEntities())
            {
                context.Logs.Add(new Log { Exception = Ex.ToString() });
                context.SaveChanges();
            }
        }
    }
}