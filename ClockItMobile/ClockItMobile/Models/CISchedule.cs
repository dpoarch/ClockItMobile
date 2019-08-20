using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace ClockIt.Mobile.Models
{
    public class CISchedule
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public List<CIPeriod> Periods { get; set; }
        public double TotalHours { get; set; }
        public int Id { get; set; }
        public string Time { get; set; }
    }
    public class CIScheduleMapped
    {
        public DateTime A { get; set; }
        public string B { get; set; }
        public List<CIPeriodMapped> C { get; set; }
        public double D { get; set; }
        public int E { get; set; }
        public string F { get; set; }
    }
    public class CIPeriod
    {
        public TimeSpan Interval { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }
    public class CIPeriodMapped
    {
        public TimeSpan A { get; set; }
        public string B { get; set; }
        public int C { get; set; }
    }
    public class MasterPageItem
    {
        public string Title { get; set; }
        public string TargetType { get; set; }

    }
    public class TodoItem
    {
        string id;
        string name;
        bool done;
        string username;
        string email;
        string schedules;
        string schedules2;
        string schedules3;
        string schedules4;
        string schedules5;
        DateTime dateCreated;

        [JsonProperty(PropertyName = "_id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "text")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "complete")]
        public bool Done
        {
            get { return done; }
            set { done = value; }
        }

        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get { return username; }
            set { username = value; }
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [JsonProperty(PropertyName = "schedules")]
        public string Schedules
        {
            get { return schedules; }
            set { schedules = value; }
        }

        [JsonProperty(PropertyName = "schedules2")]
        public string Schedules2
        {
            get { return schedules2; }
            set { schedules2 = value; }
        }
        [JsonProperty(PropertyName = "schedules3")]
        public string Schedules3
        {
            get { return schedules3; }
            set { schedules3 = value; }
        }
        [JsonProperty(PropertyName = "schedules4")]
        public string Schedules4
        {
            get { return schedules4; }
            set { schedules4 = value; }
        }
        [JsonProperty(PropertyName = "schedules5")]
        public string Schedules5
        {
            get { return schedules5; }
            set { schedules5 = value; }
        }
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }
        [Version]
        public string Version { get; set; }
    }
    public class ClockItUser
    {
        string id;
        string username;
        string email;
        List<CISchedule> schedules;
        bool done;
        DateTime dateCreated;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get { return username; }
            set { username = value; }
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [JsonProperty(PropertyName = "schedules")]
        public List<CISchedule> Schedules
        {
            get { return schedules; }
            set { schedules = value; }
        }
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }
        /*
        [JsonProperty(PropertyName = "complete")]
        public bool Done
        {
            get { return done; }
            set { done = value; }
        }
        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        [Version]
        public string Version { get; set; }*/
    }
}
