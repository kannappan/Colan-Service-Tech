using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
	[DataContract]
	[Serializable]
	public class BlobEntry
	{
		[DataMember]
		public Guid BlobId { get; set; }

		[DataMember]
		public Byte[] Blob { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public Nullable<DateTime> EditDate { get; set; }
	}
}
