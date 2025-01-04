using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MusicType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Music> Musics { get; set; } = new List<Music>();
}
