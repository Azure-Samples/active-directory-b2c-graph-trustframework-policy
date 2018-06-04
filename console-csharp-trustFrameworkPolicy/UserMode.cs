using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using System.Net.Http;

namespace console_csharp_trustframeworkpolicy
{
    internal class UserMode
    {
        public static GraphServiceClient client;


        public static bool CreateGraphClient()
        {
            try
            {
                //*********************************************************************
                // setup Microsoft Graph Client for delegated user.
                //*********************************************************************
                if (Constants.ClientIdForUserAuthn != "ENTER_YOUR_CLIENT_ID")
                {
                    client = AuthenticationHelper.GetAuthenticatedClientForUser();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You haven't configured a value for ClientIdForUserAuthn in Constants.cs. Please follow the Readme instructions for configuring this application.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Acquiring a token failed with the following error: {0}", ex.Message);
                if (ex.InnerException != null)
                {
                    //You should implement retry and back-off logic per the guidance given here:http://msdn.microsoft.com/en-us/library/dn168916.aspx
                    //InnerException Message will contain the HTTP error status codes mentioned in the link above
                    Console.WriteLine("Error detail: {0}", ex.InnerException.Message);
                }
                Console.ResetColor();
                Console.ReadKey();
                return false;
            }
        }

        public static HttpRequestMessage HttpGet(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            AuthenticationHelper.AddHeaders(request);
            return request;
        }

        public static HttpRequestMessage HttpGetID(string uri, string id)
        {
            string uriWithID = String.Format(uri, id);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uriWithID);
            AuthenticationHelper.AddHeaders(request);
            return request;
        }

        public static HttpRequestMessage HttpPutID(string uri, string id, string xml)
        {
            string uriWithID = String.Format(uri, id);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uriWithID);
            AuthenticationHelper.AddHeaders(request);
            request.Content = new StringContent(xml, Encoding.UTF8, "application/xml");
            return request;
        }

        public static HttpRequestMessage HttpPost(string uri, string xml)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            AuthenticationHelper.AddHeaders(request);
            request.Content = new StringContent(xml, Encoding.UTF8, "application/xml");
            return request;
        }

        public static HttpRequestMessage HttpDeleteID(string uri, string id)
        {
            string uriWithID = String.Format(uri, id);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uriWithID);
            AuthenticationHelper.AddHeaders(request);
            return request;
        }

        public static void LoginAsAdmin()
        {
            Console.WriteLine("Login as a global admin of the tenant (example: admin@myb2c.onmicrosoft.com");
            Console.WriteLine("=============================");

            if (CreateGraphClient())
            {
                User user = client.Me.Request().GetAsync().Result;
                Console.WriteLine("Current user:    Id: {0}  UPN: {1}", user.Id, user.UserPrincipalName);
            }
        }
    }
}
