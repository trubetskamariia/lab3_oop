using System;
using Microsoft.Maui.Controls;

namespace lab3_oop.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage() => InitializeComponent();
        private async void BtnClose_Clicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
    }
}