using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MusicTag> MusicTags { get; set; } = new List<MusicTag>();

    public virtual ICollection<TagsPlaylist> TagsPlaylists { get; set; } = new List<TagsPlaylist>();
}
