using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeChatBot.Models
{
    public class ClearReportViewModel
    {
        public Collection<ReportDataModel> PositiveReports { get; set; }

        public ClearReportViewModel()
        {
            PositiveReports = new Collection<ReportDataModel>();
        }
    }
}
