using Feelaware.SmartCloudPortal.Common.DbHelper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MvcMovieCompare.Models;

namespace MvcMovieCompare.Data
{
    public class MvcMovie2Context
    {
        private static string _ConnectionString = "";

        public MvcMovie2Context ()
        {
            
        }

        public IEnumerable<Movie> ToList()
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var helper = new DbHelperSql(conn);

                string sql = @"
select  ID, Genre, Price, ReleaseDate, Title
from    Movie
";
                var dt = helper.ExecDataTableSql(sql);
                var list = dt.AsEnumerable().Select(dr => new Movie()
                {
                    Id = (int)dr["ID"],
                    Genre = (string)dr["Genre"],
                    Price = (decimal)dr["Price"],
                    ReleaseDate = (DateTime)dr["ReleaseDate"],
                    Title = (string)dr["Title"],
                });
                return list;
            }
        }

        public Movie FirstOrDefault(int ID)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var helper = new DbHelperSql(conn);
                
                string sql = @"
select  ID, Genre, Price, ReleaseDate, Title
from    Movie
where   ID = @ID
";
                var paramIn = new SqlParameter("@ID", ID);
                var reader = helper.ExecReaderSql(sql, paramIn);
                if (!reader.HasRows)
                    return null;

                reader.Read();
                return new Movie()
                {
                    Id = reader.GetInt32("ID"),
                    Genre = reader.GetString("Genre"),
                    Price = reader.GetDecimal("Price"),
                    ReleaseDate = reader.GetDateTime("ReleaseDate"),
                    Title = reader.GetString("Title"),
                };
            }
        }

        public void Add(Movie movie)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var helper = new DbHelperSql(conn);

                string sql = @"
insert into Movie
        (Genre, Price, ReleaseDate, Title)
values  (@Genre, @Price, @ReleaseDate, @Title)
";
                var paramsIn = new SqlParameter[]
                {
                    new SqlParameter("@Genre", movie.Genre),
                    new SqlParameter("@Price", movie.Price),
                    new SqlParameter("@ReleaseDate", movie.ReleaseDate),
                    new SqlParameter("@Title", movie.Title),
                };
                int ret = helper.ExecUpdateSql(sql, paramsIn);
            }
        }

        public void Update(Movie movie)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var helper = new DbHelperSql(conn);

                string sql = @"
update  Movie
set     Genre = @Genre,
        Price = @Price,
        ReleaseDate = @ReleaseDate,
        Title = @Title
where   ID = @ID
";
                var paramsIn = new SqlParameter[]
                {
                    new SqlParameter("@Genre", movie.Genre),
                    new SqlParameter("@Price", movie.Price),
                    new SqlParameter("@ReleaseDate", movie.ReleaseDate),
                    new SqlParameter("@Title", movie.Title),
                    new SqlParameter("@ID", movie.Id),
                };
                int ret = helper.ExecUpdateSql(sql, paramsIn);
            }
        }

        public void Remove(Movie movie)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var helper = new DbHelperSql(conn);

                string sql = @"
delete  Movie
where   ID = @ID
";
                var paramIn = new SqlParameter("@ID", movie.Id);
                int ret = helper.ExecUpdateSql(sql, paramIn);
            }
        }

        internal static void AddConnectionString(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }
    }
}

