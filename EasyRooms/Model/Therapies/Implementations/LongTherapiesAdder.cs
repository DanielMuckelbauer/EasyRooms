namespace EasyRooms.Model.Therapies.Implementations;

public class LongTherapiesAdder : ITherapiesAdder
{
    private readonly IOccupationsAdder _occupationsAdder;

    public LongTherapiesAdder(IOccupationsAdder occupationsAdder)
        => _occupationsAdder = occupationsAdder;

    public void Add(IEnumerable<Room> rooms, List<Row> orderedRows, int bufferInMinutes, RoomNames roomNames)
    {
        var longTherapyRows = orderedRows
            .Where(row => TherapyTypeProvider.IsLongTherapy(row.DurationAsTimeSpan))
            .ToList();

        var longTherapyRooms = rooms.Where(room => room.IsMassageSpecificRoom);

        longTherapyRows.ForEach(therapy =>
        {
            _occupationsAdder.AddToFreeRoom(longTherapyRooms, bufferInMinutes, therapy);
            orderedRows.Remove(therapy);
        });
    }
}