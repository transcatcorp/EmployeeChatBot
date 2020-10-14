using EmployeeChatBot.ActiveDirectory;
using EmployeeChatBot.Data;
using EmployeeChatBot.Data.Access.Abstraction;
using EmployeeChatBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;
using URMC.ActiveDirectory;

namespace EmployeeChatBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly IReportAccess _reportAccess;
        private readonly ActiveDirectoryOptions _adOptions;
        private readonly EmailOptions _mailOptions;
        private readonly ElevatedUsersOptions _userOptions;

        public HomeController(IReportAccess reportAccess, IOptions<ActiveDirectoryOptions> adOptions, IOptions<EmailOptions> mailOptions, IOptions<ElevatedUsersOptions> userOptions)
        {
            _reportAccess = reportAccess;
            _adOptions = adOptions.Value;
            _mailOptions = mailOptions.Value;
            _userOptions = userOptions.Value;
        }

        public async Task<IActionResult> SaveReport(ReportModel model)
        {
            if (model.Symptoms == null)
                model.Symptoms = "";

            if (model.ReportId == null)
                return Forbid();

            int reportId = Convert.ToInt32(model.ReportId);
            var reportCheck = await _reportAccess.CheckReport(reportId);
            if (reportCheck.CompletedAt != null)
            {
                return Forbid();
            }

            var symptoms = new ReportSymptoms()
            {
                Cough = model.Symptoms.Contains("Cough"),
                Fever = model.Symptoms.Contains("Temperature"),
                Breathing = model.Symptoms.Contains("Breathing"),
                SoreThroat = model.Symptoms.Contains("Sore Throat"),
                BodyAches = model.Symptoms.Contains("Body Aches"),
                LossOfSmell = model.Symptoms.Contains("Loss of taste or smell"),
                VomitDiarrhea = model.Symptoms.Contains("Vomiting or diarrhea"),
                Traveled = model.Symptoms.Contains("Traveled"),
                CloseProximity = model.Symptoms.Contains("Close Proximity")
            };

            await _reportAccess.SaveReport(reportId, symptoms);

            if (symptoms.IsPositive())
            {
                SendNotificationEmail(_reportAccess.CheckReport(reportId).Result);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel model)
        {
            //Login Logic here
            ActiveDirectoryUser user = null;
            IndexViewModel toRet = new IndexViewModel();
            // Login here!
            URMC.ActiveDirectory.ActiveDirectory directory = new URMC.ActiveDirectory.ActiveDirectory(_adOptions);
            try
            {
                user = directory.AuthenticateAsync(new Credentials() { Username = model.Username, Password = model.Password }).GetAwaiter().GetResult();
            }
            catch (UnauthorizedADAccessException)
            {
                LoginViewModel errorModel = new LoginViewModel();
                errorModel.FailedLogin = true;
                return View(errorModel);
            }

            // At this point you can optionally choose to check if the user has a report for the day by looking up their report by EmployeeId
            // You can then set the HasReport flag on the VM to display the previous report for the day
            var report = await _reportAccess.CheckReportByEmployeeId(user.EmployeeId);

            if (report != null)
            {
                //check to see if this report was completed today
                if(report.CompletedAt != null && report.CompletedAt.Value.Date != DateTime.Today)
                {
                    report = null;
                }
            }
            
            if(report == null)
            {
                report = await _reportAccess.CreateReport(user.Username, user.EmployeeId, user.Mail);
            }

            IndexViewModel indexViewModel = new IndexViewModel
            {
                ReportId = report.Id,
                HasReport = report.CompletedAt != null,
                IsPositiveReport = report.IsPositive(),
                IsElevatedUser = _userOptions.Users.Contains(user.Username)
            };

            HttpContext.Session.SetInt32("IsElevatedUser", indexViewModel.IsElevatedUser ? 1 : 0);

            //if this is an elevated user and they already did their assessment
            if(indexViewModel.IsElevatedUser && indexViewModel.HasReport && indexViewModel.IsElevatedUser)
            {
                return RedirectToAction("Index", "ClearReport", new ClearReportViewModel() { IsElevatedUser = indexViewModel.IsElevatedUser });
            }

            return RedirectToAction("Index", indexViewModel);
        }

        [HttpGet]
        public IActionResult Index(IndexViewModel model)
        {
            var dayOfWeek = DateTime.Now.DayOfWeek.ToString();
            var chatBotImage = "CB1A.PNG";
            switch (dayOfWeek)
            {
                case "Monday":
                    chatBotImage = "CB2A.PNG";
                    break;
                case "Tuesday":
                    chatBotImage = "CB3A.PNG";
                    break;
                case "Wednesday":
                    chatBotImage = "CB4A.PNG";
                    break;
                case "Thursday":
                    chatBotImage = "CB5A.PNG";
                    break;
                case "Friday":
                    chatBotImage = "CB6A.PNG";
                    break;
                case "Saturday":
                    chatBotImage = "CB7A.PNG";
                    break;
                case "Sunday":
                    chatBotImage = "CB1A.PNG";
                    break;
            }
            model.BotImage = chatBotImage;

            // It is possible for a person to bookmark the post-sign in index page. The following options are available
            // 1) Secure the site using an Identity system and then put this action (and Save) under the Authorize tag
            // 2) Check the report ID here and if it has already been completed redirect them back to the login page

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

        private void SendNotificationEmail(ReportDataModel dataModel)
        {
            SmtpClient client = new SmtpClient(_mailOptions.MailServer)
            {
                UseDefaultCredentials = true
            };

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(_mailOptions.From),
                Subject = "Employee Has COVID-19 Symptoms",
                Priority = MailPriority.High
            };

            message.To.Add(_mailOptions.To);
            message.Body = String.Format("Employee {0} has reported COVID-19 symptoms, Travel, or COVID-19 Exposure", dataModel.Email);

            client.Send(message);
        }
    }
}