using EasyRooms.Model.Dialogs.Interfaces;
using EasyRooms.Model.FileDialog.Interfaces;
using EasyRooms.Model.Pdf.Interfaces;
using EasyRooms.Model.Rooms.Exceptions;
using EasyRooms.Model.Rooms.Interfaces;
using EasyRooms.Model.Validation.Exceptions;
using EasyRooms.ViewModel.Commands;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace EasyRooms.ViewModel;

public class XpsUploadViewModel : BindableBase
{
    public RelayCommand CalculateOccupationsCommand { get; set; }
    public RelayCommand ChooseFileCommand { get; set; }

    private readonly IFileDialogOpener _fileDialogOpener;
    private readonly IPdfWriter _pdfWriter;
    private readonly IMessageBoxShower _messageBoxShower;
    private readonly IFilledRoomsProvider _filledRoomsProvider;
    private string _fileName = @"C:\Users\dadam\OneDrive\EasyRooms\Testpläne\Puffer nicht eingehalten Linh 18.07.xps";

    public XpsUploadViewModel(
        IFileDialogOpener fileDialogOpener,
        IPdfWriter pdfWriter,
        IMessageBoxShower messageBoxShower,
        IFilledRoomsProvider filledRoomsProvider)
    {
        CalculateOccupationsCommand = new(CalculateOccupations, CanCalculateOccupations);
        ChooseFileCommand = new(OpenFileDialog);
        _messageBoxShower = messageBoxShower;
        _filledRoomsProvider = filledRoomsProvider;
        _fileDialogOpener = fileDialogOpener;
        _pdfWriter = pdfWriter;
    }

    private void OpenFileDialog()
    {
        _fileName = _fileDialogOpener.GetFileName();
        CalculateOccupationsCommand.RaiseCanExecuteChanged();
    }

    private bool CanCalculateOccupations()
        => !string.IsNullOrEmpty(_fileName);

    private void CalculateOccupations()
    {
        try
        {
            GuardFileName();
            var roomsWithDate = _filledRoomsProvider.Get(_fileName);
            _pdfWriter.Write(roomsWithDate.Rooms, roomsWithDate.Date);
            _messageBoxShower.Success();
        }
        catch (NoFreeRoomException)
        {
            _messageBoxShower.NoFreeRoomFound();
        }
        catch (RoomsValidationException)
        {
            _messageBoxShower.ValidationFailed();
        }
        catch (Exception exception)
        {
            _messageBoxShower.UnknownError(exception);
        }
    }

    private void GuardFileName()
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            throw new ArgumentNullException(nameof(_fileName));
        }
    }
}