using System;using System.Collections.Generic;
using System.IO;using System.Net;using System.Net.Http;using System.Threading.Tasks;namespace HTTPClientTest{






    /// <summary>    /// This tiny test should showcase the need of try/catch around HttpClient.GetStringAsync    /// </summary>    class Program    {        private const string url = "http://google.com/unknown";        static void Main(string[] args)        {            MainAsync(args).Wait();            Console.WriteLine("\n\n\n=====> end");            Console.ReadKey();        }        static async Task MainAsync(string[] args)        {            Console.WriteLine(url);


            try
            {
                PrintResponse(await MakeHttpCallAsync_Safe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }            Console.WriteLine("###################################################\n\n\n");            try
            {
                PrintResponse(await MakeHttpCallAsync_Unsafe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }            Console.WriteLine("###################################################\n\n\n");            try
            {
                PrintResponse(await MakeHttpCallAsync_HttpRequestMessage_Safe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCallAsync_HttpRequestMessage_Unsafe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCall_WebRequest_Safe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCall_WebRequest_Unsafe());
            }            catch (Exception ex)
            {
                PrintMisingStatusCodeMsg();
                PrintEx(ex);
            }            Console.WriteLine("###################################################\n\n\n");            try            {                PrintResponse(await MakeHttpCall_ClientGetAsync_Safe());            }            catch (Exception ex)            {                PrintMisingStatusCodeMsg();                PrintEx(ex);            }            Console.WriteLine("###################################################\n\n\n");            try            {                PrintResponse(await MakeHttpCall_ClientGetAsync_Unsafe());            }            catch (Exception ex)            {                PrintMisingStatusCodeMsg();                PrintEx(ex);            }
        }




        #region using HttpClient.GetStringAsync        private static async Task<string> MakeHttpCallAsync_Unsafe()        {            Console.WriteLine("Making HttpClient.GetStringAsync unsafe call");            var ret = string.Empty;            using (var client = new HttpClient())            {
                //  exception should be thrown here
                var responseHtml = await client.GetStringAsync(url);                PrintMisingStatusCodeMsg();                ret = responseHtml;            }            return ret;        }        private static async Task<string> MakeHttpCallAsync_Safe()        {            Console.WriteLine("Making HttpClient.GetStringAsync safe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                var request = new HttpRequestMessage(HttpMethod.Get, url);                try                {
                    //  exception should be thrown here
                    var responseHtml = await client.GetStringAsync(url);                    PrintMisingStatusCodeMsg();                    ret = responseHtml;                }                catch (HttpRequestException ex)                {                    PrintMisingStatusCodeMsg();                    PrintEx(ex, true);                }            }            return ret;        }
        #endregion //   using HttpClient.GetStringAsync

        #region using HttpResponseMessage.Content.ReadAsString        //  should cause an exception
        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Unsafe()        {            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString unsafe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    //  exception should be thrown here --> but isn't
                    //  so we don't need a try/catch block here
                    var response = await client.SendAsync(request);                    var responseHtml = await response.Content.ReadAsStringAsync();
                    PrintStatusCode(response.StatusCode, response.IsSuccessStatusCode);                    ret = responseHtml;
                }            }            return ret;        }        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Safe()        {            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString safe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    try
                    {
                        //  exception should be thrown here --> but isn't
                        //  so we can safely leave remove this try/catch block
                        var response = await client.SendAsync(request);
                        var responseHtml = await response.Content.ReadAsStringAsync();

                        PrintStatusCode(response.StatusCode, response.IsSuccessStatusCode);

                        ret = responseHtml;
                    }
                    catch (HttpRequestException ex)
                    {
                        PrintEx(ex, true);
                    }
                }
            }            return ret;        }
        #endregion //   using HttpResponseMessage.Content.ReadAsString


        #region using HttpClient.GetAsync()        //  actually this is my favorite version...        //  doesn't throw exceptions, full access to the status code and        //  easy (proper!) success check        private static async Task<string> MakeHttpCall_ClientGetAsync_Unsafe()        {            Console.WriteLine("making HttpClient.GetAsync unsafe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var response = await client.GetAsync(url))                {                    PrintStatusCode(response.StatusCode, response.IsSuccessStatusCode);                    //  exception might be thrown --> nopes... all good here                    var responseString = await response.Content.ReadAsStringAsync();                    ret = responseString;                }            }            return ret;        }        private static async Task<string> MakeHttpCall_ClientGetAsync_Safe()        {            Console.WriteLine("making HttpClient.GetAsync safe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var response = await client.GetAsync(url))                {                    PrintStatusCode(response.StatusCode, response.IsSuccessStatusCode);                    //  exit in case of not success                    if (!response.IsSuccessStatusCode)                    {                        return ret;                    }                    var responseString = await response.Content.ReadAsStringAsync();                    ret = responseString;                }            }            return ret;        }
        #endregion  //  using HttpClient.GetAsync()
        #region WebRequest        private static async Task<string> MakeHttpCall_WebRequest_Unsafe()        {            Console.WriteLine("making WebRequest unsafe call");            var ret = string.Empty;                        var request = WebRequest.CreateHttp(url);
            //  exception should be thrown here
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                PrintStatusCode(response.StatusCode, response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous);
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        string responseFromServer = reader.ReadToEnd();

                        ret = responseFromServer;
                    }
                }
            }
            return ret;        }        private static async Task<string> MakeHttpCall_WebRequest_Safe()        {            Console.WriteLine("making WebRequest safe call");            var ret = string.Empty;

            var request = WebRequest.CreateHttp(url);            try
            {
                //  exception should be thrown here
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    PrintStatusCode(response.StatusCode, response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous);
                    using (var dataStream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            string responseFromServer = reader.ReadToEnd();

                            ret = responseFromServer;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                PrintStatusCode(ex.Status);
                PrintEx(ex, true);
            }            return ret;        }        #endregion //   WebRequest
        #region helper        private static void PrintWeDied()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("We died!");
            Console.ForegroundColor = color;
        }        private static void PrintEx(Exception ex, bool cought = false)
        {
            var newColor = ConsoleColor.Yellow;
            if (!cought)
            {
                PrintWeDied();
                newColor = ConsoleColor.DarkRed;
            }

            var color = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
            Console.WriteLine(ex);
            Console.ForegroundColor = color;
        }        private static void PrintResponse(string response)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(response);
            Console.ForegroundColor = color;
        }

        private static void PrintStatusCode(HttpStatusCode statusCode, bool success)
        {
            var color = Console.ForegroundColor;
            if (success)
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            else
                Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(String.Format("Status code was: {0}", statusCode));
            Console.ForegroundColor = color;
        }

        private static void PrintStatusCode(WebExceptionStatus statusCode)
        {
            var color = Console.ForegroundColor;
            if (statusCode == WebExceptionStatus.Success)
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            else
                Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(String.Format("Status code was: {0}", statusCode));
            Console.ForegroundColor = color;
        }



        private static void PrintMisingStatusCodeMsg()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("No way to get the status code.");
            Console.ForegroundColor = color;
        }
        #endregion //   helper    }}