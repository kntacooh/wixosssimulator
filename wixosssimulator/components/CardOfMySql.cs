using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator
{
    /// <summary> WixossSimulatorでMySqlを扱うための静的メソッドを提供します。 </summary>
    public static class CardOfMySql
    {
        /// <summary>
        /// cardテーブルに接続するための ConnectionString を取得します。
        /// </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <returns> cardテーブルに接続するための ConnectionString 。 </returns>
        public static string GetConnectionString(string userId, string password)
        {
            System.Text.StringBuilder connectionString = new System.Text.StringBuilder(64);

            connectionString.Append("server=mysql1.php.xdomain.ne.jp;");
            connectionString.Append("databace=zeta00s_card;");
            connectionString.Append("userid=").Append(userId).Append(";");
            connectionString.Append("password=").Append(password).Append(";");
            //connectionString.Append("persist security info=false;");

            return connectionString.ToString();
        }
    }
}