using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace lab5.Models
{
    public class bookorder
    {
        public int Id { get; set; }
        public string custname { get; set; }
        public int total { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime orderdate { get; set; }
    }

}
