using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Music
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int AuthorId { get; set; }

    public int TypeId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();

    public virtual ICollection<MusicGenre> MusicGenres { get; set; } = new List<MusicGenre>();

    public virtual ICollection<MusicPlaylist> MusicPlaylists { get; set; } = new List<MusicPlaylist>();

    public virtual ICollection<MusicTag> MusicTags { get; set; } = new List<MusicTag>();

    public virtual MusicType Type { get; set; } = null!;

    public virtual ICollection<UserPlayListsMusic> UserPlayListsMusics { get; set; } = new List<UserPlayListsMusic>();
}
