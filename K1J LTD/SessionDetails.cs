using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K1J_LTD
{
    internal class SessionDetails
    {
        public static string accountName;
        public static string accountID;
        public static List<string[]> basket = new List<string[]>();
        public static MySqlConnection Connection;
    }
}
