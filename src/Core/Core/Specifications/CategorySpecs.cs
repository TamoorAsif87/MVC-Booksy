namespace Core.Specifications;

public class CategorySpecs
{
    private const int DefaultShowItems = 20;
    private int _showItems = DefaultShowItems;

    public int ShowItems
    {
        get => _showItems;

        set
        {
            _showItems = value > 20 || value < 0 ? DefaultShowItems : value;
        }
    }
}
