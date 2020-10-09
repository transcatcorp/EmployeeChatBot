using EmployeeChatBot.ActiveDirectory;
using EmployeeChatBot.Data;
using EmployeeChatBot.Data.Access.Abstraction;
using EmployeeChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;
using URMC.ActiveDirectory;

namespace EmployeeChatBot.Controllers
{
    public class ClearReportController : Controller
    {
        private readonly IReportAccess _reportAccess;
        private readonly ActiveDirectoryOptions _adOptions;
        private readonly EmailOptions _mailOptions;

        public ClearReportController(IReportAccess reportAccess, IOptions<ActiveDirectoryOptions> adOptions, IOptions<EmailOptions> mailOptions)
        {
            _reportAccess = reportAccess;
            _adOptions = adOptions.Value;
            _mailOptions = mailOptions.Value;
        }

        [Route("ClearReport/{id:int}")]
        public async Task<IActionResult> ClearReport(int id)
        {
            await _reportAccess.ClearReport(id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Index(ClearReportViewModel model)
        {
            model.PositiveReports = await _reportAccess.GetPositiveReports();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}