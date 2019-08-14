using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dal;
using Model;
using System.Web.Script.Serialization;

namespace Note
{
	/// <summary>
	/// Note 的摘要说明
	/// </summary>
	public class Note : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			string msgName = context.Request.QueryString["name"];
			string msgContent = context.Request.QueryString["message"];
			if (msgName != null && msgContent != null)
			{

				Message m = new Message()
				{
					msgName = msgName,
					msgContent = msgContent,
					msgTime = DateTime.Now,
				};
				int count = DalBase.Insert(m);
				context.Response.Write(count);
			}
			else
			{
				JavaScriptSerializer js = new JavaScriptSerializer();
				var list = DalBase.SelectAll<Message>();
				string json = js.Serialize(list);
				context.Response.Write(json);
		}
	}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}