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
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = await client.UploadDataTaskAsync(new Uri(HostUri + "/Game/Create"), new byte[] { });
            }

            return ToDictionary(response);
        }

        public static async Task<Dictionary<string, string>> GameCreateWithBot()
        {
            var response = default(byte[]);

            using (var client = new WebClient())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes("{}");
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                response = await client.UploadDataTaskAsync(new Uri(HostUri + "/Game/CreateWithBot"), bytes);
            }

            return ToDictionary(response);
        }

        internal static async Task<Dictionary<string, string>> GameGetInfo(string gameId)
        {
            var response = default(byte[]);

            using (var client = new WebClient())
            {
                response = await client.DownloadDataTaskAsync(HostUri + $"/Game/getInfo?gameId={gameId}");
            }

            return ToDictionary(response);
        }

        internal static async Task<JObject> GameStartWithBot(string gameId)
        {
            var response = default(byte[]);

            gameId = $"\"{gameId}\"";
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    response = await client.UploadDataTaskAsync(HostUri + $"/Game/startWithBot", Encoding.UTF8.GetBytes(gameId));
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return ToJObject(response);
        }
        internal static async Task<JObject> GameMoveWithBot(string firstPlayerCode, string previousBoardStateId, JToken moveVector)
        {
            var response = default(byte[]);

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers["playerCode"] = firstPlayerCode;
                    client.Headers["previousBoardStateId"] = previousBoardStateId;
                    response = await client.UploadDataTaskAsync(HostUri + $"/Game/moveWithBot", Encoding.UTF8.GetBytes(moveVector.ToString()));
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return ToJObject(response);
        }

        static Dictionary<string, string> ToDictionary(byte[] bytes)
        {
            var responseString = Encoding.UTF8.GetString(bytes);

            var json = JObject.Parse(responseString);

            return json
                .Properties()
                .Select(s => new KeyValuePair<string, string>(s.Name, s.Value.ToString()))
                .ToDictionary(s => s.Key, s => s.Value);
        }


        static JObject ToJObject(byte[] bytes)
        {
            var responseString = Encoding.UTF8.GetString(bytes);

            return JObject.Parse(responseString);
        }

        
    }
}
