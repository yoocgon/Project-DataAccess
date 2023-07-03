
using System.Net;


namespace KCureDataAccess
{
    public class CustomHttpClient
    {
        private HttpClient client;

        public CustomHttpClient() 
        {
            client = new HttpClient();
        }

        public async void RequestGet(string url)
        {
            try
            {
                using (var response = await client.GetAsync(url))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(body);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception={ex.Message}");
            }
        }

        public async void RequestPost(string url, string strJson)
        {
            try
            {
                StringContent httpContent = new StringContent(strJson, System.Text.Encoding.UTF8, "application/json");
                using (var response = await client.PostAsync(url, httpContent))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(body);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception={ex.Message}");
            }
        }
    }
}
