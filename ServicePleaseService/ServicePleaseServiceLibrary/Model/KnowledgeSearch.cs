using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServicePleaseServiceLibrary.Model
{
    [DataContract]
    public enum HelpDocOptions
    {
        [EnumMember]
        NoHelpDocs = 1,

        [EnumMember]
        JustHelpDocs = 2,

        [EnumMember]
        Combined = 3
    }
         
    [DataContract]
    public class KnowledgeSearch
    {
        [DataMember]
        public HelpDocOptions HelpDocOption { get; set; }

        [DataMember]
        public List<Guid> CategoryIds { get; set; }

        [DataMember]
        public List<Guid> LocationIds { get; set; }

        [DataMember]
        public List<string> SearchTerms { get; set; }

         [DataMember]
        public string IsCategoryAll { get; set; }

         [DataMember]
        public string IsLocationAll { get; set; }
        
    }
}
