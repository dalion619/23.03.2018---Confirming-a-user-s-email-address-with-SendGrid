using Newtonsoft.Json;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleConfirmingEmailAddressesWithSendGrid
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
            Console.ReadLine();
        }

        async static Task RunAsync()
        {
            var email = "dev@lionelchetty.co.za";
            var sendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];

            var client = new SendGrid.SendGridClient(sendGridApiKey);
            string queryParams = $"{{'limit':100,'email': '{email}'}}";
            var response = await client.RequestAsync(method: SendGridClient.Method.GET, urlPath: "email_activity", queryParams: queryParams);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Body.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(json))
                {
                    var emailActivity = JsonConvert.DeserializeObject<List<SendGridQueryResponseModel>>(json);
                    if (emailActivity.FirstOrDefault(x => x.@event == "open") != null)
                    {
                        var emailConfirmed = true;
                    }
                }
            }
            
        }

        public class SendGridQueryResponseModel
        {
           
            public DateTime timestamp
            {
                get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(this.created); }                
            }
            public int created { get; set; }
            public string email { get; set; }
            public string @event { get; set; }
        }
    }
}
