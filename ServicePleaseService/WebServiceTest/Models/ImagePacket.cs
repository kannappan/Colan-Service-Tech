using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
	[DataContract]
	[Serializable]
	public class BlobPacket
	{
		[DataMember]
		public Guid EntityId { get; set; }

		[DataMember]
		public Guid BlobTypeId { get; set; }

		[DataMember]
		public string BlobBytes { get; set; }
	}
}