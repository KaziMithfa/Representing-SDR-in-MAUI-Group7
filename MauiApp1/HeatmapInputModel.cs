using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

public class HeatmapInputModel
{
    public IBrowserFile CsvFile { get; set; }
    public string CsvContent { get; set; }
    public bool UseFileInput { get; set; } = true;

    [Required(ErrorMessage = "Highlight touch is required.")]
    public int HighlightTouch { get; set; }

    [Required(ErrorMessage = "Figure name is required.")]
    public string FigureName { get; set; }

    [Required(ErrorMessage = "X Axis label is required.")]
    public string XAxisTitle { get; set; }

    [Required(ErrorMessage = "Y Axis label is required.")]
    public string YAxisTitle { get; set; }

    public int? MaxCycles { get; set; }
    public int? MinCell { get; set; }
    public int? MaxCell { get; set; }
    public int? MinTouch { get; set; }
    public int? MaxTouch { get; set; }
    public bool IsHorizontal { get; set; }
}
