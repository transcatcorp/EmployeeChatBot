using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeChatBot.Models
{
    public class ReportSymptoms
    {
        public bool Cough { get; set; }
        public bool Fever { get; set; }
        public bool Breathing { get; set; }
        public bool SoreThroat { get; set; }
        public bool BodyAches { get; set; }
        public bool LossOfSmell { get; set; }
        public bool VomitDiarrhea { get; set; }
        public bool Traveled { get; set; }
        public bool CloseProximity { get; set; }


        public bool IsPositive()
        {
            return Fever || Cough || Breathing || SoreThroat || BodyAches || LossOfSmell || VomitDiarrhea || Traveled || CloseProximity;
        }
    }
}
