using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SEO_Automation.Model
{
    /// <summary>
    /// Defines the model class URI which defined the url query string pararm sent in request. 
    /// The fields have validation which makes it mandatory to pass the arguments which otherwise would lead to BadRequest() error being sent.
    /// </summary>
    public class URI
    {
        [Required(ErrorMessage = "searchString is required")]
        public string searchString { get; set; }

        [Required(ErrorMessage = "url is required")]
        public string url { get; set; }

        [Range(1, 100, ErrorMessage = "searchOption is required")]
        public int searchOption { get; set; }
    }
}
