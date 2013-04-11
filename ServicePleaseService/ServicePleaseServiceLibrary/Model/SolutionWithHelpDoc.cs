using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicePleaseServiceLibrary.Model
{
    [Serializable]
    [DataContract]
    public class SolutionWithHelpDoc
    {
        [DataMember]
        public Guid SolutionId { get; set; }

        [DataMember]
        public Guid TicketId { get; set; }

        [DataMember]
        public string SolutionShortDesc { get; set; }

        [DataMember]
        public string SolutionText { get; set; }

        [DataMember]
        public string CreateDate { get; set; }

        [DataMember]
        public string EditDate { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public int LikeCount { get; set; }

        [DataMember]
        public int UnlikeCount { get; set; }

        [DataMember]
        public bool IsHelpDoc { get; set; }
    }
}
