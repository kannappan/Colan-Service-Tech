using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
	[DataContract]
	[Serializable]
	public class SolutionBlob
	{
		[DataMember]
		public Guid SolutionBlobId { get; set; }

		[DataMember]
		public Guid SolutionId { get; set; }

		[DataMember]
		public Guid BlobId { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public Nullable<DateTime> EditDate { get; set; }
	}
}
