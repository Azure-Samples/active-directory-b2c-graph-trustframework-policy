using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace console_csharp_trustframeworkpolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            // validate parameters
            if (!CheckValidParameters(args))
                return;

            HttpRequestMessage request = null;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            try
            {
                // Login as global admin of the Azure AD B2C tenant
                UserMode.LoginAsAdmin();

                // Graph client does not yet support trustFrameworkPolicy, so using HttpClient to make rest calls
                switch (args[0].ToUpper())
                {
                    case "LIST":
                        // List all polcies using "GET /trustFrameworkPolicies"
                        request = UserMode.HttpGet(Constants.TrustFrameworkPolicesUri);
                        break;
                    case "GET":
                        // Get a specific policy using "GET /trustFrameworkPolicies/{id}"
                        request = UserMode.HttpGetID(Constants.TrustFrameworkPolicyByIDUri, args[1]);
                        break;
                    case "CREATE":
                        // Create a policy using "POST /trustFrameworkPolicies" with XML in the body
                        string xml = System.IO.File.ReadAllText(args[1]);
                        request = UserMode.HttpPost(Constants.TrustFrameworkPolicesUri, xml);
                        break;
                    case "UPDATE":
                        // Update using "PUT /trustFrameworkPolicies/{id}" with XML in the body
                        xml = System.IO.File.ReadAllText(args[2]);
                        request = UserMode.HttpPutID(Constants.TrustFrameworkPolicyByIDUri, args[1], xml);
                        break;
                    case "DELETE":
                        // Delete using "DELETE /trustFrameworkPolicies/{id}"
                        request = UserMode.HttpDeleteID(Constants.TrustFrameworkPolicyByIDUri, args[1]);
                        break;
                    default:
                        return;
                }

                Print(request);

                HttpClient httpClient = new HttpClient();
                Task<HttpResponseMessage> response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

                Print(response);
            }
            catch (Exception e)
            {
                Print(request);
                Console.WriteLine("\nError {0} {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "");
            }
        }

        public static bool CheckValidParameters(string[] args)
        {
            if (Constants.ClientIdForUserAuthn.Equals("ENTER_YOUR_CLIENT_ID") ||
                Constants.Tenant.Equals("ENTER_YOUR_TENANT_NAME"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("1. Open 'Constants.cs'");
                Console.WriteLine("2. Update 'ClientIdForUserAuthn'");
                Console.WriteLine("3. Update 'Tenant'");
                Console.WriteLine("");
                Console.WriteLine("See README.md for detailed instructions.");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[press any key to exit]");
                Console.ReadKey();
                return false;
            }

            if (args.Length <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a command as the first argument.");
                Console.ForegroundColor = ConsoleColor.White;
                PrintHelp(args);
                return false;
            }

            switch (args[0].ToUpper())
            {
                case "LIST":
                    break;
                case "GET":
                    if (args.Length <= 1)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "CREATE":
                    if (args.Length <= 1)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "UPDATE":
                    if (args.Length <= 2)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "DELETE":
                    if (args.Length <= 1)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "HELP":
                    PrintHelp(args);
                    return false;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid command.");
                    Console.ForegroundColor = ConsoleColor.White;
                    PrintHelp(args);
                    return false;
            }
            return true;
        }

        public static void Print(Task<HttpResponseMessage> responseTask)
        {
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error Calling the Graph API HTTP Status={0}", response.StatusCode);
            }

            Console.WriteLine(response.Headers);
            Task<string> taskContentString = response.Content.ReadAsStringAsync();
            taskContentString.Wait();
            Console.WriteLine(taskContentString.Result);
        }

        public static void Print(HttpRequestMessage request)
        {
            if(request != null)
            {
                Console.Write(request.Method + " ");
                Console.WriteLine(request.RequestUri);
                Console.WriteLine("");
            }
        }

        private static void PrintHelp(string[] args)
        {
            string appName = "B2CPolicyClient";
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("- Square brackets indicate optional arguments");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("List                         : {0} List", appName);
            Console.WriteLine("Get                          : {0} Get [PolicyID]", appName);
            Console.WriteLine("                             : {0} Get B2C_1A_PolicyName", appName);
            Console.WriteLine("Create                       : {0} Create [RelativePathToXML]", appName);
            Console.WriteLine("                             : {0} Create policytemplate.xml", appName);
            Console.WriteLine("Update                       : {0} Update [PolicyID] [RelativePathToXML]", appName);
            Console.WriteLine("                             : {0} Update B2C_1A_PolicyName updatepolicy.xml", appName);
            Console.WriteLine("Delete                       : {0} Delete [PolicyID]", appName);
            Console.WriteLine("                             : {0} Delete B2C_1A_PolicyName", appName);
            Console.WriteLine("Help                         : {0} Help", appName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");

            if(args.Length == 0)
            {
                Console.WriteLine("[press any key to exit]");
                Console.ReadKey();
            }
        }
    }
}
