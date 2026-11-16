using lab3_oop.Controllers;
using lab3_oop.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using lab3_oop.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab3_oop.Views
{
    public partial class MainPage : ContentPage, IMainView
    {
        private readonly MainController _controller;

        public MainPage()
        {
            InitializeComponent();
            _controller = new MainController(this);
        }

        public string Keyword => TxtKeyword.Text ?? string.Empty;
        public string SelectedCategory => CboCategory.SelectedItem?.ToString() ?? string.Empty;
        public string MaxTimeText => TxtMaxTime.Text ?? string.Empty;
        public bool IsVegetarianOnly => ChkVeg.IsChecked;
        public Recipe? GetSelectedRecipe() => GridRecipes.SelectedItem as Recipe;

        public void UpdateGrid(List<Recipe> recipes) => GridRecipes.ItemsSource = recipes;

        public void UpdateCategories(List<string> categories)
        {
            string current = SelectedCategory;
            CboCategory.ItemsSource = categories;
            if (categories.Contains(current))
                CboCategory.SelectedItem = current;
            else if (categories.Count > 0)
                CboCategory.SelectedIndex = 0;
        }

        public void UpdateStatus(string fileName, bool isModified, bool isFileLoaded)
        {
            LblStatus.Text = $"{fileName}{(isModified ? " *" : "")}";
            BtnSave.IsEnabled = isModified;
            BtnAdd.IsEnabled = isFileLoaded;
            BtnEdit.IsEnabled = isFileLoaded;
            BtnDelete.IsEnabled = isFileLoaded;
        }

        public void ClearSelection() => GridRecipes.SelectedItem = null;

        public void ClearSearchFields()
        {
            TxtKeyword.Text = string.Empty;
            TxtMaxTime.Text = string.Empty;
            ChkVeg.IsChecked = false;
            if (CboCategory.ItemsSource != null && CboCategory.Items.Count > 0)
                CboCategory.SelectedIndex = 0;
        }

        public async Task<string?> PickJsonFileAsync()
        {
            var jsonTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.macOS, new[] { "json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть JSON-файл",
                FileTypes = jsonTypes
            });

            return result?.FullPath;
        }

        public async Task ShowMessageAsync(string title, string message) =>
            await DisplayAlertAsync(title, message, "OK");

        public async Task<bool> ConfirmAsync(string title, string message) =>
            await DisplayAlertAsync(title, message, "Так", "Ні");

        public async Task<Recipe?> OpenEditFormAsync(Recipe? recipeToEdit, int defaultId)
        {
            var tcs = new TaskCompletionSource<Recipe?>();
            await Navigation.PushModalAsync(new EditPage(recipeToEdit, defaultId, tcs));
            return await tcs.Task;
        }

        public async Task OpenAboutFormAsync() => await Navigation.PushModalAsync(new AboutPage());

        private async void BtnOpen_Clicked(object? sender, EventArgs e) => await _controller.OnOpenFileClickedAsync();
        private async void BtnSave_Clicked(object? sender, EventArgs e) => await _controller.OnSaveFileClickedAsync();
        private async void BtnSearch_Clicked(object? sender, EventArgs e) => await _controller.OnSearchClickedAsync();
        private void BtnReset_Clicked(object? sender, EventArgs e) => _controller.OnResetSearchClicked();
        private async void BtnAdd_Clicked(object? sender, EventArgs e) => await _controller.OnAddClickedAsync();
        private async void BtnEdit_Clicked(object? sender, EventArgs e) => await _controller.OnEditClickedAsync();
        private async void BtnDelete_Clicked(object? sender, EventArgs e) => await _controller.OnDeleteClickedAsync();
        private async void BtnAbout_Clicked(object? sender, EventArgs e) => await _controller.OnAboutClickedAsync();
    }
}