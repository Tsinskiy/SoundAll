using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class TagsPlaylist
{
    public int Id { get; set; }

    public int TagId { get; set; }

    public int PlaylistId { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
