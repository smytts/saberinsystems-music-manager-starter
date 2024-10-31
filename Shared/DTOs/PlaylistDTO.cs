using System;
using System.Collections.Generic;

namespace music_manager_starter.Shared.DTOs
{
    public class PlaylistDTO
    {
        public Guid Id { get; set; } // Unique identifier for the playlist
        public string Name { get; set; } // Name of the playlist
        public string Description { get; set; } // Description of the playlist
        public List<Guid> SongIds { get; set; } = new List<Guid>(); // For song selection during form submission
        public List<SongDTO> Songs { get; set; } = new List<SongDTO>(); // List of song details in the playlist
    }
}
