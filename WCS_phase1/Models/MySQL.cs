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

        /// <summary>
        /// 获取对应table数据数目
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public int GetCount(String table, String conditions)
        {
            try
            {
                String sql = String.Format(@"select count(*) COUNT from {0} where 1 = 1 and {1}", table, conditions);
                DataTable dt = SelectAll(sql);
                int count = Convert.ToInt32(dt.Rows[0]["COUNT"].ToString());
                return count;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
