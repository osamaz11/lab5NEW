using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace lab5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class bkController : ControllerBase
    {
        [HttpGet("{cat}")]
        public IEnumerable<mybook> Get(int cat)
        {
            List<mybook> li = new List<mybook>();
            //  SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mynewdb;Integrated Security=True;Pooling=False");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("lab5Context");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM book where category ='" + cat + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                li.Add(new mybook
                {
                    title = (string)reader["title"],
                });

            }

            reader.Close();
            conn1.Close();
            return li;
        }
    }
}
