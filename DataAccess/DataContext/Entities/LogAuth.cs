﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccess.DataContext.Entities
{
    public partial class LogAuth
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string RequestIp { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}