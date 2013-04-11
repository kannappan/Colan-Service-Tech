using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
	[DataContract]
	[Serializable]
	public class DailyRecap
	{
		[DataMember]
        public Guid DailyRecapId { get; set; }

		[DataMember]
		public Guid TechId { get; set; }

		[DataMember]
        public string Action { get; set; }

		[DataMember]
		public string Object { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public Nullable<System.DateTime> EditDate { get; set; }
	}
}
