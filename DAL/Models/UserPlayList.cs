using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class UserPlayList
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserPlayListsMusic> UserPlayListsMusics { get; set; } = new List<UserPlayListsMusic>();
}
