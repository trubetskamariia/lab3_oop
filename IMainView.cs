using System.Collections.Generic;
using System.Threading.Tasks;
using lab3_oop.Models;

namespace lab3_oop.Views
{
    public interface IMainView
    {
        string Keyword { get; }
        string SelectedCategory { get; }
        string MaxTimeText { get; }
        bool IsVegetarianOnly { get; }
        Recipe? GetSelectedRecipe();

        void UpdateGrid(List<Recipe> recipes);
        void UpdateCategories(List<string> categories);
        void UpdateStatus(string fileName, bool isModified, bool isFileLoaded);
        void ClearSelection();
        void ClearSearchFields();

        Task<string?> PickJsonFileAsync();
        Task ShowMessageAsync(string title, string message);
        Task<bool> ConfirmAsync(string title, string message);
        Task<Recipe?> OpenEditFormAsync(Recipe? recipeToEdit, int defaultId);
        Task OpenAboutFormAsync();
    }
}