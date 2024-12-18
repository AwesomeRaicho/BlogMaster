﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogMaster.Core.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }

        public string? CatergoryNameEn { get; set; }



        //Navigation Property
        public List<Blog_Category>? BlogCategories { get; set; }

    }
}
