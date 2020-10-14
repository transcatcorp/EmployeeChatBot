using EmployeeChatBot.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeChatBot.Data.Access.Abstraction
{
    public interface IReportAccess
    {
        Task SaveReport(int reportId, ReportSymptoms symptoms);

        Task<ReportDataModel> CheckReport(int reportId);

        Task<ReportDataModel> CreateReport(string username, string empId, string email);

        Task<ReportDataModel> CheckReportByEmployeeId(string empId);

        Task LogFailedLogin(string username, string domain);

        Task<Collection<ReportDataModel>> GetPositiveReports();
        Task ClearReport(int id);
    }
}