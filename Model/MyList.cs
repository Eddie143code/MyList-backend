﻿using System.Text.Json.Serialization;

namespace MyList_backend.Model
{
    public class MyList
    {
        public int? MyListId { get; set; }
        public string? Name { get; set;}
        [JsonIgnore]
        public List<Item>? Items { get; set; } 
        public ApplicationUser? User { get; set; }
    }
}
