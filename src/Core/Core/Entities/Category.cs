﻿namespace Core.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<CategoryTranslation>? CategoryTranslations  { get; set; }
}

