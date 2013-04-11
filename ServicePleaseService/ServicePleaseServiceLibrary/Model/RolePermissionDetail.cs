using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServicePleaseServiceLibrary.Model
{
    [Serializable]
    [DataContract]
    public class RolePermissionDetail
    {
        [DataMember]
        public Guid PermissionId { get; set; }

        [DataMember]
        public Guid RoleId { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string CreateDate { get; set; }

        [DataMember]
        public string EditDate { get; set; }

        [DataMember]
        public bool Status { get; set; }
    }
}
