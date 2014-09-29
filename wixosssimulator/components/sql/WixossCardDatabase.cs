using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WixossSimulator.Sql
{
    /// <summary> WixossCard データベースを扱うための静的メソッドを提供します。 </summary>
    public static class WixossCardDatabase
    {
        /// <summary>
        /// SQL Serverで WixossCard データベースに接続するための ConnectionString を取得します。
        /// </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <returns> cardテーブルに接続するための ConnectionString 。 </returns>
        public static string CreateConnectionString(string userId, string password)
        {
            System.Text.StringBuilder connectionString = new System.Text.StringBuilder(64);

            connectionString.Append("workstation id=WixossCard.mssql.somee.com;");
            connectionString.Append("packet size=4096;");
            connectionString.Append("user id=").Append(userId).Append(";");
            connectionString.Append("pwd=").Append(password).Append(";");
            connectionString.Append("data source=WixossCard.mssql.somee.com;");
            connectionString.Append("persist security info=False;");
            connectionString.Append("initial catalog=WixossCard;");

            return connectionString.ToString();
        }

        /// <summary>
        /// MySQLでのcardデータベースに接続するための ConnectionString を取得します。
        /// </summary>
        /// <param name="userId"> 接続で使用するユーザーのID。 </param>
        /// <param name="password"> 接続で使用するユーザーのパスワード。 </param>
        /// <returns> cardテーブルに接続するための ConnectionString 。 </returns>
        public static string ForMySql(string userId, string password)
        {
            System.Text.StringBuilder connectionString = new System.Text.StringBuilder(64);

            //connectionString.Append("Server=localhost;");
            connectionString.Append("Server=mysql1.php.xdomain.ne.jp;");
            connectionString.Append("Database=zeta00s_card;");
            connectionString.Append("Uid=zeta00s_").Append(userId).Append(";");
            connectionString.Append("Pwd=").Append(password).Append(";");
            //connectionString.Append("persist security info=false;");

            return connectionString.ToString();
        }


    }
}