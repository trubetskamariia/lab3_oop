using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using lab3_oop.Controllers;
using lab3_oop.Models;

namespace lab3_oop.Views
{
    public partial class EditPage : ContentPage, IEditView
    {
        private readonly EditController _controller;
        private readonly TaskCompletionSource<Recipe?> _tcs;

        public EditPage(Recipe? recipe, int defaultId, TaskCompletionSource<Recipe?> tcs)
        {
            InitializeComponent();
            _tcs = tcs;
            _controller = new EditController(this, recipe, defaultId);
        }

        public string IdText => TxtId.Text ?? string.Empty;
        public string TitleText => TxtTitle.Text ?? string.Empty;
        public string CategoryText => TxtCategory.Text ?? string.Empty;
        public string PrepTimeText => TxtTime.Text ?? string.Empty;
        public bool IsVegetarian => ChkVeg.IsChecked;
        public string IngredientsText => TxtIngredients.Text ?? string.Empty;

        public void SetData(Recipe r, bool isEditMode)
        {
            Title = isEditMode ? "Редагування рецепта" : "Новий рецепт";
            TxtId.Text = r.Id.ToString();
            TxtId.IsReadOnly = isEditMode;
            TxtTitle.Text = r.Title;
            TxtCategory.Text = r.Category;
            TxtTime.Text = r.PrepTimeMinutes > 0 ? r.PrepTimeMinutes.ToString() : string.Empty;
            ChkVeg.IsChecked = r.IsVegetarian;
            TxtIngredients.Text = string.Join(", ", r.Ingredients);
        }

        public async Task ShowErrorAsync(string msg) => await DisplayAlertAsync("Помилка валідації", msg, "OK");

        public async void CloseWithResult(Recipe? result)
        {
            await Navigation.PopModalAsync();
            _tcs.TrySetResult(result);
        }

        private async void BtnSave_Clicked(object? sender, EventArgs e) => await _controller.OnSaveClickedAsync();
        private void BtnCancel_Clicked(object? sender, EventArgs e) => _controller.OnCancelClicked();

        protected override bool OnBackButtonPressed()
        {
            _controller.OnCancelClicked();
            return true;
        }
    }
}