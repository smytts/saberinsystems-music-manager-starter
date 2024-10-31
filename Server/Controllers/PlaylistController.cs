﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public PlaylistsController(DataDbContext context)
        {
            _context = context;
        }

        // Get all playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            return await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .ToListAsync();
        }

        // Get a specific playlist by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(Guid id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }
            return playlist;
        }

        // Create new playlist
        [HttpPost]
        public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                return BadRequest("Playlist cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (playlist.Songs == null || !playlist.Songs.Any())
            {
                return BadRequest("At least one song must be included in the playlist.");
            }

            // Fetch existing songs from the database
            var existingSongs = await _context.Songs
                .Where(song => playlist.Songs.Select(s => s.Id).Contains(song.Id))
                .ToListAsync();

            // Check if all songs exist
            if (existingSongs.Count != playlist.Songs.Count)
            {
                return BadRequest("One or more songs do not exist in the library.");
            }

            // Associate existing songs with the new playlist
            playlist.Songs = existingSongs;

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylist), new { id = playlist.Id }, playlist);
        }


        // Update an exisiting playlist
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(Guid id, Playlist updatedPlaylist)
        {
            if (id != updatedPlaylist.Id)
            {
                return BadRequest("Playlist ID mismatch.");
            }

            _context.Entry(updatedPlaylist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // Delete a playlist
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaylistExists(Guid id)
        {
            return _context.Playlists.Any(playlist => playlist.Id == id);
        }
    }
}
