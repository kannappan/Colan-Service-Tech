using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace ServicePleaseServiceLibrary.Model
{
	[DataContract]
	[Serializable]
	public class ProblemBlobPacket
	{
		[DataMember]
		public Guid ProblemBlobId { get; set; }

		[DataMember]
		public Guid ProblemId { get; set; }

		[DataMember]
		public Guid BlobEntryId { get; set; }

		[DataMember]
		public Guid BlobTypeId { get; set; }

		[DataMember]
		public string BlobBytes { get; set; }
	}
}
