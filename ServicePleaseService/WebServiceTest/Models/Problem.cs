using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
	[DataContract]
	[Serializable]
	public class Problem
	{
		[DataMember]
		public Guid ProblemId { get; set; }

		[DataMember]
		public Guid TicketId { get; set; }

		[DataMember]
		public string ProblemShortDesc { get; set; }

		[DataMember]
		public string ProblemText { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public Nullable<System.DateTime> EditDate { get; set; }
	}
}
