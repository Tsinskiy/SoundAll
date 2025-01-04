using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Music> Musics { get; set; } = new List<Music>();
}
