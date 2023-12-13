using System.Text.Json.Serialization;

namespace MyList_backend.Model
{
    public class Item
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public int? MyListId { get; set; }
 
        public MyList? MyList { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
