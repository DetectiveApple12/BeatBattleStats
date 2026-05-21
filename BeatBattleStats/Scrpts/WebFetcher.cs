using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace BeatBattleStats.Scrpts
{
    public class WebFetcher
    {
        public HttpClient client;
        public static string baseURL = "https://beat-battle.net/profile/{user}";

        public WebFetcher()
        {
            client = new();
            
        }
        
        public async Task<string> FetchProfileHTML(string user)
        {
            var response = await client.GetAsync(baseURL.Replace("{user}", user));
            var text = await response.Content.ReadAsStringAsync();
            return text;
        }
    }
}
