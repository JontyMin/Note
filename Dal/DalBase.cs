﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Model;
using System.Reflection;
namespace Dal
{
    public class DalBase
    {

		/// <summary>
		/// 配置文件连接字符串
		/// </summary>
		private static readonly string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString();

		#region DBHelp
		#region MyExecuteNonQuery
		/// <summary>
		/// ExecuteNonQuery方法
		/// </summary>
		/// <param name="sql">SQL语句</param>
		/// <param name="sp">参数化</param>
		/// <returns>int影响行数</returns>
		private static int MyExecuteNonQuery(string sql, SqlParameter[] sp)
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				if (sp != null)
				{
					cmd.Parameters.AddRange(sp);
				}
				return cmd.ExecuteNonQuery();
			}
		}
		#endregion


		#region ExecuteSqlDataReader
		/// <summary>
		/// ExecuteSqlDataReader
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="sp"></param>
		/// <returns></returns>
		private static SqlDataReader ExecuteSqlDataReader(string sql, SqlParameter[] sp)
		{
			SqlConnection conn = new SqlConnection(connStr);
			conn.Open();
			SqlCommand cmd = new SqlCommand(sql, conn);
			if (sp != null)
			{
				cmd.Parameters.AddRange(sp);
			}
			SqlDataReader sd = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			return sd;
		}
		#endregion


		#region ExecuteObject
		/// <summary>
		/// ExecuteObject
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		private static object ExecuteObject(string sql)
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				return cmd.ExecuteScalar();

			}
		}
		#endregion
		#endregion



		/// <summary>
		/// 插入数据
		/// </summary>
		/// <param name="s"></param>
		/// <returns>int影响行数</returns>
		public static int Insert(ModelBase s) {
			Type t = s.GetType();//反射
			string tname = t.Name;//表名
			List<SqlParameter> list = new List<SqlParameter>();
			StringBuilder sb = new StringBuilder();
			StringBuilder sb1 = new StringBuilder();
			sb.Append("insert into ");
			sb.Append(tname);
			sb.Append("(");
			PropertyInfo[] pi = t.GetProperties();
			foreach (PropertyInfo item in pi)
			{
				if (item.GetValue(s,null)!=null)
				{
					sb.Append(item.Name);
					sb.Append(",");
					SqlParameter p = new SqlParameter("@"+item.Name,item.GetValue(s,null));
					list.Add(p);
					sb1.Append("@"+item.Name);
					sb1.Append(",");
				}
			}
			sb.Remove(sb.Length-1,1);
			sb.Append(")");
			sb.Append(" values (");
			sb1.Remove(sb1.Length-1,1);
			sb.Append(sb1+")");
			Console.WriteLine(sb);
			return MyExecuteNonQuery(sb.ToString(),list.ToArray());
		}


		/// <summary>
		/// 查询所有数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> SelectAll<T>() where T : ModelBase {
			List<T> list = new List<T>();
			Type t = typeof(T);
			string sql = "select * from " + t.Name;
			using (SqlDataReader sd=ExecuteSqlDataReader(sql,null))
			{
				while (sd.Read())
				{
					T t1 = Activator.CreateInstance<T>();
					PropertyInfo[] ps = t.GetProperties();
					foreach (PropertyInfo item in ps)
					{
						if (sd[item.Name]!=DBNull.Value)
						{
							item.SetValue(t1,sd[item.Name]);
						}
					}
					list.Add(t1);
				}

			}
			return list;
		}

		/// <summary>
		/// 条件查询
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql"></param>
		/// <param name="sp"></param>
		/// <returns></returns>
		public static List<T> SelectByid<T>(string sql,SqlParameter[]sp) where T:ModelBase
		{
			List<T> list = new List<T>();
			Type t = typeof(T);
			using (SqlDataReader sd=ExecuteSqlDataReader(sql,sp))
			{
				T t1 = Activator.CreateInstance<T>();
				PropertyInfo[] pi = t.GetProperties();
				foreach (PropertyInfo item in pi)
				{
					if (sd[item.Name]!=DBNull.Value)
					{
						item.SetValue(t1,sd[item.Name]);
					}
				}
				list.Add(t1);
			}
			return list;
		}

		/// <summary>
		/// 修改
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int Updata(ModelBase s) {
			Type t = s.GetType();
			string tname = t.Name;
			StringBuilder sb = new StringBuilder();
			sb.Append("updata");
			sb.Append(tname);
			sb.Append(" set ");
			string keyname = "";
			object[] objs = t.GetCustomAttributes(true);
			List<SqlParameter> list = new List<SqlParameter>();
			if (objs.Length>0)
			{
				keyname = (objs[0] as KeyInfoAttribute).KeyName;
			}
			else
			{
				throw new Exception("get out");
			}
			foreach (PropertyInfo item in t.GetProperties())
			{
				if (item.GetValue(s,null)!=null)
				{
					if (item.Name.ToLower()!=keyname.ToLower())
					{
						sb.Append(item.Name);
						sb.Append("=");
						sb.Append("@"+item.Name);
						sb.Append(",");

						list.Add(new SqlParameter("@"+item.Name,item.GetValue(s,null)));
					}
				}
			}
			sb.Remove(sb.Length-1,1);
			sb.Append(" where ");
			sb.Append(keyname);
			sb.Append("=");
			sb.Append("@"+keyname);

			return MyExecuteNonQuery(sb.ToString(), list.ToArray()); ;
		}
		/// <summary>
		///删除
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <returns></returns>
		public static int Delete<T>(object o) {
			Type t = typeof(T);
			string tname = t.Name;
			StringBuilder sb = new StringBuilder();
			sb.Append("delete from ");
			sb.Append(tname);
			sb.Append(" where ");
			string keyname = "";
			object[] objs = t.GetCustomAttributes(true);
			List<SqlParameter> list = new List<SqlParameter>();
			if (objs.Length > 0)
			{
				keyname = (objs[0] as KeyInfoAttribute).KeyName;
			}
			else
			{
				throw new Exception("get out");
			}
			sb.Append(keyname);
			sb.Append("=");
			sb.Append("@"+keyname);
			SqlParameter sp = new SqlParameter("@" + keyname, o);
			return MyExecuteNonQuery(sb.ToString(),new SqlParameter[] { sp}) ;
		}

		public static int GetCount<T>() where T : ModelBase
		{
			Type t = typeof(T);
			string tableName = t.Name;
			string sql = "select count(*) from " + tableName + "";
			return Convert.ToInt32(ExecuteObject(sql));
		}
		public static int GetCount(string sql)
		{

			return Convert.ToInt32(ExecuteObject(sql));
		}
		public static int GetMax<T>() where T : ModelBase
		{
			Type t = typeof(T);
			string tableName = t.Name;
			string sql = "select Max(UID) from " + tableName + "";
			return Convert.ToInt32(ExecuteObject(sql));
		}
	}
}
