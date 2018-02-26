using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HTTPClientTest
{
    /// <summary>
    /// This tiny test should showcase the need of try/catch around HttpClient.SendAsync and HttpResponseMessage.Content.Read
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


        static async Task MainAsync(string[] args)
        {
            Console.WriteLine(url);

            await MakeHttpCallAsync_Safe();

            await MakeHttpCallAsync_Unsafe();
        }

        private static async Task<string> MakeHttpCallAsync_Unsafe()
        {
            Console.WriteLine("making unsafe call");

            var ret = string.Empty;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

				ret = json;

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
					var response = await client.SendAsync(request);
                    var responseHtml = await response.Content.ReadAsStringAsync();
                    ret = responseHtml;
                }
                catch (HttpRequestException ex) {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine(ret);
            return ret;
        }
    }
}
