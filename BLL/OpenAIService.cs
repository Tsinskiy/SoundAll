using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Models;
using System.Collections.Generic;

namespace BLL
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly MusicFinderService _musicFinderService;
        private readonly SoundContext _soundContext;

        public OpenAIService(HttpClient httpClient, string apiKey, MusicFinderService musicFinderService, SoundContext soundContext)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _musicFinderService = musicFinderService;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _soundContext = soundContext;
        }

        public async Task<(string question, string answer)> GetResponseFromAI(string prompt)
        {
            var musicList = _soundContext.Musics.ToList();
            var musicNames = musicList.Select(m => m.Name).ToList();
            var musicNamesString = string.Join(", ", musicNames);
            var enhancedPrompt = $"{prompt}\n\n  Give response with language used before this text. You are a helpful audio assistant. Choose music only from this list: {musicNamesString}. Start with the phrase  'Here is the list of music that suits your needs best.' in appropriate language";
            var data = new
            {
                model = "gpt-4", // Updated model name
                messages = new[]
                {
            new
            {
                role = "user",
                content = enhancedPrompt
            }
        },
                max_tokens = 500 // Adjust this as needed
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from OpenAI API: " + responseString);
                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;

                    var choices = root.GetProperty("choices");
                    if (choices.GetArrayLength() > 0)
                    {
                        var message = choices[0].GetProperty("message");
                        var contentProperty = message.GetProperty("content");

                        return (prompt, contentProperty.GetString().Trim());
                    }
                }
                Console.Error.WriteLine("Invalid response structure from OpenAI API: " + responseString);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception occurred while calling OpenAI API: " + ex.Message);
            }

            return (prompt, string.Empty);
        }


        public List<Music> FindMusicInDatabase(MusicFinderDTO finder)
        {
            return _musicFinderService.FindMusic(finder);
        }

        public async Task<string> FindMusicUsingOpenAI(string prompt)
        {


           
            var (_, answer) = await GetResponseFromAI(prompt);
            return answer;
        }


    }
}
