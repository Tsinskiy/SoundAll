using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class UserPlayListsMusic
{
    public int Id { get; set; }

    public int UserPlaylistId { get; set; }

    public int MusicId { get; set; }

    public virtual Music Music { get; set; } = null!;

    public virtual UserPlayList UserPlaylist { get; set; } = null!;
}
