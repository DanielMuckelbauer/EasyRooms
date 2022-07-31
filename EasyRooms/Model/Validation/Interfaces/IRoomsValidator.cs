namespace EasyRooms.Model.Validation.Interfaces;

public interface IRoomsValidator
{
    bool IsValid(IEnumerable<Room> rooms, RoomNames roomNames, int savedOptionsBuffer);
}