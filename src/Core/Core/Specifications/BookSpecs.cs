namespace Core.Specifications;

public class BookSpecs
{
    public string Search { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int PriceStart { get; set; }
    public int PriceEnd { get; set; }
    public int ShowItems { get; set; }
    public string SortBy { get; set; } = string.Empty;
    public bool IsPaginationEnabled { get; set; } = false;
    public int Take { get; set; }
    public int Skip { get; set; }
}
