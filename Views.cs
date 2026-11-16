using System.Threading.Tasks;
using lab3_oop.Models;

namespace lab3_oop.Views
{
    public interface IEditView
    {
        string IdText { get; }
        string TitleText { get; }
        string CategoryText { get; }
        string PrepTimeText { get; }
        bool IsVegetarian { get; }
        string IngredientsText { get; }

        void SetData(Recipe recipe, bool isEditMode);
        Task ShowErrorAsync(string message);
        void CloseWithResult(Recipe? result);
    }
}