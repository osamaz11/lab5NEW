using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace lab5.Models
{
    public class orders
    {
        public int Id { get; set; }
        public string bookname { get; set; }
        public string custname { get; set; }
        public int quantity { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime buydate { get; set; }
    }
}
