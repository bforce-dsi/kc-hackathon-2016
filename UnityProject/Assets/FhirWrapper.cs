
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Pathfinding.Serialization.JsonFx;
namespace Assets {
    [System.Serializable]
    public class Link {
        public string relation { get; set; }
        public string url { get; set; }
    }
    [System.Serializable]
    public class Meta {
        public string versionId { get; set; }
        public string lastUpdated { get; set; }
    }
    [System.Serializable]
    public class Text {
        public string status { get; set; }
        public string div { get; set; }
    }
    [System.Serializable]
    public class Recorder {
        public string reference { get; set; }
        public string display { get; set; }
    }
    [System.Serializable]
    public class Patient {
        public string reference { get; set; }
        public string display { get; set; }
    }
    [System.Serializable]
    public class Coding {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
        public bool userSelected { get; set; }
    }
    [System.Serializable]
    public class Substance {
        public string text { get; set; }
        public List<Coding> coding { get; set; }
    }
    [System.Serializable]
    public class Resource {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta meta { get; set; }
        public Text text { get; set; }
        public string recordedDate { get; set; }
        public Recorder recorder { get; set; }
        public Patient patient { get; set; }
        public Substance substance { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string category { get; set; }
    }
    [System.Serializable]
    public class Entry {
        public string fullUrl { get; set; }
        public Resource resource { get; set; }
    }
    [System.Serializable]
    public class RootObject {
        public string resourceType { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public int total { get; set; }
        public List<Link> link { get; set; }
        public List<Entry> entry { get; set; }
    }

    public class FhirWrapper {
        private string jsonString;
        public FhirWrapper(string id) {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
           delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors) {
                                       return true; // **** Always accept
                                   };
            HttpWebRequest request =
                (HttpWebRequest)
                WebRequest.Create(
                    "https://fhir-open.sandboxcernerpowerchart.com/dstu2/d075cf8b-3261-481d-97e5-ba6c48d3b41f/AllergyIntolerance?patient=" + id);
            request.ContentType = "charset=utf-8";
            request.Accept = "application/json";

            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            jsonString = reader.ReadToEnd();


        }

        public string MakeApiRequest(string parameters)
        {
            try
            {
                string requestString =
                    "https://fhir-open.sandboxcernerpowerchart.com/dstu2/d075cf8b-3261-481d-97e5-ba6c48d3b41f/" + parameters;
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(requestString);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return null;
            }            
        }
        public List<Dictionary<string, string>> GetPatientAllergies()
        {
            
            List<Dictionary<string, string>> allergies = new List<Dictionary<string, string>>();
            RootObject data = JsonReader.Deserialize<RootObject>(jsonString);


            foreach (Entry entry in data.entry) {
                Dictionary<string, string> allergy = new Dictionary<string, string>();
                allergy.Add("Allergy", entry.resource.substance.text);
                allergy.Add("Type", entry.resource.type);
                allergy.Add("Status", entry.resource.status);
                allergy.Add("Category", entry.resource.category);
                if (AllergyIsActive(allergy)) {
                    allergies.Add(allergy);
                }
            }
            return allergies;
        }

        private bool AllergyIsActive(Dictionary<string, string> allergy)
        {
            string status;
            allergy.TryGetValue("Status", out status);
            if (status != null)
            {
                switch (status.ToLower()) {
                    case "inactive":
                        return false;
                    case "entered-in-error":
                        return false;
                    case "refuted":
                        return false;
                    case "resolved":
                        return false;
                    default:
                        return true;
                }
            }
            return false;
        }
    }
}
