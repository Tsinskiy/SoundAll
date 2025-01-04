using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using BLL;
using Microsoft.SqlServer.Server;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SoundWeb.CustomClasses;
using Microsoft.AspNetCore.Authorization;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Dsp;
using DAL.ModelsExt;

namespace SoundWeb.Controllers
{

    [LayoutByRole]
    public class MusicsController : Controller
    {
        private readonly SoundContext _context;
        MediaService _mediaService;

        public MusicsController(SoundContext context, MediaService mediaService)
        {
            _context = context;
            _mediaService = mediaService;   
        }

        // GET: Musics
        public IActionResult Index(int? genreId, int? tagId)
        {
            var genres = _context.Genres.ToList();
            var tags = _context.Tags.ToList();

            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            ViewBag.Tags = new SelectList(tags, "Id", "Name");

            var musicQuery = _context.Musics.AsQueryable();

            if (genreId.HasValue)
            {
                var musicGenres = _context.MusicGenres.Where(mg => mg.GenreId == genreId.Value).Select(mg => mg.MusicId).ToList();
                musicQuery = musicQuery.Where(m => musicGenres.Contains(m.Id));
            }

            if (tagId.HasValue)
            {
                var musicTags = _context.MusicTags.Where(mt => mt.TagId == tagId.Value).Select(mt => mt.MusicId).ToList();
                musicQuery = musicQuery.Where(m => musicTags.Contains(m.Id));
            }

            var music = musicQuery.ToList();

            return View(music);
        }

        // GET: Musics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("MusicId", (int)id);
            var media = await _context.Media.FirstOrDefaultAsync(m => m.MusicId == id);
            ViewBag.Media = media;

            return View(music);
        }

        // GET: Musics/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Musics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Music music)
        {
            if (ModelState.IsValid)
            {
                _context.Add(music);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(music);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {

            int musicId = (int)HttpContext.Session.GetInt32("MusicId");

            if (uploadedFile != null)
            {
                await _mediaService.AddMusicFileAsync(musicId, uploadedFile, "mp3");  
                return View("Success");
            }

            return View("Error");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Download(int musicId)
        {
            var musicFile = await _mediaService.GetMusicFileAsync(musicId);
            if (musicFile == null)
            {
                return NotFound();
            }

            var content = new MemoryStream(musicFile.Data);
            var contentType = "audio/mpeg"; 
            var fileName = "musicfile.mp3"; 

            return File(content, contentType, fileName);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditMusic()
        {
            int musicId = (int)HttpContext.Session.GetInt32("MusicId");
            var model = new EditMusicViewModel { MusicId = musicId };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditMusic(EditMusicViewModel model)
        {
            var musicFile = await _mediaService.GetMusicFileAsync(model.MusicId);
            if (musicFile == null)
            {
                return NotFound();
            }

            using (var ms = new MemoryStream(musicFile.Data))
            using (var reader = new Mp3FileReader(ms))
            {
                ISampleProvider sampleProvider = reader.ToSampleProvider();

                if (model.Volume != 1.0f)
                {
                    sampleProvider = new VolumeSampleProvider(sampleProvider) { Volume = model.Volume };
                }

                if (model.StartOffset > 0)
                {
                    sampleProvider = new OffsetSampleProvider(sampleProvider) { SkipOver = TimeSpan.FromSeconds(model.StartOffset) };
                }

                if (model.EndOffset > 0)
                {
                    sampleProvider = new OffsetSampleProvider(sampleProvider) { Take = TimeSpan.FromSeconds(model.EndOffset) };
                }


                if (model.PitchFactor != 1.0f)
                {
                    sampleProvider = new SmbPitchShiftingSampleProvider(sampleProvider, 512, 4096, model.PitchFactor);
                }

                if (model.FadeInDuration > 0 || model.FadeOutDuration > 0)
                {
                    var fadeInOut = new FadeInOutSampleProvider(sampleProvider, true);
                    if (model.FadeInDuration > 0)
                    {
                        fadeInOut.BeginFadeIn(model.FadeInDuration);
                    }
                    if (model.FadeOutDuration > 0)
                    {
                        fadeInOut.BeginFadeOut(model.FadeOutDuration);
                    }
                    sampleProvider = fadeInOut;
                }

                //using (var outputStream = new MemoryStream())
                //{
                //    WaveFileWriter.WriteWavFileToStream(outputStream, (IWaveProvider)sampleProvider);
                //    outputStream.Position = 0;
                //    return File(outputStream.ToArray(), "audio/wav", "edited_music.wav");
                //}


                var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.Channels);
                var resampler = new WdlResamplingSampleProvider(sampleProvider, waveFormat.SampleRate);

                using (var outputStream = new MemoryStream())
                {
                    WaveFileWriter.WriteWavFileToStream(outputStream, resampler.ToWaveProvider());
                    outputStream.Position = 0;
                    return File(outputStream.ToArray(), "audio/wav", "edited_music.wav");
                }
            }
        }




        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            int musicId = (int)HttpContext.Session.GetInt32("MusicId");

            if (imageFile == null || imageFile.Length == 0)
            {
                return View("Error");
            }
            await _mediaService.UploadPictureAsync(musicId, imageFile);
            return View("Success");

        }

        
        public async Task<IActionResult> GetMusicFile(int id)
        {
            var musicFile = await _context.Media.Where(m=>m.MusicId == id).FirstOrDefaultAsync();
            if (musicFile == null)
            {
                return NotFound();
            }
            return File(musicFile.Data, "audio/mpeg");
        }


        [Authorize(Roles = "Admin")]
        // GET: Musics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }
            return View(music);
        }

        // POST: Musics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Music music)
        {
            if (id != music.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(music);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(music.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(music);
        }

        // GET: Musics/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // POST: Musics/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var music = await _context.Musics.FindAsync(id);
            if (music != null)
            {
                _context.Musics.Remove(music);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(int id)
        {
            return _context.Musics.Any(e => e.Id == id);
        }
    }
}
