using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

    public class PagedResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalCount { get; set; }
    }
}