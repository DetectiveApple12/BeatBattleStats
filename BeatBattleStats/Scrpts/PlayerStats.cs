using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BeatBattleStats.Scrpts
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string AvatarUrl { get; set; }
        public string JoinedDate { get; set; }
    }

    public class Stats
    {
        public QuickBattleStats QuickBattle { get; set; }
        public RankedStats Ranked { get; set; }
    }

    public class QuickBattleStats
    {
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
    }

    public class RankedStats
    {
        // Empty for now, expand later if needed
    }

    public class Comment
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
    }

    public class ProfileData
    {
        public UserInfo UserInfo { get; set; }
        public string Bio { get; set; }
        public Stats Stats { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class BeatBattleProfileParser
    {
        public static ProfileData Parse(string html)
        {
            var userInfo = ExtractUserInfo(html);
            var bio = ExtractBio(html);
            var stats = ExtractStats(html);
            var comments = ExtractComments(html);

            return new ProfileData
            {
                UserInfo = userInfo,
                Bio = bio,
                Stats = stats,
                Comments = comments
            };
        }

        private static UserInfo ExtractUserInfo(string html)
        {
            var profileNameMatch = Regex.Match(html, @"""profileName"":""([^""]+)""");
            var userIdMatch = Regex.Match(html, @"""userId"":""([^""]+)""");
            var avatarMatch = Regex.Match(html, @"""profileAvatarUrl"":""([^""]+)""");
            var joinedMatch = Regex.Match(html, @"""joinedAt"":""([^""]+)""");

            return new UserInfo
            {
                Username = profileNameMatch.Success ? profileNameMatch.Groups[1].Value : "unknown",
                UserId = userIdMatch.Success ? userIdMatch.Groups[1].Value : "unknown",
                AvatarUrl = avatarMatch.Success ? avatarMatch.Groups[1].Value : "",
                JoinedDate = joinedMatch.Success ? joinedMatch.Groups[1].Value : "unknown"
            };
        }

        private static string ExtractBio(string html)
        {
            var bioMatch = Regex.Match(html, @"""initialBio"":""([^""]+)""");
            return bioMatch.Success ? bioMatch.Groups[1].Value : "";
        }

        private static Stats ExtractStats(string html)
        {
            var winsMatch = Regex.Match(html, @"""wins"":(\d+)");
            var playedMatch = Regex.Match(html, @"""gamesPlayed"":(\d+)");

            return new Stats
            {
                QuickBattle = new QuickBattleStats
                {
                    Wins = winsMatch.Success ? int.Parse(winsMatch.Groups[1].Value) : 0,
                    GamesPlayed = playedMatch.Success ? int.Parse(playedMatch.Groups[1].Value) : 0
                },
                Ranked = new RankedStats()
            };
        }

        private static List<Comment> ExtractComments(string html)
        {
            var comments = new List<Comment>();
            var jsonMatch = Regex.Match(html, @"""initialComments""\s*:\s*(\[.*?\])", RegexOptions.Singleline);

            if (!jsonMatch.Success)
                return comments;

            try
            {
                var jsonText = jsonMatch.Groups[1].Value;
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var parsed = JsonSerializer.Deserialize<List<JsonElement>>(jsonText, options);

                if (parsed != null)
                {
                    foreach (var item in parsed)
                    {
                        comments.Add(new Comment
                        {
                            Id = item.GetProperty("id").GetString(),
                            AuthorId = item.GetProperty("authorId").GetString(),
                            AuthorName = item.GetProperty("authorName").GetString(),
                            AuthorAvatarUrl = item.GetProperty("authorAvatarUrl").GetString(),
                            Content = item.GetProperty("content").GetString(),
                            CreatedAt = item.GetProperty("createdAt").GetString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to parse comments: {ex.Message}");
            }

            return comments;
        }
    }
}
