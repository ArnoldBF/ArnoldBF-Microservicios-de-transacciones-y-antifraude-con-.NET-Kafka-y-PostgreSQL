using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiFraudService.Tests
{
    public class AntiFraudLogic
    {
        public static string GetStatus(decimal value, decimal dailyToltal)
        {
            if (value > 2000 || dailyToltal > 20000)
            {
                return "Rejected";

            }
            return "Approved";
        }
    }
}