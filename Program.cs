using System;
using System.Collections.Generic;

namespace ADO.Net6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=DESKTOP-GE7UVHJ\\SQLEXPRESS;Initial Catalog=MusicDB3;Integrated Security=True;Connect Timeout=30;Encrypt=False";
            MusicDB db = new MusicDB(connectionString);

            bool continueRunning = true;

            while (continueRunning)
            {
                Console.WriteLine("\nChoose an option:");
                Console.WriteLine("1. Create a new playlist");
                Console.WriteLine("2. Get Tracks with Play Count Above Average");
                Console.WriteLine("3. Get Top-3 Tracks and Albums by Artist");
                Console.WriteLine("4. Search for a Track");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewPlaylist(db);
                        break;
                    case "2":
                        GetTracksAboveAverage(db);
                        break;
                    case "3":
                        GetTopTracksAndAlbums(db);
                        break;
                    case "4":
                        SearchForTrack(db);
                        break;
                    case "0":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        break;
                }
            }
        }

        static void AddNewPlaylist(MusicDB db)
        {
            Console.WriteLine("\nEnter details for the new playlist:");
            Console.Write("Playlist Name: ");
            string playlistName = Console.ReadLine()?.Trim();

            Console.Write("Category: ");
            string category = Console.ReadLine()?.Trim();

            Console.WriteLine("Enter track Ids separated by commas (e.g., 1,2,3): ");
            List<int> trackIds = new List<int>();
            string[] trackIdsStr = Console.ReadLine().Split(',');
            foreach (string idStr in trackIdsStr)
            {
                if (int.TryParse(idStr.Trim(), out int trackId))
                {
                    trackIds.Add(trackId);
                }
                else
                {
                    Console.WriteLine($"Invalid track Id: {idStr}");
                }
            }

            try
            {
                db.CreatePlaylist(playlistName, category, trackIds);
                Console.WriteLine("Playlist created with tracks.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating playlist: " + ex.Message);
            }
        }

        static void GetTracksAboveAverage(MusicDB db)
        {
            Console.Write("\nEnter Album Id: ");
            int albumId = int.Parse(Console.ReadLine());

            var tracks = db.GetTracksWithAboveAveragePlays(albumId);

            if (tracks.Count > 0)
            {
                Console.WriteLine("\nTracks with Play Count Above Average:");
                foreach (var trackId in tracks)
                {
                    Console.WriteLine($"Track Id: {trackId}");
                }
            }
            else
            {
                Console.WriteLine("No tracks found with play count above average.");
            }
        }

        static void GetTopTracksAndAlbums(MusicDB db)
        {
            Console.Write("\nEnter Artist Id: ");
            int artistId = int.Parse(Console.ReadLine());

            var (topTracks, topAlbums) = db.GetTop3TracksAndAlbumsByArtist(artistId);

            Console.WriteLine("\nTop 3 Tracks:");
            foreach (var trackId in topTracks)
            {
                Console.WriteLine($"Track Id: {trackId}");
            }

            Console.WriteLine("\nTop 3 Albums:");
            foreach (var albumId in topAlbums)
            {
                Console.WriteLine($"Album Id: {albumId}");
            }
        }

        static void SearchForTrack(MusicDB db)
        {
            Console.Write("\nEnter track name or lyrics to search: ");
            string searchTerm = Console.ReadLine()?.Trim();

            var tracks = db.SearchTracks(searchTerm);

            if (tracks.Count > 0)
            {
                Console.WriteLine("\nFound Tracks:");
                foreach (var trackId in tracks)
                {
                    Console.WriteLine($"Track Id: {trackId}");
                }
            }
            else
            {
                Console.WriteLine("No tracks found matching the search term.");
            }
        }
    }
}
