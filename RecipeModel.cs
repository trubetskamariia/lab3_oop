using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab3_oop.Models
{
    public class RecipeModel
    {
        private List<Recipe> _recipes = new();
        public string? CurrentFilePath { get; private set; }
        public bool IsModified { get; private set; }

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public async Task LoadFromFileAsync(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (stream.Length == 0)
                throw new JsonException("Файл порожній.");

            var loaded = await JsonSerializer.DeserializeAsync<List<Recipe>>(stream, _jsonOptions)
                         ?? throw new JsonException("Не вдалося зчитати список рецептів.");

            Validate(loaded);

            _recipes = loaded;
            CurrentFilePath = filePath;
            IsModified = false;
        }

        private static void Validate(List<Recipe> items)
        {
            var seenIds = new HashSet<int>();
            foreach (var r in items)
            {
                if (r == null)
                    throw new JsonException("Файл містить порожній (null) елемент.");
                if (r.Id <= 0 || !seenIds.Add(r.Id))
                    throw new JsonException($"Некоректний або дубльований ID: {r.Id}.");
                if (string.IsNullOrWhiteSpace(r.Title))
                    throw new JsonException($"У рецепта з ID={r.Id} відсутня назва.");
                if (r.PrepTimeMinutes <= 0)
                    throw new JsonException($"У рецепта з ID={r.Id} некоректний час приготування.");

                r.Category ??= string.Empty;
                r.Ingredients ??= new List<string>();
            }
        }

        public async Task SaveToFileAsync()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
                throw new InvalidOperationException("Файл не відкрито.");

            string json = JsonSerializer.Serialize(_recipes, _jsonOptions);
            await File.WriteAllTextAsync(CurrentFilePath, json, new UTF8Encoding(false));
            IsModified = false;
        }

        public List<Recipe> Search(string? keyword, string? category, int? maxTime, bool onlyVeg)
        {
            IEnumerable<Recipe> query = _recipes;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(r =>
                    r.Title.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    (r.Ingredients != null && r.Ingredients.Any(i => i.Contains(kw, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "Усі категорії")
            {
                query = query.Where(r => string.Equals(r.Category, category, StringComparison.OrdinalIgnoreCase));
            }

            if (maxTime.HasValue && maxTime.Value > 0)
            {
                query = query.Where(r => r.PrepTimeMinutes <= maxTime.Value);
            }

            if (onlyVeg)
            {
                query = query.Where(r => r.IsVegetarian);
            }

            return query.OrderBy(r => r.Id).ToList();
        }

        public List<string> GetCategories()
        {
            var cats = _recipes
                .Select(r => r.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            cats.Insert(0, "Усі категорії");
            return cats;
        }

        public void Add(Recipe item)
        {
            _recipes.Add(item);
            IsModified = true;
        }

        public void Update(Recipe item)
        {
            int index = _recipes.FindIndex(r => r.Id == item.Id);
            if (index != -1)
            {
                _recipes[index] = item;
                IsModified = true;
            }
        }

        public bool Delete(int id)
        {
            int removed = _recipes.RemoveAll(r => r.Id == id);
            if (removed > 0)
            {
                IsModified = true;
                return true;
            }
            return false;
        }

        public bool IdExists(int id) => _recipes.Any(r => r.Id == id);

        public int GetNextId() => _recipes.Count > 0 ? _recipes.Max(r => r.Id) + 1 : 1;
    }
}