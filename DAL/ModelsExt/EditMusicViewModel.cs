using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ModelsExt
{
    public class EditMusicViewModel
    {
        public int MusicId { get; set; }
        public float Volume { get; set; } = 1.0f;
        public double StartOffset { get; set; }
        public double EndOffset { get; set; }
        public float PitchFactor { get; set; } = 1.0f;
        public int FadeInDuration { get; set; }
        public int FadeOutDuration { get; set; }
    }
}
