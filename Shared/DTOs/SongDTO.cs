using System;
using System.Collections.Generic;

namespace music_manager_starter.Shared.DTOs
{
    public class SongDTO
    {
        public Guid Id { get; set; } // Unique identifier for the song
        public string Title { get; set; } // Title of the song
        public string Artist { get; set; } // Artist of the song
        public string Album { get; set; } // Album the song belongs to
        public string Genre { get; set; } // Genre of the song
        public bool IsSelected { get; set; } // Indicates if the song is selected in the UI
        public List<Guid> PlaylistIds { get; set; } = new List<Guid>(); // List of playlist IDs the song belongs to
    }
}
