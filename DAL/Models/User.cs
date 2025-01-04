using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? AccountFinishDate { get; set; }

    public int UserType { get; set; }

    public virtual ICollection<UserPlayList> UserPlayLists { get; set; } = new List<UserPlayList>();
}
