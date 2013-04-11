using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicePleaseServiceLibrary.Model
{
    [Serializable]
    [DataContract]
    public class TechMetrics
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string UserName{ get; set; }

        [DataMember]
        public long TTO{ get; set; }

        [DataMember]
        public long TTC { get; set; }

        [DataMember]
        public double ART{ get; set; }

        [DataMember]
        public double ATO{get;set;}

        [DataMember]
        public string HashedPassword{get;set;}

        [DataMember]
        public string FirstName{get;set;}

        [DataMember]
        public string MiddleName{get;set;}

        [DataMember]
        public string LastName{get;set;}

        [DataMember]
        public string Email{get;set;}

        [DataMember]
        public string Phone{get;set;}

        [DataMember]
        public string CreateDate { get; set; }

        [DataMember]
        public string EditDate { get; set; }

    }
}
