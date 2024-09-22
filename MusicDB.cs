using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ADO.Net6
{
    internal class MusicDB
    {
        private string connectionString;

        public MusicDB(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void CreatePlaylist(string playlistName, string category, List<int> trackIds)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string queryPlaylist = "INSERT INTO Playlists (Name, Category) VALUES (@Name, @Category); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdPlaylist = new SqlCommand(queryPlaylist, connection, transaction);
                    cmdPlaylist.Parameters.AddWithValue("@Name", playlistName);
                    cmdPlaylist.Parameters.AddWithValue("@Category", category);

                    int playlistId = Convert.ToInt32(cmdPlaylist.ExecuteScalar());

                    foreach (int trackId in trackIds)
                    {
                        string queryTrack = "INSERT INTO PlaylistTracks (PlaylistId, TrackId) VALUES (@PlaylistId, @TrackId)";
                        SqlCommand cmdTrack = new SqlCommand(queryTrack, connection, transaction);
                        cmdTrack.Parameters.AddWithValue("@PlaylistId", playlistId);
                        cmdTrack.Parameters.AddWithValue("@TrackId", trackId);
                        cmdTrack.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error creating playlist: " + ex.Message);
                }
            }
        }

        public List<int> GetTracksWithAboveAveragePlays(int albumId)
        {
            List<int> trackIds = new List<int>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string avgQuery = "SELECT AVG(PlayCount) FROM Tracks WHERE AlbumId = @AlbumId";
                SqlCommand avgCmd = new SqlCommand(avgQuery, connection);
                avgCmd.Parameters.AddWithValue("@AlbumId", albumId);
                double averagePlayCount = Convert.ToDouble(avgCmd.ExecuteScalar());

                string query = "SELECT Id FROM Tracks WHERE AlbumId = @AlbumId AND PlayCount > @AveragePlayCount";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AlbumId", albumId);
                cmd.Parameters.AddWithValue("@AveragePlayCount", averagePlayCount);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trackIds.Add(reader.GetInt32(0));
                    }
                }
            }
            return trackIds;
        }

        public (List<int>, List<int>) GetTop3TracksAndAlbumsByArtist(int artistId)
        {
            List<int> topTracks = new List<int>();
            List<int> topAlbums = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string topTracksQuery = "SELECT TOP 3 Id FROM Tracks WHERE AlbumId IN (SELECT Id FROM Albums WHERE ArtistId = @ArtistId) ORDER BY Rating DESC";
                SqlCommand topTracksCmd = new SqlCommand(topTracksQuery, connection);
                topTracksCmd.Parameters.AddWithValue("@ArtistId", artistId);

                using (SqlDataReader reader = topTracksCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topTracks.Add(reader.GetInt32(0));
                    }
                }

                string topAlbumsQuery = "SELECT TOP 3 Id FROM Albums WHERE ArtistId = @ArtistId ORDER BY Rating DESC";
                SqlCommand topAlbumsCmd = new SqlCommand(topAlbumsQuery, connection);
                topAlbumsCmd.Parameters.AddWithValue("@ArtistId", artistId);

                using (SqlDataReader reader = topAlbumsCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topAlbums.Add(reader.GetInt32(0));
                    }
                }
            }

            return (topTracks, topAlbums);
        }

        public List<int> SearchTracks(string searchTerm)
        {
            List<int> trackIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id FROM Tracks WHERE Name LIKE @SearchTerm OR Lyrics LIKE @SearchTerm";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trackIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return trackIds;
        }
    }
}
