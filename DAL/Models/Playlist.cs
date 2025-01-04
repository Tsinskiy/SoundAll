using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Playlist
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<MusicPlaylist> MusicPlaylists { get; set; } = new List<MusicPlaylist>();

    public virtual ICollection<TagsPlaylist> TagsPlaylists { get; set; } = new List<TagsPlaylist>();
}
