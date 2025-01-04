using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class OpenAIDTO
    {
        [Required(ErrorMessage = "Question is required.")]
        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
