using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServiceTest.Models
{
    public class Category
    {
        public Guid CategoryId {get; set;}

        public Guid? OrganizationId { get; set; }

        public  string CategoryName { get; set; }

        public Byte[] CategoryIcon { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
