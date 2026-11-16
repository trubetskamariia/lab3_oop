using lab3_oop.Models;
using lab3_oop.Views;
using System;
using System.IO;
using System.Threading.Tasks;

namespace lab3_oop.Controllers
{
    public class MainController
    {
        private readonly IMainView _view;
        private readonly RecipeModel _model;

        public MainController(IMainView view)
        {
            _view = view;
            _model = new RecipeModel();
            RefreshUI(resetSearch: true);
        }

        public async Task OnOpenFileClickedAsync()
        {
            if (_model.IsModified)
            {
                if (!await _view.ConfirmAsync("Увага", "Є незбережені зміни. Відкрити інший файл без збереження?"))
                    return;
            }

            string? path = await _view.PickJsonFileAsync();
            if (string.IsNullOrEmpty(path)) return;

            if (!string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
            {
                await _view.ShowMessageAsync("Помилка", "Оберіть файл із розширенням .json");
                return;
            }

            try
            {
                await _model.LoadFromFileAsync(path);
                RefreshUI(resetSearch: true);
            }
            catch (Exception ex)
            {
                await _view.ShowMessageAsync("Помилка читання JSON", ex.Message);
            }
        }

        public async Task OnSaveFileClickedAsync()
        {
            try
            {
                await _model.SaveToFileAsync();
                RefreshStatus();
                await _view.ShowMessageAsync("Успіх", "Файл успішно збережено.");
            }
            catch (Exception ex)
            {
                await _view.ShowMessageAsync("Помилка збереження", ex.Message);
            }
        }

        public async Task OnSearchClickedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_view.MaxTimeText) &&
                (!int.TryParse(_view.MaxTimeText.Trim(), out int p) || p <= 0))
            {
                await _view.ShowMessageAsync("Помилка фільтра", "Максимальний час має бути цілим додатним числом.");
                return;
            }

            ApplySearch();
        }

        private void ApplySearch()
        {
            int? maxTime = int.TryParse(_view.MaxTimeText.Trim(), out int parsed) && parsed > 0
                ? parsed
                : null;

            _view.UpdateGrid(_model.Search(_view.Keyword, _view.SelectedCategory, maxTime, _view.IsVegetarianOnly));
        }

        public void OnResetSearchClicked()
        {
            _view.ClearSearchFields();
            _view.UpdateGrid(_model.Search(null, null, null, false));
        }

        public async Task OnAddClickedAsync()
        {
            if (_model.CurrentFilePath == null)
            {
                await _view.ShowMessageAsync("Помилка", "Спочатку відкрийте JSON-файл.");
                return;
            }

            Recipe? newRecipe = await _view.OpenEditFormAsync(null, _model.GetNextId());
            if (newRecipe != null)
            {
                if (_model.IdExists(newRecipe.Id))
                {
                    await _view.ShowMessageAsync("Помилка", $"Рецепт із ID = {newRecipe.Id} вже існує.");
                    return;
                }
                _model.Add(newRecipe);
                RefreshUI(resetSearch: false);
            }
        }

        public async Task OnEditClickedAsync()
        {
            Recipe? selected = _view.GetSelectedRecipe();
            if (selected == null)
            {
                await _view.ShowMessageAsync("Увага", "Оберіть запис у таблиці для редагування.");
                return;
            }

            Recipe? updated = await _view.OpenEditFormAsync(selected.Clone(), 0);
            if (updated != null)
            {
                _model.Update(updated);
                RefreshUI(resetSearch: false);
            }
        }

        public async Task OnDeleteClickedAsync()
        {
            Recipe? selected = _view.GetSelectedRecipe();
            if (selected == null)
            {
                await _view.ShowMessageAsync("Увага", "Оберіть запис для видалення.");
                return;
            }

            if (await _view.ConfirmAsync("Видалення", $"Видалити рецепт «{selected.Title}»?"))
            {
                if (_model.Delete(selected.Id))
                {
                    _view.ClearSelection();
                    RefreshUI(resetSearch: false);
                }
            }
        }

        public async Task OnAboutClickedAsync() => await _view.OpenAboutFormAsync();

        private void RefreshStatus()
        {
            bool isLoaded = !string.IsNullOrEmpty(_model.CurrentFilePath);
            string fileName = isLoaded
                ? Path.GetFileName(_model.CurrentFilePath!)
                : "Файл не відкрито";

            _view.UpdateStatus(fileName, _model.IsModified, isLoaded);
        }

        private void RefreshUI(bool resetSearch)
        {
            RefreshStatus();
            _view.UpdateCategories(_model.GetCategories());

            if (resetSearch)
                OnResetSearchClicked();
            else
                ApplySearch();
        }
    }
}