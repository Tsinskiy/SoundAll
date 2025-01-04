using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MusicPlaylist
{
    public int Id { get; set; }

    public int PlaylistId { get; set; }

    public int MusicId { get; set; }

    public virtual Music Music { get; set; } = null!;

    public virtual Playlist Playlist { get; set; } = null!;
}
