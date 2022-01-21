using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Шашки
{
    static class BackendService
    {
        static string HostUri => ConfigurationManager.AppSettings["backendHost"];
        public static async Task<Dictionary<string, string>> GameCreate() 
        {
            var response = default(byte[]);

            using (var client = new WebClient())
            {
                response = await client.UploadDataTaskAsync(new Uri(HostUri + "/Game/Create"), new byte[] { });
            }

            var responseString = Encoding.UTF8.GetString(response);

            var json = JObject.Parse(responseString);

            return json
                .Properties()
                .Select(s => new KeyValuePair<string, string>(s.Name, s.Value.ToString()))
                .ToDictionary(s => s.Key, s => s.Value);
        }

        public static async Task<Dictionary<string, string>> GameCreateWithBot()
        {
            var response = default(byte[]);

            using (var client = new WebClient())
            {
                response = await client.UploadDataTaskAsync(new Uri(HostUri + "/Game/CreateWithBot"), new byte[] { });
            }

            var responseString = Encoding.UTF8.GetString(response);

            var json = JObject.Parse(responseString);

            return json
                .Properties()
                .Select(s => new KeyValuePair<string, string>(s.Name, s.Value.ToString()))
                .ToDictionary(s => s.Key, s => s.Value);
        }

        internal static async Task<Dictionary<string, string>> GameGetInfo(string gameId)
        {
            var response = default(byte[]);

            using (var client = new WebClient())
            {
                response = await client.DownloadDataTaskAsync(HostUri + $"/Game/getInfo?gameId={gameId}");
            }

            var responseString = Encoding.UTF8.GetString(response);

            var json = JObject.Parse(responseString);

            return json
                .Properties()
                .Select(s => new KeyValuePair<string, string>(s.Name, s.Value.ToString()))
                .ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
