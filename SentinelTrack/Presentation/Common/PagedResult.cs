namespace SentinelTrack.Presentation.Common
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public List<Link> Links { get; set; } = new();
    }

    public class Link
    {
        public string Rel { get; set; } = default!;
        public string Href { get; set; } = default!;
        public string Method { get; set; } = "GET";
    }

    public class Resource<T>
    {
        public T Data { get; set; } = default!;
        public List<Link> Links { get; set; } = new();
    }
}
