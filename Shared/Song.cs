﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using music_manager_starter.Shared;

namespace music_manager_starter.Shared
{
    public sealed class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public List<Playlist> Playlists { get; set; } = new List<Playlist>(); // For many-to-many relationship with Playlist
        public bool IsSelected { get; set; }
    }
}
