using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MusicGenre
{
    public int Id { get; set; }

    public int MusicId { get; set; }

    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Music Music { get; set; } = null!;
}
