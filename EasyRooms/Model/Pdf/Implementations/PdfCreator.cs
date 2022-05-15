using EasyRooms.Model.Pdf.Interfaces;
using EasyRooms.Model.Pdf.Models;

namespace EasyRooms.Model.Pdf.Implementations;

public class PdfCreator : IPdfCreator
{
    private readonly IPlansCreator _plansCreator;
    private readonly IHeaderPrinter _headerPrinter;

    public PdfCreator(IPlansCreator plansCreator, IHeaderPrinter headerPrinter)
    {
        _plansCreator = plansCreator;
        _headerPrinter = headerPrinter;
    }

    public IEnumerable<PdfData> Create(IEnumerable<Room> rooms)
    {
        var therapyPlans = _plansCreator.Create(rooms);
        return therapyPlans.Select(WritePdf);
    }

    private PdfData WritePdf(TherapyPlan plan)
    {
        var pdf = Pdf.Create(plan.Therapist);
        _headerPrinter.PrintPageHeader(pdf, plan.Therapist, TherapyPlanConstants.PageHeaderOffset);
        _headerPrinter.PrintColumnHeaders(pdf, TherapyPlanConstants.ColumnsHeaderOffset);
        PrintRows(plan, pdf);
        return pdf;
    }

    private static void PrintRows(TherapyPlan plan, PdfData pdf)
    {
        var rowsOffset = TherapyPlanConstants.InitialRowsOffset;
        for (var i = 0; i < plan.Rows.Count; i++)
        {
            PrintRow(plan.Rows[i], i, rowsOffset, pdf);
            rowsOffset += TherapyPlanConstants.LineHeight;
        }
    }

    private static void PrintRow(TherapyPlanRow row, int rowIndex, double yOffset, PdfData pdfData)
    {
        var rowStrings = new[] {row.StartTime, row.Duration, row.Comment, row.Room, row.TherapyShort, row.Patient};
        LinePrinter.PrintLine(pdfData, rowIndex, rowStrings, pdfData.Font, yOffset);
    }
}