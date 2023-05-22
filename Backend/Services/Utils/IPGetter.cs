
namespace Services.Utils
{
    // AWS EC2 instance public ip can change
    public static class IPGetter
    {
        private static string ApiUrl = "https://api.ipify.org";
        private static string Ip;

        public static async Task<string> GetPublicIPAsync()
        {
            if (string.IsNullOrEmpty(Ip))
            {
                Ip = await GetIpAsync();
                return Ip;
            }
            return Ip;
        }

        private static async Task<string> GetIpAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(ApiUrl);
                return response.Trim();
            }
        }
    }
}
