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
	public class BlobPacket
	{
		[DataMember]
		public Guid EntityId { get; set; }

		[DataMember]
		public Guid BlobTypeId { get; set; }

		[DataMember]
		public string BlobBytes { get; set; }

        [DataMember]
        public string CreateDate { get; set; }

        [DataMember]
        public string EditDate { get; set; }
	}
}
