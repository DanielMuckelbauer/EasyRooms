using System.Collections.Generic;
using System.Linq;
using EasyRooms.Model.Rooms.Models;

namespace EasyRooms.Model.Validation;

public class RoomsValidator : IRoomsValidator
{
    public bool IsValid(IEnumerable<Room> rooms, RoomNames roomNames)
        => rooms.All(room => !OccupationsOverlap(room) && (!room.IsPartnerRoom || IsValidPartnerRoom(roomNames, room)));

    private static bool OccupationsOverlap(Room room)
        => room.Occupations.Count > 0 && RemoveDoublePartnerMassages(room).Occupations
            .All(occupation1 => !room.Occupations
                .Any(occupation2 => occupation1.StartTime < occupation2.EndTime
                                    && occupation2.StartTime < occupation1.EndTime));

    private static Room RemoveDoublePartnerMassages(Room room)
    {
        var clonedRoom = new Room("cloned", 0);
        var unevenPartnerOccupations = room.Occupations
            .Where(occupation => TherapyTypeProvider.IsPartnerTherapy(occupation.TherapyShort))
            .Where((_, i) => i % 2 == 1);
        clonedRoom.Occupations = room.Occupations.Where(occupation => !unevenPartnerOccupations.Contains(occupation)).ToList();
        return clonedRoom;
    }

    private static bool IsValidPartnerRoom(RoomNames roomNames, Room room)
        => roomNames.PartnerRoomsRoomsAsList.Contains(room.Name);
}