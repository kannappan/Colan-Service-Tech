using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace WebServiceTest.Models
{
    [DataContract]
    [Serializable]
    public class EmailElements
    {
        [DataMember]
        public string FromAddress { get; set; }
        
        [DataMember]
        public StringCollection ToAddresses { get; set; }
        
        [DataMember]
        public StringCollection CCAddresses { get; set; }
        
        [DataMember]
        public string SubjectText { get; set; }
        
        [DataMember]
        public string BodyText { get; set; }
    }
}
