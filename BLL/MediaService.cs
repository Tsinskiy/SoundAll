using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL
{
    public class MediaService
    {
        SoundContext _soundContext;

        public MediaService(SoundContext soundContext)
        {
            _soundContext = soundContext;
        }

        public async Task AddMusicFileAsync(int musicId, IFormFile file, string format, byte[]? image = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var musicFile = new Medium
                {
                    MusicId = musicId,
                    Data = memoryStream.ToArray(),
                    FileType = format,
                    Picture = image
                };

                _soundContext.Media.Add(musicFile);
                await _soundContext.SaveChangesAsync();
            }
        }
        public async Task<Medium> GetMusicFileAsync(int musicId)
        {
            var musicFile = await _soundContext.Media.FirstOrDefaultAsync(m => m.MusicId == musicId);
            return musicFile;
        }


        public async Task UploadPictureAsync(int musicId, IFormFile pictureFile)
        {
            if (pictureFile == null || pictureFile.Length == 0)
                throw new ArgumentException("File is required");

            var medium = await _soundContext.Media.FirstOrDefaultAsync(m => m.MusicId == musicId);
            if (medium == null)
            {
                throw new ArgumentException("The music file must be added first");
            }

            var fileExtension = Path.GetExtension(pictureFile.FileName);
            List<string> allowedExtensions = [ ".jpg", ".jpeg", ".png" ];
            //if (!allowedExtensions.Contains(fileExtension))
            //{
            //    throw new ArgumentException("File must be of allowed extension : .jpg, .jpeg, .png");
            //}
            
            using (var memoryStream = new MemoryStream())
            {
                await pictureFile.CopyToAsync(memoryStream);
                medium.Picture = memoryStream.ToArray();

                await _soundContext.SaveChangesAsync();

            }

        }




        //public async Task<Medium?> FindMusicFileAsync(string hash)
        //{
        //    return await _soundContext.Media.FirstOrDefaultAsync(m => m.Hash == hash);
        //}

    }
}
