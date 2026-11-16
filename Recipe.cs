using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab3_oop.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PrepTimeMinutes { get; set; }
        public bool IsVegetarian { get; set; }
        public List<string> Ingredients { get; set; } = new();

        [JsonIgnore]
        public string IngredientsSummary => Ingredients != null && Ingredients.Count > 0
            ? string.Join(", ", Ingredients)
            : "—";

        [JsonIgnore]
        public string VegStatus => IsVegetarian ? "Так" : "Ні";

        public Recipe Clone() => new()
        {
            Id = Id,
            Title = Title,
            Category = Category,
            PrepTimeMinutes = PrepTimeMinutes,
            IsVegetarian = IsVegetarian,
            Ingredients = Ingredients != null ? new List<string>(Ingredients) : new List<string>()
        };
    }
}