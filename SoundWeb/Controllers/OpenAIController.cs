using BLL.DTO;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Models;
using SoundWeb.CustomClasses;

namespace SoundWeb.Controllers
{
    [LayoutByRole]
    public class OpenAIController : Controller
    {
        private readonly OpenAIService _openAIService;
        private readonly MusicFinderService _musicFinderService;

        public OpenAIController(OpenAIService openAIService, MusicFinderService musicFinderService)
        {
            _openAIService = openAIService;
            _musicFinderService = musicFinderService;
        }

        public IActionResult Find()
        {
            OpenAIDTO finder = new OpenAIDTO();
            return View(finder);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find([Bind("Question")] OpenAIDTO finder)
        {
           // if (ModelState.IsValid)
            {
                var (question, answer) = await _openAIService.GetResponseFromAI(finder.Question);
                finder.Question = question;
                finder.Answer = answer;

                if (!string.IsNullOrEmpty(finder.Answer))
                {
                    TempData["OpenAiAnswer"] = JsonConvert.SerializeObject(finder);
                    return RedirectToAction("ShowFoundOpenAIAnswer");
                }
                else
                {
                    return RedirectToAction("NotFound");
                }
            }

            return View(finder);
        }
        public IActionResult NotFound()
        {
            return View();
        }
        public IActionResult ShowFoundOpenAIAnswer()
        {
            OpenAIDTO finder = new OpenAIDTO();
            if (TempData["OpenAiAnswer"] != null)
            {
                finder = JsonConvert.DeserializeObject<OpenAIDTO>(TempData["OpenAiAnswer"].ToString());
            }
            return View(finder);
        }

        public IActionResult FindMusicWithFuzzySearch()
        {
            MusicFuzzyFinderDTO finder = new MusicFuzzyFinderDTO();
            return View(finder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FindMusicWithFuzzySearch([Bind("Name")] MusicFuzzyFinderDTO finder)
        {
            List<Music> results = _musicFinderService.FindMusicWithFuzzySearch(finder.Name);
            ViewBag.Results = results;
            return View(finder);
        }
    }
}
