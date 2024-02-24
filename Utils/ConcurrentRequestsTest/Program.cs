using System.Collections.Concurrent;

namespace ConcurrentRequestsTest
{
    internal class Program
    {
        private static ConcurrentDictionary<int, int> uniqueSeqIdDict = new ConcurrentDictionary<int, int>();
        private static int concurrentRequests = 100; //number of concurrent requests to be triggered <MODIFY_AS_PER_REQUIREMENT>
        private static string baseUrl = "https://localhost:7052/api/getId"; //loadbalancer url <MODIFY_AS_PER_REQUIREMENT>
        static async Task Main(string[] args)
        {
            
            var tasks = new List<Task>();

            using (var client = new HttpClient())
            {
                for (int i = 0; i < concurrentRequests; i++)
                    tasks.Add(SendRequestAsync(client, baseUrl));

                await Task.WhenAll(tasks);
            }

            Console.WriteLine("Process completed!");
            Console.Read();
        }

        private static async Task SendRequestAsync(HttpClient client, string baseUrl)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(baseUrl);

                if(response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    int respData = Convert.ToInt32(responseBody);

                    if (!uniqueSeqIdDict.TryAdd(respData,1))
                        Console.WriteLine("Duplicate record found ************************************* " + respData);
                    else
                        Console.WriteLine("response: " + respData);
                }
                else
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
