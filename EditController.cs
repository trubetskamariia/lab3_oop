using lab3_oop.Models;
using lab3_oop.Views;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace lab3_oop.Controllers
{
    public class EditController
    {
        private readonly IEditView _view;

        public EditController(IEditView view, Recipe? recipe, int defaultId)
        {
            _view = view;
            if (recipe != null)
                _view.SetData(recipe, isEditMode: true);
            else
                _view.SetData(new Recipe { Id = defaultId }, isEditMode: false);
        }

        public async Task OnSaveClickedAsync()
        {
            if (!int.TryParse(_view.IdText.Trim(), out int id) || id <= 0)
            {
                await _view.ShowErrorAsync("ID має бути цілим додатним числом.");
                return;
            }
            if (string.IsNullOrWhiteSpace(_view.TitleText) || string.IsNullOrWhiteSpace(_view.CategoryText))
            {
                await _view.ShowErrorAsync("Назва та Категорія є обов'язковими полями.");
                return;
            }
            if (!int.TryParse(_view.PrepTimeText.Trim(), out int time) || time <= 0)
            {
                await _view.ShowErrorAsync("Час приготування має бути цілим числом більше 0.");
                return;
            }

            var ingredients = _view.IngredientsText
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .Where(i => i.Length > 0)
                .ToList();

            if (ingredients.Count == 0)
            {
                await _view.ShowErrorAsync("Вкажіть хоча б один інгредієнт.");
                return;
            }

            var result = new Recipe
            {
                Id = id,
                Title = _view.TitleText.Trim(),
                Category = _view.CategoryText.Trim(),
                PrepTimeMinutes = time,
                IsVegetarian = _view.IsVegetarian,
                Ingredients = ingredients
            };

            _view.CloseWithResult(result);
        }

        public void OnCancelClicked() => _view.CloseWithResult(null);
    }
}