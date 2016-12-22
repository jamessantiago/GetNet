﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace getnet.core.Model.Entities
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }

        [StringLength(300)]
        public string DisplayName { get; set; }

        [Required, StringLength(200)]
        public string Email { get; set; }

        public virtual ICollection<AlertRule> AlertRules { get; set; }

        public class UserProfileBuildItem : IModelBuildItem
        {
            public void Build(ref ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<UserProfile>().HasIndex("Email").IsUnique();
            }
        }
    }
}
