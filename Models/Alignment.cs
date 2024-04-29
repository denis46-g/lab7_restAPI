using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestAPI.Models;

public partial class Alignment
{
    public int Id { get; set; }

    public string? Alignment1 { get; set; }

    [JsonIgnore]
    public virtual ICollection<Superhero> Superheroes { get; set; } = new List<Superhero>();
}
