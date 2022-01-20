using DomainLogic.ResultModels;
using DomainLogic.Services;
using Newtonsoft.Json;
using System;
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

        public async Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(Guid gameId)
        {
            var response = await _client.PostAsync("/game/registerSecondPlayer", new StringContent(gameId.ToString()));
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GameRegisterSecondPlayerResult>(responseString);
        }
    }
}
