using DomainLogic.Models;
using DomainLogic.ResultModels;
using DomainLogic.Services;
using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Implementations.Mq
{
    public class GameServiceHttpClient: IGameServiceClient
    {
        private readonly HttpClient _client;

        public GameServiceHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<GameGetInfoResult> GetInfo(Guid gameId)
        {
            var response = await _client.GetAsync("/Game/getInfo?gameId=" + gameId);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GameGetInfoResult>(responseString);
            }
            else
                throw new Exception();
        }

        public async Task<object> MakeMove(MoveVector move, Guid playerCode)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"/Game/move"),
                Content = new StringContent(JsonSerializer.Serialize(move), System.Text.Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("playerCode", playerCode.ToString());
            var response = await _client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(responseString);
            }
            else
                throw new Exception();
        }

        public async Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(Guid gameId)
        {
            var stringBody = $"\"{gameId}\"";
            var response = await _client.PostAsync("/Game/registerSecondPlayer", new StringContent(stringBody, System.Text.Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GameRegisterSecondPlayerResult>(responseString);
            }
            else
                throw new Exception();
        }
    }
}
