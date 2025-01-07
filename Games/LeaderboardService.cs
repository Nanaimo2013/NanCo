using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Octokit;
using NanCo;

namespace NanCo.Games
{
    public class LeaderboardService
    {
        private readonly GitHubClient github;
        private const string GIST_ID = "cecaa226a1d3fd55754c96a9e760c5aa";

        public LeaderboardService()
        {
            github = new GitHubClient(new ProductHeaderValue("NanCo-Leaderboard"));
            github.Credentials = new Credentials(Config.GITHUB_TOKEN);
        }

        public async Task<string> GetLeaderboard()
        {
            try
            {
                var gist = await github.Gist.Get(GIST_ID);
                return gist.Files.Values.First().Content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching leaderboard: {ex.Message}");
                return "Error loading leaderboard";
            }
        }

        public async Task UpdateLeaderboard(string gameName, string playerName, int score)
        {
            try
            {
                var currentContent = await GetLeaderboard();
                var scores = ParseLeaderboard(currentContent);
                UpdateScores(scores, gameName, playerName, score);
                var newContent = FormatLeaderboard(scores);

                var gistUpdate = new GistUpdate();
                gistUpdate.Files.Add("leaderboard.txt", new GistFileUpdate { Content = newContent });
                await github.Gist.Edit(GIST_ID, gistUpdate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating leaderboard: {ex.Message}");
            }
        }

        public async Task<string> GetFormattedLeaderboard()
        {
            try
            {
                var content = await GetLeaderboard();
                var scores = ParseLeaderboard(content);
                return FormatLeaderboardDisplay(scores);
            }
            catch (Exception ex)
            {
                return $"Error loading leaderboard: {ex.Message}";
            }
        }

        private Dictionary<string, Dictionary<string, int>> ParseLeaderboard(string content)
        {
            var scores = new Dictionary<string, Dictionary<string, int>>();
            
            if (string.IsNullOrEmpty(content)) return scores;
            
            var lines = content.Split('\n');
            string currentGame = "";
            
            foreach (var line in lines)
            {
                if (line.EndsWith(":"))
                {
                    currentGame = line.TrimEnd(':');
                    scores[currentGame] = new Dictionary<string, int>();
                }
                else if (!string.IsNullOrWhiteSpace(line) && line.Contains(":"))
                {
                    var parts = line.Trim().Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int score))
                    {
                        scores[currentGame][parts[0].Trim()] = score;
                    }
                }
            }
            return scores;
        }

        private void UpdateScores(Dictionary<string, Dictionary<string, int>> scores, string game, string player, int score)
        {
            if (!scores.ContainsKey(game))
                scores[game] = new Dictionary<string, int>();

            if (!scores[game].ContainsKey(player) || scores[game][player] < score)
                scores[game][player] = score;
        }

        private string FormatLeaderboard(Dictionary<string, Dictionary<string, int>> scores)
        {
            // Format the scores into a string
            return string.Join("\n", scores.Select(game =>
                $"{game.Key}:\n" + string.Join("\n", 
                    game.Value.OrderByDescending(s => s.Value)
                        .Select(s => $"  {s.Key}: {s.Value}")
                )
            ));
        }

        private string FormatLeaderboardDisplay(Dictionary<string, Dictionary<string, int>> scores)
        {
            var output = "\n=== LEADERBOARD ===\n";
            foreach (var game in scores)
            {
                output += $"\n{game.Key}\n";
                output += "---------------\n";
                var topPlayers = game.Value
                    .OrderByDescending(s => s.Value)
                    .Take(5)
                    .Select((s, i) => $"{i + 1}. {s.Key}: {s.Value}");
                output += string.Join("\n", topPlayers);
                output += "\n";
            }
            return output;
        }
    }
} 