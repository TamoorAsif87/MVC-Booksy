namespace UI.Areas.admin.ViewModels;

public class BookVMAdmin
{
    public IEnumerable<string>? Categories { get; set; }
    public IEnumerable<BookDto>? Books { get; set; }
    public int showItems { get; set; } = 5;
    public string CategoryName { get; set; } = string.Empty;
    private int _rating = 0;

    public int rating
    {
        get => _rating;
        set => _rating = (value < 0 || value > 5) ? 0 : value;
    }
    private int _priceStart = 0;
    private int _priceEnd = 100;

    public int priceStart
    {
        get => _priceStart;
        set => _priceStart = value < 0 ? 0 : value;
    }

    public int priceEnd
    {
        get => _priceEnd;
        set
        {
            if (value < 0)
            {
                _priceEnd = 1000;
            }
            else
            {
                _priceEnd = (value < _priceStart) ? _priceStart : value;
            }
        }
    }
    public string SortBy { get; set; } = string.Empty;
  
}
