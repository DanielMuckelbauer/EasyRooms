using EasyRooms.Model.BulkCalculation;
using EasyRooms.Model.FileDialog.Interfaces;
using EasyRooms.ViewModel.Commands;

// ReSharper disable MemberCanBePrivate.Global

namespace EasyRooms.ViewModel;

public class BulkTestViewModel : BindableBase
{
    public RelayCommand ChooseDirectoryCommand { get; }
    public RelayCommand BulkCalculateCommand { get; }

    private readonly IFileDialogOpener _dialogOpener;
    private readonly IBulkCalculator _bulkCalculator;

    private string _directory = "C:\\Users\\dadam\\OneDrive\\EasyRooms\\Testpläne";

    public BulkTestViewModel(IFileDialogOpener dialogOpener, IBulkCalculator bulkCalculator)
    {
        _dialogOpener = dialogOpener;
        _bulkCalculator = bulkCalculator;
        ChooseDirectoryCommand = new(ChooseDirectory);
        BulkCalculateCommand = new(BulkCalculate, CanExecuteBulkCalculate);
    }

    private void ChooseDirectory()
    {
        _directory = _dialogOpener.GetDirectory();
        BulkCalculateCommand.RaiseCanExecuteChanged();
    }

    private void BulkCalculate()
        => _bulkCalculator.Calculate(_directory);

    private bool CanExecuteBulkCalculate()
        => !string.IsNullOrEmpty(_directory);
}