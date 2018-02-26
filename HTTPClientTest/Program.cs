using System;using System.Collections.Generic;
using System.IO;using System.Net;using System.Net.Http;using System.Threading.Tasks;namespace HTTPClientTest{    /// <summary>    /// This tiny test should showcase the need of try/catch around HttpClient.GetStringAsync    /// </summary>    class Program    {        private const string url = "http://google.com/unknown";        static void Main(string[] args)        {            MainAsync(args).Wait();            Console.WriteLine("\n\n\n=====> end");            Console.ReadKey();        }        static async Task MainAsync(string[] args)        {            Console.WriteLine(url);                        try
            {
                PrintResponse(await MakeHttpCallAsync_Safe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }            Console.WriteLine("###################################################\n\n\n");            try
            {
                PrintResponse( await MakeHttpCallAsync_Unsafe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }            Console.WriteLine("###################################################\n\n\n");            try
            {
                PrintResponse(await MakeHttpCallAsync_HttpRequestMessage_Safe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCallAsync_HttpRequestMessage_Unsafe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCall_WebRequest_Safe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }
            Console.WriteLine("###################################################\n\n\n");


            try
            {
                PrintResponse(await MakeHttpCall_WebRequest_Unsafe());
            }            catch (Exception ex)
            {
                PrintEx(ex);
            }
        }


        #region using HttpClient.GetStringAsync        private static async Task<string> MakeHttpCallAsync_Unsafe()        {            Console.WriteLine("Making HttpClient.GetStringAsync unsafe call");            var ret = string.Empty;            using (var client = new HttpClient())            {
                //  exception should be thrown here
                var responseHtml = await client.GetStringAsync(url);                ret = responseHtml;            }            Console.WriteLine(ret);            return ret;        }        private static async Task<string> MakeHttpCallAsync_Safe()        {            Console.WriteLine("Making HttpClient.GetStringAsync safe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                var request = new HttpRequestMessage(HttpMethod.Get, url);                try                {
                    //  exception should be thrown here
                    var responseHtml = await client.GetStringAsync(url);                    ret = responseHtml;                }                catch (HttpRequestException ex)                {                    PrintEx(ex, true);                }            }            return ret;        }        #endregion //   using HttpClient.GetStringAsync        #region using HttpResponseMessage.Content.ReadAsString        //  should cause an exception        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Unsafe()        {            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString unsafe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    //  exception should be thrown here --> but isn't
                    //  so we don't need a try/catch block here
                    var response = await client.SendAsync(request);                    var responseHtml = await response.Content.ReadAsStringAsync();                    ret = responseHtml;
                }            }            return ret;        }        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Safe()        {            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString safe call");            var ret = string.Empty;            using (var client = new HttpClient())            {                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    try
                    {
                        //  exception should be thrown here --> but isn't
                        //  so we can safely leave remove this try/catch block
                        var response = await client.SendAsync(request);
                        var responseHtml = await response.Content.ReadAsStringAsync();

                        ret = responseHtml;
                    }
                    catch (HttpRequestException ex)
                    {
                        PrintEx(ex, true);
                    }
                }                }            return ret;        }        #endregion //   using HttpResponseMessage.Content.ReadAsString		        #region WebRequest        private static async Task<string> MakeHttpCall_WebRequest_Unsafe()        {            Console.WriteLine("making WebRequest unsafe call");            var ret = string.Empty;                        var request = WebRequest.CreateHttp(url);
            //  exception should be thrown here
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
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
            if (!cought)
                PrintWeDied();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(ex);
            Console.ForegroundColor = color;
        }        private static void PrintResponse(string response)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(response);
            Console.ForegroundColor = color;
        }
        #endregion //   helper    }}