using EasyRooms.Model.Validation.Interfaces;

namespace EasyRooms.Model.Validation.Implementations;

public class RoomsValidator : IRoomsValidator
{
    public bool IsValid(IEnumerable<Room> rooms, RoomNames roomNames, int savedOptionsBuffer)
        => rooms.All(room => !OccupationsOverlap(room, savedOptionsBuffer) && (!room.IsPartnerRoom || IsValidPartnerRoom(roomNames, room)));

    private static bool OccupationsOverlap(Room room, int savedOptionsBuffer)
        => room.Occupations.Count > 0
           && RemoveDoublePartnerMassages(room).Occupations
               .All(occupation1 => room.Occupations
                   .All(occupation2 =>
                       // todo ignore if occupations have same patient
                       occupation1.StartTime > occupation2.EndTime.Add(TimeSpan.FromMinutes(savedOptionsBuffer))
                                       && occupation2.StartTime > occupation1.EndTime.Add(TimeSpan.FromMinutes(savedOptionsBuffer))));

    private static Room RemoveDoublePartnerMassages(Room room)
    {
        var clonedRoom = new Room("cloned");
        var unevenPartnerOccupations = room.Occupations
            .Where(occupation => TherapyTypeComparer.IsPartnerTherapy(occupation.TherapyShort))
            .Where((_, i) => i % 2 == 1);
        clonedRoom.Occupations = room.Occupations
            .Where(occupation => !unevenPartnerOccupations.Contains(occupation))
            .ToList();
        return clonedRoom;
    }

    private static bool IsValidPartnerRoom(RoomNames roomNames, Room room)
        => roomNames.PartnerRoomsRoomsAsList.Contains(room.Name);
}