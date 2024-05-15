using Microsoft.AspNetCore.Mvc;
using MvcMovies.Models;
using System.Data.SqlClient;

namespace MvcMovies.Controllers
{
    public class MoviesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var listMovies = new List<Movie>();

            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
            connectionString.InitialCatalog = "Movies";
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Id, Title, ReleaseDate, Genre, Price FROM dbo.Movies_table";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var movie = new Movie
                    {
                        Title = (string)reader["Title"],
                        Genre = (string)reader["Genre"],
                        Id = (int)reader["Id"],
                        Price = (decimal)reader["Price"],
                        ReleaseDate = (string)reader["ReleaseDate"]
                    };
                    listMovies.Add(movie);
                }
                reader.Close();
            }

            return View(listMovies);

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var idMovie = id;
            var movie = new Movie { };

            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
            connectionString.InitialCatalog = "Movies";
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT Id, Title, ReleaseDate, Genre, Price FROM dbo.Movies_table WHERE Id = {idMovie}";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movie.Title = (string)reader["Title"];
                    movie.Genre = (string)reader["Genre"];
                    movie.Id = (int)reader["Id"];
                    movie.Price = (decimal)reader["Price"];
                    movie.ReleaseDate = (string)reader["ReleaseDate"];
                }
                reader.Close();
            }

            return View(movie);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        public async Task<IActionResult> Delete()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
            connectionString.InitialCatalog = "Movies";
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;

            using (SqlConnection connectionCreate = new SqlConnection(cs))
            {
                connectionCreate.Open();

                SqlCommand cmdCreate = new SqlCommand($"DELETE FROM dbo.Movies_table WHERE Id = {id} ", connectionCreate);

                cmdCreate.ExecuteNonQuery();

                connectionCreate.Close();
            }

            return RedirectToAction("Index", "Movies");
        }

        [HttpPost]
        public IActionResult Create(IFormCollection formData)
        {
            SqlConnectionStringBuilder connectionString = new();
            connectionString.DataSource = "DESKTOP-9NLV2TR\\MSSQLSERVER10";
            connectionString.InitialCatalog = "Movies";
            connectionString.IntegratedSecurity = true;

            var cs = connectionString.ConnectionString;
            var last_id = 0;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT MAX(Id) as last_id FROM dbo.Movies_table";

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    last_id = (int)reader["last_id"];
                }
                reader.Close();
            }

            var movie = new Movie
            {
                Title = formData["Title"],
                Genre = formData["Genre"],
                Id = last_id + 1,
                Price = Decimal.Parse(formData["Price"]),
                ReleaseDate = formData["ReleaseDate"],
            };

            using (SqlConnection connectionCreate = new SqlConnection(cs))
            {
                connectionCreate.Open();

                SqlCommand cmdCreate = new SqlCommand($"INSERT INTO dbo.Movies_table VALUES('{movie.Title}', '{movie.Genre}', {movie.Price}, '{movie.ReleaseDate}', {movie.Id})", connectionCreate);

                cmdCreate.ExecuteNonQuery();

                connectionCreate.Close();
            }

            return RedirectToAction("Index", "Movies");
        }
    }
}
