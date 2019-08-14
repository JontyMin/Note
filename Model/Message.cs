using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

	[KeyInfo("msgID")]
	public class Message : ModelBase
	{
		public System.Int32 ? msgID { get; set; }
		public System.String msgContent { get; set; }
		public System.DateTime ? msgTime { get; set; }
		public System.String msgName { get; set; }
	}
}
