using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using music_manager_starter.Shared;
using System.Text.Json.Serialization;

namespace music_manager_starter.Shared
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<Song> Songs { get; set; } = new List<Song>(); // initialized to avoid null reference issues
    }
}
