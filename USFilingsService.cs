using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using FundXWebBlzr.Models;



namespace FundXWebBlzr.Services
{
    public class USFilingsService
    {
        public HttpClient _httpClient;

        public USFilingsService(HttpClient client)
        {
            _httpClient = client;
        }


        public async Task<List<Val>> GetValsAsync(string tkr, string tag, short rYr)
        {
            var response = await _httpClient.GetAsync($"api/v1/us/std/items/{tag}/fiscal/yr/{rYr}/{tkr}?tkn=0");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Val>>(responseContent);
        }

        public async Task<List<Co>> GetCosAsync(string nTry)
        {
            var response = await _httpClient.GetAsync($"api/v1/us/rep/cos/try/{nTry}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Co>>(responseContent);
        }

        

        public async Task<ToDo> GetToDoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"todos/{id}");
            response.EnsureSuccessStatusCode();

            using var responseContent = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ToDo>(responseContent);
        }
    }
}
