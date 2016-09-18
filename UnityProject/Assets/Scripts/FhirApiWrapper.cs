using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Assets.Scripts {
    class FhirApiWrapper
    {
        private HttpWebRequest request;


        private void securityOff()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors) {
                                return true; // **** Always accept
                            };
        }
        //Gets patient info by id
        public FhirApiWrapper(string id)
        {
            securityOff();
            StringBuilder builder = new StringBuilder();
            builder.Append("https://fhir-open-api-dstu2.smarthealthit.org/Patient/");
            request = (HttpWebRequest) WebRequest.Create(builder.ToString());
        }

    }
}
