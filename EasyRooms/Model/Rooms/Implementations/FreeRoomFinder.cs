using EasyRooms.Model.Rooms.Exceptions;
using EasyRooms.Model.Rooms.Interfaces;

namespace EasyRooms.Model.Rooms.Implementations;

public class FreeRoomFinder : IFreeRoomFinder
{
    public Room FindFreeRoom(TimeSpan startTime, TimeSpan endTime, int bufferInMinutes, IEnumerable<Room> rooms)
    {
        var enumeratedRooms = rooms.ToList();
        for (; bufferInMinutes >= 0; bufferInMinutes--)
        {
            var freeRoom = enumeratedRooms
                .FirstOrDefault(room => !room.IsOccupiedAt(startTime, endTime, bufferInMinutes)
                                        && !string.IsNullOrEmpty(room.Name));
            if (freeRoom is { })
            {
                return freeRoom;
            }
        }

        throw new NoFreeRoomException();
    }
}