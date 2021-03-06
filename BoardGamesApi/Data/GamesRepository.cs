﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BoardGamesApi.Models;

namespace BoardGamesApi.Data
{
    public class GamesRepository : IGamesRepository
    {
        private IList<Game> _games;

        public PagedList<Game> GetPage(int page = 1, int pageSize = 10)
        {
            var games = GetGames();

            return new PagedList<Game>
            {
                Items = games.Skip((page - 1) * pageSize).Take(pageSize),
                Page = page,
                PageSize = pageSize,
                TotalCount = games.Count
            };
        }

        public Game GetById(string id)
        {
            var games = GetGames();

            return games.FirstOrDefault(x => x.Id == id);
        }

        public void Create(Game game)
        {
            game.Id = GetNextId();
            GetGames().Add(game);
        }

        public void Delete(string id)
        {
            var games = GetGames();
            var gameToDelete = games.First(x => x.Id == id);

            games.Remove(gameToDelete);
        }

        public void Update(Game game)
        {
            var games = GetGames();
            var gameToUpdate = games.First(x => x.Id == game.Id);

            games.Remove(gameToUpdate);
            games.Add(game);
        }

        private IList<Game> GetGames()
        {
            if (_games == null)
            {
                var assembly = typeof(GamesRepository).Assembly;

                using var resourceStream = assembly.GetManifestResourceStream("BoardGamesApi.Data.games.json");
                using var reader = new StreamReader(resourceStream, Encoding.UTF8);

                var json = reader.ReadToEnd();
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                _games = JsonSerializer.Deserialize<IList<Game>>(json, options);
            }

            return _games;
        }

        private string GetNextId()
        {
            var maxId = GetGames().Select(x => int.Parse(x.Id.Split("-")[1])).Max();
            return $"gam-{maxId + 1:D6}";
        }
    }
}
