﻿using Dapper.FluentMap.Mapping;
using System;

namespace EmployeeChatBot.Models
{
    public class ReportDataModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string EmployeeId { get; set; }

        public bool Fever { get; set; }

        public bool Coughing { get; set; }

        public bool Breathing { get; set; }

        public bool SoreThroat { get; set; }

        public bool BodyAches { get; set; }

        public bool LossOfSmell { get; set; }
        public bool VomitDiarrhea { get; set; }
        public bool Traveled { get; set; }
        public bool CloseProximity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsPositive()
        {
            return Fever || Coughing || Breathing || SoreThroat || BodyAches || VomitDiarrhea || CloseProximity;
        }
    }

    public class ReportMap : EntityMap<ReportDataModel>
    {
        internal ReportMap()
        {
            Map(u => u.EmployeeId).ToColumn("EmployeeId");
            Map(u => u.Username).ToColumn("Username");
            Map(u => u.Email).ToColumn("Email");
            Map(u => u.Id).ToColumn("Report_ID");
            Map(u => u.CompletedAt).ToColumn("CompletedAt");
        }
    }
}
