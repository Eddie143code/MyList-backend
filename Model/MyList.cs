namespace MyList_backend.Model
{
    public class MyList
    {
        public int? MyListId { get; set; }
        public string? Name { get; set;}
        public ApplicationUser? User { get; set; }
    }
}
