﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GetValue.Models
{
    public class ValueContext : DbContext
    {
        public DbSet<Value> Values { get; set; }
    }
}