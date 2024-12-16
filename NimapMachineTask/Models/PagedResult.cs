namespace NimapMachineTask.Models
{
    public class PagedResult
    {
        public IEnumerable<Product> Products { get; set; }
        public int CurrentPage { get; set; }              
        public int PageSize { get; set; }                  
        public int TotalPages { get; set; }               
        public int TotalRecords { get; set; }
    }
}
