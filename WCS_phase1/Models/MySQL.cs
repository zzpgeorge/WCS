using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace WCS_phase1.Models
{
    class MySQL
    {
        static readonly String conn = ConfigurationManager.AppSettings["MySqlConn"];

        /// <summary>
        /// 获取所有字段数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable SelectAll(String sql)
        {
            try
            {
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sql, conn);
                DataSet dataSet = new DataSet();
                mySqlDataAdapter.Fill(dataSet, "Table");
                return dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public void ExcuteSql(String sql)
        {
            try
            {
                MySqlConnection sqlcon = new MySqlConnection(conn);
                sqlcon.Open();
                MySqlCommand mySqlCommand = new MySqlCommand(sql, sqlcon);
                mySqlCommand.ExecuteNonQuery();
                sqlcon.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
