using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

//Auto-generated class created from JSON file
namespace FinalProjectA
{
    public class Event
    {
        public string type { get; set; }

        public string date { get; set; }
    }

    public class Product
    {
        public string productType { get; set; }
        public string subProduct { get; set; }
    }

    public class Issue
    {
        public string issueType { get; set; }


        public string subIssue { get; set; }
    }

    public class Company
    {
        public string companyName { get; set; }

        public string companyState { get; set; }
        public string companyZip { get; set; }
    }


    public class Submitted
    {
        public string via { get; set; }
    }

    public class Response
    {
        public string timely { get; set; }
        public string consumerDisputed { get; set; }
        public string responseType { get; set; }
        //public string publicResponse { get; set; }

        private string ppublicResponse;
        public string publicResponse
        {
            get { return ppublicResponse; }
            set
            {
               
                ppublicResponse = Regex.Replace(value, @"\s+", " ");
            }
        }
    }

    public class Complaint
    {
        public string id { get; set; }

        [XmlIgnore]
        public List<Event> @event { get; set; }
        public Product product { get; set; }
        public Issue issue { get; set; }
        public Company company { get; set; }

        
        [XmlIgnore]
        public Submitted submitted { get; set; }

        public Response response { get; set; }
        public string consumerNarrative { get; set; }

        public string submissionType { get; set; }

        public string sentToCompanyDate { get; set; }

        public string receivedDate { get; set; }


        public Complaint()
        {
        }

  
    }
    public class ConsumerComplaints
    {
        public List<Complaint> complaint { get; set; }
    }

    public class RootObject
    {
        public ConsumerComplaints consumerComplaints { get; set; }
    }
}
