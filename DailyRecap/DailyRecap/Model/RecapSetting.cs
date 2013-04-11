using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyRecap.Model
{
    public class RecapSetting
    {
        public Guid RecapSettingId { get; set; }

        public string Name { get; set; }

        public string BroadcastTime { get; set; }

        public string StartTime { get; set; }

        public string EndTime{ get; set; }

        public string RecapMail{ get; set; }

        public string TimeZone { get; set; }

        public bool Active { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EditDate{ get; set; }

        
    }
}
