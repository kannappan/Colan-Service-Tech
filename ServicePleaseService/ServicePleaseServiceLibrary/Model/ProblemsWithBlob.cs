using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicePleaseServiceLibrary.Model
{
    [Serializable]
    //public enum BlobsIconTypes
    //{
    //    [EnumMember]
    //    Audio = 1,

    //    [EnumMember]
    //    Video = 2,

    //    [EnumMember]
    //    Image = 4
    //}

    [DataContract]
    public class ProblemsWithBlob
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
        public string LikeCount { get; set; }

        [DataMember]
        public string UnlikeCount { get; set; }

        [DataMember]
        public string CreateDate { get; set; }

        [DataMember]
        public string EditDate { get; set; }
        
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string BlobsIconType { get; set; }
    }
}
