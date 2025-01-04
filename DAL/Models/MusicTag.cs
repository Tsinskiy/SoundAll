using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class MusicTag
{
    public int Id { get; set; }

    public int MusicId { get; set; }

    public int TagId { get; set; }

    public virtual Music Music { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
