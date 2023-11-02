﻿using IMDBLib.titleBasics;
using System.Data;
using System.Data.SqlClient;

namespace IMDBConsole.titleActions
{
    public class TitleBulked : IInserter<Title>, IInserter<Genre>, IInserter<TitleGenre>
    {
        readonly GlobalFunctions f = new();
        public void InsertData(SqlConnection sqlConn, List<Title> titles)
        {
            DataTable titlesTable = new("Titles");

            titlesTable.Columns.Add("tconst", typeof(string));
            titlesTable.Columns.Add("titleType", typeof(string));
            titlesTable.Columns.Add("primaryTitle", typeof(string));
            titlesTable.Columns.Add("originalTitle", typeof(string));
            titlesTable.Columns.Add("isAdult", typeof(bool));
            titlesTable.Columns.Add("startYear", typeof(int));
            titlesTable.Columns.Add("endYear", typeof(int));
            titlesTable.Columns.Add("runtimeMinutes", typeof(int));

            foreach (Title title in titles)
            {
                DataRow titleRow = titlesTable.NewRow();
                f.FillParameterBulked(titleRow, "tconst", title.tconst);
                f.FillParameterBulked(titleRow, "titleType", title.titleType);
                f.FillParameterBulked(titleRow, "primaryTitle", title.primaryTitle);
                f.FillParameterBulked(titleRow, "originalTitle", title.originalTitle);
                f.FillParameterBulked(titleRow, "isAdult", title.isAdult);
                f.FillParameterBulked(titleRow, "startYear", title.startYear);
                f.FillParameterBulked(titleRow, "endYear", title.endYear);
                f.FillParameterBulked(titleRow, "runtimeMinutes", title.runtimeMinutes);
                titlesTable.Rows.Add(titleRow);
            }
            SqlBulkCopy bulkCopy = new(sqlConn, SqlBulkCopyOptions.KeepNulls, null);
            bulkCopy.DestinationTableName = "Titles";
            bulkCopy.BulkCopyTimeout = 0;
            bulkCopy.WriteToServer(titlesTable);
        }
        // This function only sends the very first genre. I want it to send all genres.
        public void InsertData(SqlConnection sqlConn, List<Genre> genres)
        {
            int genreID = f.GetMaxId("genreID", "Genres", sqlConn);
            if (genreID == -1)
            {
                genreID = 0;
            }
            else
            {
                genreID++;
            }

            DataTable genresTable = new("Genres");

            genresTable.Columns.Add("genreID", typeof(int));
            genresTable.Columns.Add("genreName", typeof(string));

            foreach (Genre genre in genres)
            {
                DataRow genreRow = genresTable.NewRow();
                f.FillParameterBulked(genreRow, "genreID", genreID);
                f.FillParameterBulked(genreRow, "genreName", genre.genreName);
                genresTable.Rows.Add(genreRow);
                genreID++;
            }
            SqlBulkCopy bulkCopy = new(sqlConn, SqlBulkCopyOptions.KeepNulls, null);
            bulkCopy.DestinationTableName = "Genres";
            bulkCopy.BulkCopyTimeout = 0;
            bulkCopy.WriteToServer(genresTable);
        }

        public void InsertData(SqlConnection sqlConn, List<TitleGenre> titleGenres)
        {
            DataTable titleGenresTable = new("TitlesGenres");

            titleGenresTable.Columns.Add("tconst", typeof(string));
            titleGenresTable.Columns.Add("genreID", typeof(int));

            foreach (TitleGenre titleGenre in titleGenres)
            {
                int genreID = f.GetID("genreID", "Genres", "genreName", titleGenre.genreName, sqlConn);

                if (genreID != -1)
                {
                    DataRow titleGenreRow = titleGenresTable.NewRow();
                    f.FillParameterBulked(titleGenreRow, "tconst", titleGenre.tconst);
                    f.FillParameterBulked(titleGenreRow, "genreID", genreID);
                    titleGenresTable.Rows.Add(titleGenreRow);
                }
                else
                {
                    Console.WriteLine($"Genre '{titleGenre.genreName}' not found.");
                }
            }
            SqlBulkCopy bulkCopy = new(sqlConn, SqlBulkCopyOptions.KeepNulls, null);
            bulkCopy.DestinationTableName = "TitlesGenres";
            bulkCopy.BulkCopyTimeout = 0;
            bulkCopy.WriteToServer(titleGenresTable);
        }
    }
}
