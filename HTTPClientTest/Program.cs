using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HTTPClientTest
{
    /// <summary>
    /// This tiny test should showcase the need of try/catch around HttpClient.GetStringAsync
    /// </summary>
    class Program
    {
        private const string url = "http://google.com/unknown";

		static void Main(string[] args)
        {
            MainAsync(args).Wait();

            Console.WriteLine("\n\n\n=====> end");
            Console.ReadKey();
        }


        #region using HttpClient.GetStringAsync
        static async Task MainAsync(string[] args)
        {
            Console.WriteLine(url);

            await MakeHttpCallAsync_Safe();
            await MakeHttpCallAsync_Unsafe();

            await MakeHttpCallAsync_HttpRequestMessage_Safe();
            await MakeHttpCallAsync_HttpRequestMessage_Unsafe();
        }

        private static async Task<string> MakeHttpCallAsync_Unsafe()
        {
            Console.WriteLine("making unsafe call");

            var ret = string.Empty;

            using (var client = new HttpClient())
            {
                var responseHtml = await client.GetStringAsync(url);

                ret = responseHtml;

            }

            Console.WriteLine(ret);
            return ret;
        }


        private static async Task<string> MakeHttpCallAsync_Safe()
        {
            Console.WriteLine("Making safe call");

            var ret = string.Empty;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                try {
                    var responseHtml = await client.GetStringAsync(url);

                    ret = responseHtml;
                }
                catch (HttpRequestException ex) {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine(ret);
            return ret;
        }
        #endregion //   using HttpClient.GetStringAsync


        #region using HttpResponseMessage.Content.ReadAsString
        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Unsafe()
        {
            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString unsafe call");

            var ret = string.Empty;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);
                var responseHtml = await response.Content.ReadAsStringAsync();

                ret = responseHtml;

            }

            Console.WriteLine(ret);
            return ret;
        }


        private static async Task<string> MakeHttpCallAsync_HttpRequestMessage_Safe()
        {
            Console.WriteLine("making HttpResponseMessage.Content.ReadAsString unsafe call");

            var ret = string.Empty;

            using (var client = new HttpClient())
            {
				var request = new HttpRequestMessage(HttpMethod.Get, url);

                try
                {
                    var response = await client.SendAsync(request);
                    var responseHtml = await response.Content.ReadAsStringAsync();

                    ret = responseHtml;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine(ret);
            return ret;
        }
        #endregion //   using HttpResponseMessage.Content.ReadAsString
    }
}
