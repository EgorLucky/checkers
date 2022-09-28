using System.Text.Json.Serialization;
using System.Text.Json;
using DomainLogic.Services;
using DomainLogic.ResultModels;
using DomainLogic.Models;

namespace Implementations.GameServiceHttpClient
{
    public class GameServiceHttpClient : IGameServiceClient
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

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                options.Converters.Add(new JsonStringEnumConverter());
                return JsonSerializer.Deserialize<GameGetInfoResult>(responseString, options);
            }
            else
                throw new Exception();
        }

        public async Task<MoveResult> MakeMove(MoveVector move, Guid playerCode, Guid previousBoardStateId)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_client.BaseAddress}Game/move"),
                Content = new StringContent(JsonSerializer.Serialize(move), System.Text.Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("playerCode", playerCode.ToString());
            httpRequest.Headers.Add("previousBoardStateId", previousBoardStateId.ToString());
            var response = await _client.SendAsync(httpRequest);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                options.Converters.Add(new JsonStringEnumConverter());
                return JsonSerializer.Deserialize<MoveResult>(responseString, options);
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
                return JsonSerializer.Deserialize<GameRegisterSecondPlayerResult>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            else
                throw new Exception();
        }
    }
}