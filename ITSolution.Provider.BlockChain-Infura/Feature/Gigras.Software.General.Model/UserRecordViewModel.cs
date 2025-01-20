using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigras.Software.General.Model
{
    public class UserRecordViewModel
    {
        public int RecordId { get; set; }
        public string State { get; set; }
        public Dictionary<string, string> FieldValues { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
