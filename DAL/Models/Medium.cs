using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Medium
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public byte[]? Picture { get; set; }

    public int MusicId { get; set; }

    public virtual Music Music { get; set; } = null!;
}
