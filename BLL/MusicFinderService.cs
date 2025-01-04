using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;

namespace BLL
{
    public class MusicFinderService
    {
        private readonly SoundContext _soundContext;

        public MusicFinderService(SoundContext soundContext)
        {
            _soundContext = soundContext;
        }

        public List<Music> FindMusic(MusicFinderDTO finder)
        {
            if (finder == null)
                return new List<Music>();

            IQueryable<Music> query = _soundContext.Musics;

            if (!string.IsNullOrEmpty(finder.Name))
            {
                query = query.Where(m => m.Name.Contains(finder.Name));
            }

            if (!string.IsNullOrEmpty(finder.Author))
            {
                query = query.Include(m => m.Author)
                             .Where(m => m.Author.Name.Contains(finder.Author));
            }

            if (!string.IsNullOrEmpty(finder.Tag))
            {
                query = query.Include(m => m.MusicTags)
                             .ThenInclude(mt => mt.Tag)
                             .Where(m => m.MusicTags.Any(mt => mt.Tag.Name.Contains(finder.Tag)));
            }

            if (!string.IsNullOrEmpty(finder.Genre))
            {
                query = query.Include(m => m.MusicGenres)
                             .ThenInclude(mg => mg.Genre)
                             .Where(m => m.MusicGenres.Any(mg => mg.Genre.Name.Contains(finder.Genre)));
            }

            return query.ToList();
        }

        public List<Music> FindMusicWithFuzzySearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return new List<Music>();

            IQueryable<Music> query = _soundContext.Musics
                .Include(m => m.Author)
                .Include(m => m.MusicTags)
                    .ThenInclude(mt => mt.Tag)
                .Include(m => m.MusicGenres)
                    .ThenInclude(mg => mg.Genre);

            var allMusic = query.ToList();

            allMusic = allMusic.Where(m =>
                Fuzz.PartialRatio(m.Name, searchQuery) > 70 ||
                (m.Author != null && Fuzz.PartialRatio(m.Author.Name, searchQuery) > 70) ||
                m.MusicTags.Any(mt => Fuzz.PartialRatio(mt.Tag.Name, searchQuery) > 70) ||
                m.MusicGenres.Any(mg => Fuzz.PartialRatio(mg.Genre.Name, searchQuery) > 70)
            ).ToList();

            return allMusic;
        }
    }

}