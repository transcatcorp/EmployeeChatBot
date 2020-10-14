using Dapper;
using Dapper.FluentMap;
using EmployeeChatBot.Data.Access.Abstraction;
using EmployeeChatBot.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeChatBot.Data.Access
{
    public class ReportAccess : BaseDataAccess, IReportAccess
    {
        public ReportAccess(IConfiguration config) : base(config)
        {
        }

        public async Task SaveReport(int reportId, ReportSymptoms symptoms)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            await
                conn.ExecuteAsync(
                    "[dbo].[Report_Save]",
                    new
                    {
                        ReportId = reportId,
                        symptoms.Cough,
                        symptoms.Fever,
                        symptoms.Breathing,
                        symptoms.SoreThroat,
                        symptoms.BodyAches,
                        symptoms.LossOfSmell,
                        symptoms.VomitDiarrhea,
                        symptoms.Traveled,
                        symptoms.CloseProximity
                    },
                    commandType: CommandType.StoredProcedure);
        }

        public async Task<ReportDataModel> CreateReport(string username, string empId, string email)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            var retVal = await
                conn.QueryFirstOrDefaultAsync<ReportDataModel>(
                    "[dbo].[Report_Create]",
                    new
                    {
                        Username = username,
                        Email = email,
                        EmployeeId = empId,
                    },
                    commandType: CommandType.StoredProcedure);

            return retVal;
        }

        public async Task<ReportDataModel> CheckReportByEmployeeId(string empId)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            var reports = await
                conn.QueryFirstOrDefaultAsync<ReportDataModel>(
                    "[dbo].[Report_CheckByEmployeeId]",
                    new
                    {
                        EmployeeId = empId
                    },
                    commandType: CommandType.StoredProcedure);

            return
                reports;
        }

        // This is an optional table that we utilized in order to track individuals who had login failures.
        // We would follow up with them to ensure their accounts were setup properly and they could take the chatbot daily
        // The Domain indicated the issue with the account and what account they had (UR vs URMC Active Directory in our case)
        // Username was the username they attempted to use.
        public async Task LogFailedLogin(string username, string domain)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            await
                conn.ExecuteAsync(
                    "[dbo].[FailedLogin_Save]",
                    new
                    {
                        Domain = domain,
                        Username = username
                    },
                    commandType: CommandType.StoredProcedure);
        }

        public async Task<ReportDataModel> CheckReport(int reportId)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            var retVal = await
                conn.QueryFirstOrDefaultAsync<ReportDataModel>(
                    "[dbo].[Report_CheckById]",
                    new
                    {
                        ReportId = reportId
                    },
                    commandType: CommandType.StoredProcedure);

            return retVal;
        }

        public async Task<Collection<ReportDataModel>> GetPositiveReports()
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            var retVal = await
                conn.QueryAsync<ReportDataModel>(
                    "[dbo].[Report_GetAllPositiveReports]",
                    commandType: CommandType.StoredProcedure);

            return new Collection<ReportDataModel>(retVal.ToList());
        }

        public async Task ClearReport (int id)
        {
            using IDbConnection conn = DbConnection;
            conn.Open();

            var retVal = await
                conn.QueryAsync(
                    "[dbo].[Report_ClearPositiveResult]",
                    new
                    {
                        ReportId = id
                    },
                    commandType: CommandType.StoredProcedure);

            return;
        }
    }
}