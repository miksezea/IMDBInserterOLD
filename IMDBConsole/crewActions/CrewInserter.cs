﻿using IMDBLib.titleCrew;
using System.Data.SqlClient;

namespace IMDBConsole.crewActions
{
    public class CrewInserter
    {
        readonly List<Director> directors = new();
        readonly List<Writer> writers = new();
        int _lineAmount = 0;
        string _path = "";
        SqlConnection sqlConn = new();
        readonly GlobalFunctions f = new();

        public void InsertCrewData(string connString, int inserterType, string path, int lineAmount)
        {
            DateTime before = DateTime.Now;

            _lineAmount = lineAmount;
            _path = path;

            sqlConn = new(connString);
            sqlConn.Open();

            MakeLists();

            IInserter<Director>? directorInsert = null;
            IInserter<Writer>? writerInsert = null;

            switch (inserterType)
            {
                case 1:
                    directorInsert = new CrewNormal();
                    writerInsert = new CrewNormal();
                    break;
                case 2:
                    directorInsert = new CrewPrepared();
                    writerInsert = new CrewPrepared();
                    break;
                case 3:
                    directorInsert = new CrewBulked();
                    writerInsert = new CrewBulked();
                    break;
            }
            directorInsert?.InsertData(sqlConn, directors);
            writerInsert?.InsertData(sqlConn, writers);

            DateTime after = DateTime.Now;
            TimeSpan ts = after - before;
            Console.WriteLine($"Time taken: {ts}");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        public void MakeLists()
        {
            IEnumerable<string> lines = File.ReadLines(_path).Skip(1);
            if (_lineAmount != 0)
            {
                lines = lines.Take(_lineAmount);
            }

            foreach (string line in lines)
            {
                string[] values = line.Split("\t");

                if (values.Length == 3)
                {
                    bool tconstExists = f.CheckForTconst(values[0], sqlConn);
                    if (tconstExists)
                    {
                        // Directors table
                        if (values[2] != @"\N")
                        {
                            string[] directors = values[2].Split(",");

                            foreach (string director in directors)
                            {
                                bool nconstExists = f.CheckForNconst(director, sqlConn);
                                if (nconstExists)
                                {
                                    this.directors.Add(new Director(director, values[0]));
                                }
                            }
                        }

                        if (values[3] != @"\N")
                        {
                            string[] writers = values[3].Split(",");

                            foreach (string writer in writers)
                            {
                                bool nconstExists = f.CheckForNconst(writer, sqlConn);
                                if (nconstExists)
                                {
                                    this.writers.Add(new Writer(writer, values[0]));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
