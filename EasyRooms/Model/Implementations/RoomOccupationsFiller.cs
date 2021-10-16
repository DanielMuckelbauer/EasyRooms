﻿using System;
using System.Collections.Generic;
using System.Linq;
using EasyRooms.Model.Interfaces;
using EasyRooms.Model.Models;

namespace EasyRooms.Model.Implementations;

public class RoomOccupationsFiller : IRoomOccupationsFiller
{
    private readonly IOccupationCreationDataProvider _occupationCreationDataProvider;
    private readonly IPartnerRoomFiller _partnerRoomFiller;

    public RoomOccupationsFiller(IOccupationCreationDataProvider occupationKeyInformationExtractor, IPartnerRoomFiller partnerRoomFiller)
    {
        _occupationCreationDataProvider = occupationKeyInformationExtractor;
        _partnerRoomFiller = partnerRoomFiller;
    }

    public IEnumerable<Room> FillRoomOccupations(IEnumerable<Row> rows, RoomNames roomNames, int bufferInMinutes = 0)
    {
        var orderedRows = OrderRows(rows).ToList();
        return CreateRooms(roomNames, orderedRows, bufferInMinutes);
    }

    private IEnumerable<Room> CreateRooms(RoomNames roomNames, List<Row> orderedRows, int bufferInMinutes)
    {
        var rooms = roomNames.AllRoomsAsList.Select((name, i) => new Room(name, i)).ToList();
        SetPartnerRoomProperty(roomNames, rooms);
        _partnerRoomFiller.AddPartnerTherapies(rooms, orderedRows, bufferInMinutes);
        AddNormalTherapies(rooms, orderedRows, bufferInMinutes);
        return rooms;
    }

    private static void SetPartnerRoomProperty(RoomNames roomNames, List<Room> rooms)
        => roomNames.PartnerRoomsRoomsAsList.ToList()
            .ForEach(partnerRoom => rooms
                .Single(room => string.Equals(room.Name, partnerRoom, StringComparison.OrdinalIgnoreCase)).IsPartnerRoom = true);

    private void AddNormalTherapies(List<Room> rooms, List<Row> orderedRows, int bufferInMinutes)
        => orderedRows.ForEach(row => AddOccupation(row, rooms, bufferInMinutes));

    private void AddOccupation(Row row, List<Room> rooms, int bufferInMinutes)
    {
        var occupationCreationData = _occupationCreationDataProvider
            .CalculateOccupationCreationData(row.StartTime, row.Duration, bufferInMinutes, rooms);
        occupationCreationData.FreeRoom
            .AddOccupation(new Occupation(row.Therapist, row.Patient, row.TherapyShort, row.TherapyLong, occupationCreationData.StartTime, occupationCreationData.EndTime));
    }

    private static IOrderedEnumerable<Row> OrderRows(IEnumerable<Row> rows)
        => rows.OrderBy(row => TimeSpan.Parse(row.StartTime.Trim('(', ')')))
            .ThenBy(row => int.Parse(row.Duration));
}
