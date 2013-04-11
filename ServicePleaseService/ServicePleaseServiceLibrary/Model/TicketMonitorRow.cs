using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ServicePleaseServiceLibrary.Model
{
    [Serializable]
    [DataContract]
    public class TicketMonitorRow
    {
        [DataMember]
        public int TicketNumber { get; set; }

        [DataMember]
        public Guid TicketId { get; set; }

        [DataMember]
        public Guid CategoryId { get; set; }

        [DataMember]
        public Guid ContactId { get; set; }

        [DataMember]
        public Guid LocationId { get; set; }

        [DataMember]
        public Guid OrganizationId { get; set; }

        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Contact { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public double Elapsed { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Tech { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string ServicePlan { get; set; }

        [DataMember]
        public bool SnoozeTicket { get; set; }
    }
}
