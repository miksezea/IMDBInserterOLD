﻿using IMDBLib.titleBasics;
using System.Data.SqlClient;

namespace IMDBConsole.titleActions
{
    public class TitleNormal : IInserter<Title>, IInserter<Genre>, IInserter<TitleGenre> 
    {
        readonly GlobalFunctions f = new();
        readonly TitleExtra e = new();
        public void InsertData(SqlConnection sqlConn, List<Title> titles)
        {
            foreach (Title title in titles) 
            {
                SqlCommand sqlCmd = new("INSERT INTO [dbo].[Titles]" +
                    "([tconst],[titleType],[primaryTitle],[originalTitle]," +
                    "[isAdult],[startYear],[endYear],[runtimeMinutes])VALUES " +
                    $"('{title.tconst}','{title.titleType}','{f.ConvertToSqlString(title.primaryTitle)}'," +
                    $"'{f.ConvertToSqlString(title.originalTitle)}','{title.isAdult}'," +
                    $"{f.CheckIntForNull(title.startYear)},{f.CheckIntForNull(title.endYear)}," +
                    $"{f.CheckIntForNull(title.runtimeMinutes)})", sqlConn);

                try 
                {
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(sqlCmd.CommandText);
                    Console.ReadKey();
                }
            }
        }

        public void InsertData(SqlConnection sqlConn, List<Genre> genres) 
        {
            foreach (Genre genre in genres) 
            {
                SqlCommand sqlCmd = new("INSERT INTO [dbo].[Genres]" +
                    "([genreName])VALUES " +
                    $"('{genre.genreName}')", sqlConn);

                try 
                {
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(sqlCmd.CommandText);
                    Console.ReadKey();
                }
            }
        }

        public void InsertData(SqlConnection sqlConn, List<TitleGenre> titleGenres)
        {
            foreach (TitleGenre titleGenre in titleGenres) 
            {
                int genreID = f.GetID("genreID", "Genres", "genreName", titleGenre.genreName, sqlConn);

                if (genreID != -1) 
                {
                    // Insert the data into the TitlesGenres table
                    SqlCommand sqlCmd = new("INSERT INTO [dbo].[TitlesGenres]" +
                        "([tconst],[genreID])VALUES " +
                        $"('{titleGenre.tconst}',{genreID})", sqlConn);

                    try
                    {
                        sqlCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(sqlCmd.CommandText);
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine($"Genre '{titleGenre.genreName}' not found.");
                }
            }
        }
    }
}
