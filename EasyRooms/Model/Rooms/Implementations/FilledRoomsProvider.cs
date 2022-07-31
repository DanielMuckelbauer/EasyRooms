using EasyRooms.Model.DayPlan.Interfaces;
using EasyRooms.Model.DayPlan.Models;
using EasyRooms.Model.Persistence.Extensions;
using EasyRooms.Model.Persistence.Interfaces;
using EasyRooms.Model.Rooms.Interfaces;
using EasyRooms.Model.Validation.Exceptions;
using EasyRooms.Model.Validation.Interfaces;

namespace EasyRooms.Model.Rooms.Implementations;

public class FilledRoomsProvider : IFilledRoomsProvider
{
    private readonly IDayPlanParser _dayPlanParser;
    private readonly IRoomOccupationsFiller _occupationsFiller;
    private readonly IRoomsValidator _validator;
    private readonly IPersistenceService _persistenceService;

    public FilledRoomsProvider(
        IPersistenceService persistenceService,
        IDayPlanParser dayPlanParser,
        IRoomOccupationsFiller occupationsFiller,
        IRoomsValidator validator)
    {
        _persistenceService = persistenceService;
        _dayPlanParser = dayPlanParser;
        _occupationsFiller = occupationsFiller;
        _validator = validator;
    }

    public RoomsWithDate Get(string fileName)
    {
        var parsedPlan = _dayPlanParser.ParseRows(fileName);
        var roomNames = _persistenceService.SavedOptions.Rooms.ToRoomNames();
        var savedOptionsBuffer = _persistenceService.SavedOptions.Buffer;
        var rooms = GetFilledRooms(parsedPlan, roomNames, savedOptionsBuffer);
        return new(rooms, parsedPlan.Date);
    }

    private IEnumerable<Room> GetFilledRooms(ParsedPlan plan, RoomNames roomNames, int savedOptionsBuffer)
    {
        var filledRooms = _occupationsFiller
            .FillRoomOccupations(plan.Rows, roomNames, savedOptionsBuffer)
            .ToList();
        Validate(filledRooms, savedOptionsBuffer);
        return filledRooms;
    }

    private void Validate(IEnumerable<Room> filledRooms, int savedOptionsBuffer)
        => _ = _validator.IsValid(filledRooms, _persistenceService.SavedOptions.Rooms.ToRoomNames(), savedOptionsBuffer)
            ? default(object)
            : throw new RoomsValidationException();
}