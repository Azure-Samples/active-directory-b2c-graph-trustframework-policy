using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace console_csharp_trustframeworkpolicy
{
    class Program
    {
        static string resource = "POLICY";
        static string command = "LIST";
        static string[] commands = { "LIST", "GET", "CREATE", "UPDATE", "DELETE" };
        static string[] resources = { "POLICY", "KEYSET"};
        static int uriIndex = 0;
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
                switch (command.ToUpper())
                {
                    case "LIST":
                        // List all polcies using "GET /trustFrameworkPolicies"
                        request = UserMode.HttpGet(Constants.ResourceUri[uriIndex]);
                        break;
                    case "GET":
                        // Get a specific policy using "GET /trustFrameworkPolicies/{id}"
                        request = UserMode.HttpGetID(Constants.ResourceUri[uriIndex], args[1]);
                        break;
                    case "CREATE":
                        // Create a policy using "POST /trustFrameworkPolicies" with XML in the body
                        string xml = System.IO.File.ReadAllText(args[1]);
                        request = UserMode.HttpPost(Constants.ResourceUri[uriIndex], xml);
                        break;
                    case "UPDATE":
                        // Update using "PUT /trustFrameworkPolicies/{id}" with XML in the body
                        xml = System.IO.File.ReadAllText(args[2]);
                        request = UserMode.HttpPutID(Constants.ResourceUri[uriIndex], args[1], xml);
                        break;
                    case "DELETE":
                        // Delete using "DELETE /trustFrameworkPolicies/{id}"
                        request = UserMode.HttpDeleteID(Constants.ResourceUri[uriIndex], args[1]);
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

        public void GetGraphUri()
        {

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
            List<string> argList = new List<string>(args);
            if (args == null)
            {
                Print("Passing default parameters Resource: POLICY and Command: LIST");
                return true;
            }
            else if (args.Length == 0)
            { 
                argList.Add("LIST");
            }
            for(int i=0;i<args.Length;i++)
            {
                //we dont care after the first 2 parameters
                if (i > 1)
                    break;
                //determin which 
                if (Array.Exists<string>(resources, k => k.Equals(args[i].ToUpper())))
                {
                    resource = args[i].ToUpper();
                    argList.RemoveAt(i);
                }
                if (Array.Exists<string>(commands, m => m.Equals(args[i].ToUpper())))
                {
                    command = args[i].ToUpper();
                }
                
            }
            if (resource == "KEYSET")
            {
                uriIndex = 1;
            }
            Print("Received parameters " + string.Join(" ", args));
            Print("massaged parameters " + string.Join(" ", argList.ToArray()));
            Print(string.Format("Inferred resource {0} and command {1} and index uri {2}", resource, command, uriIndex));

            switch (command)
            {
                case "LIST":
                    break;
                case "GET":
                    if (argList.Count <= 1)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "CREATE":
                    if (argList.Count <= 1)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "UPDATE":
                    if (argList.Count <= 2)
                    {
                        PrintHelp(args);
                        return false;
                    }
                    break;
                case "DELETE":
                    if (argList.Count <= 1)
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

        public static void Print(string print)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(print);
            
        }

        public static void PrintError(string print, params string[] args)
        {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(print);
                Console.ForegroundColor = ConsoleColor.White;
                PrintHelp(args);
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
            Console.WriteLine("REQUIRED Parameters  1 - Resource: Policy (default), Keyset ");
            Console.WriteLine("REQUIRED Parameters  2 - Command: List (default), Get, Create, Update, delete");
            Console.WriteLine("Optional Parameters  3 - Id: Policy ID, Keyset ID");
            Console.WriteLine("Optional Parameters  4 - Relative File Path: For Policy Xml File, For Keyset Json File");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("List (default)               : {0} Policy|Keyset List", appName);
            Console.WriteLine("Get                          : {0} Policy|Keyset Get [PolicyID]", appName);
            Console.WriteLine("                             : {0} Policy Get B2C_1A_PolicyName", appName);
            Console.WriteLine("                             : {0} Keyset Get B2C_1A_GoogleSecret", appName);
            Console.WriteLine("Create                       : {0} Policy|Keyset Create [RelativePathTo(XML|Json)]", appName);
            Console.WriteLine("                             : {0} Policy Create policy.xml", appName);
            Console.WriteLine("                             : {0} Keyset Create keyset.json", appName);
            Console.WriteLine("Update                       : {0} Policy|Keyset Update [PolicyID] [RelativePathToXML]", appName);
            Console.WriteLine("                             : {0} Policy|Keyset Update B2C_1A_PolicyName updatepolicy.xml", appName);
            Console.WriteLine("                             : {0} Keyset Update B2C_1A_GoogleSecret keyset.json", appName);
            Console.WriteLine("Delete                       : {0} Policy|Keyset Delete [PolicyID|KeyId]", appName);
            Console.WriteLine("                             : {0} Policy Delete B2C_1A_PolicyName", appName);
            Console.WriteLine("                             : {0} Keyset Delete B2C_1A_Secret", appName);
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
