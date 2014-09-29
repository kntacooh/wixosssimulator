using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
//using System.Reflection;

namespace WixossSimulator.Sql
{
    public class TableData
    {
        protected TableData() { }
        protected TableData(SqlDataReader reader){
            for (int i = 0; i < reader.FieldCount; i++)
            {
                this.GetType().GetProperty(reader.GetName(i)).SetValue(this, reader.GetValue(i));
            }
        }
    }
}