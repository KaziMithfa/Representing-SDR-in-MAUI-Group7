# ML 23/24-07 Implement ML22/23-8 Implement the SDR representation in the MAUI application

# Abstract:

In the field of software engineering, there is a growing demand for user-friendly graphical interfaces, particularly in applications where visualizing data is crucial. This paper details the creation and deployment of a Multimodal Adaptive User Interface (MAUI) application designed to display Sparse Distributed Representations (SDRs) utilizing Scalable Vector Graphics (SVG). The project was motivated by the necessity to seamlessly transition SDR representations from supplied Python scripts to an interactive MAUI application using C#. Our strategy involved crafting a MAUI application capable of accepting input in various forms, including text and file formats, to enhance user convenience. Utilizing the features of SVG, our application generates SDR outputs in both horizontal and vertical layouts, accommodating diverse user preferences. Following extensive development and continuous work, our MAUI application faithfully replicates the functionalities of the expected outcome provided by our professor, successfully meeting the project's primary objective. Our efforts contribute to advancing SDR-based methodologies and the development of MAUI applications, paving the way for future research and innovation in software engineering.

# Introduction:

Sparse Distributed Representation is a frequently employed idea in neural network frameworks, especially within unsupervised learning and spatial coding. It involves representing data in a high-dimensional space where only a small fraction of dimensions is given at any time. This representation is valuable for tasks such as pattern recognition, anomaly detection, and memory storage among others.

SDR offers a highly efficient method for representing complex data patterns using a sparse set of active elements. By integrating SDR into a MAUI application, users can work with large datasets without sacrificing performance or overwhelming system resources.

MAUI applications are designed to adapt to user preferences and input modalities. By incorporating SDR, users can interact with data in various forms, including text, images, and files, making the application more versatile and user-friendly.

# Requirements:

To develope and run this project, we need.
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [.NET Multi-platform App UI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-8.0&tabs=vswin)

# Environment setup:
To setup the development platform, we have to.
- Install Visual Studio 2022 via [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/)
- Download and install [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). Latest version is recommended. 
- Install [.NET Multi-platform App UI workload.](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vswin&view=net-maui-8.0)
- Build a basic [MAUI Application](https://learn.microsoft.com/en-us/dotnet/maui/get-started/first-app?view=net-maui-8.0&tabs=vswin&pivots=devices-windows) 
- Add/reference nuget packages `Microsoft.Maui.Controls`, `Microsoft.Maui.Controls.Compatibility`, `SkiaSharp` and `Svg.Skia`.

# Application setup:
To setup core application we have to add the corresponding .razor, services and component files one by one.
- Place the [Heatmap.razor](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/Components/Pages/Heatmap.razor) core file inside the components to handle Razor Page.
- To navigate Heatmap, add conponent under [NavMenu.razor](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/Components/Layout/NavMenu.razor).
- Add [CsvDataService](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/Services/CsvDataService.cs) class under the Services to handle SDR input.
- Add [PathService](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/Services/PathService.cs). and [IPath](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/Services/IPath.cs). both services.
- Add [HeatmapInputModel](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/HeatmapInputModel.cs) class inside project to deal with initial parameters.
- Include [MauiProgram](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/MauiApp1/MauiProgram.cs) class to operate input data.

# Code demonstration:
## MauiProgram
The provided C# code defines a static class `MauiProgram`, which contains a static method `CreateMauiApp()` responsible for creating a Maui application instance.

```csharp
namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<CsvDataService>();
            builder.Services.AddSingleton<IPath, PathService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

```
## Input file handeling:
The `CsvDataService` class provides methods for extracting data from CSV files or content. 

- `ReadDataFromCsvAsync`: Reads data from a CSV file asynchronously, constructing a list of hash sets containing integer values. Calculates the maximum and minimum values found in the data.

- `ReadDataFromCsvContent`: Reads data from CSV content synchronously, similar to the asynchronous method but accepts CSV content as a string input. 

Both methods handle parsing errors and ensure the returned data is ready for further processing.

```csharp
namespace MauiApp1.Services
{
    public class CsvDataService
    {
        public async Task<(List<HashSet<int>>, int, int)> ReadDataFromCsvAsync(Stream fileStream)
        {
            try
            {
                var dataSets = new List<HashSet<int>>();
                var allCells = new List<int>();

                // Use StreamReader in asynchronous mode
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var values = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(v => v.Trim())
                                         .Where(v => !string.IsNullOrWhiteSpace(v) && int.TryParse(v, out _))
                                         .Select(int.Parse)
                                         .ToList();

                        if (values.Count > 0)
                        {
                            var cell = new HashSet<int>(values);
                            dataSets.Add(cell);
                            allCells.AddRange(cell);
                        }
                    }
                }
                var maxCell = allCells.Count > 0 ? allCells.Max() + 100 : 0;
                var minCell = allCells.Count > 0 ? allCells.Min() - 100 : 0;
                return (dataSets, maxCell, minCell);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public (List<HashSet<int>>, int, int) ReadDataFromCsvContent(string csvContent)
        {
            var dataSets = new List<HashSet<int>>();
            var allCells = new List<int>();

            // Split the input CSV content by lines
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var values = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(v => v.Trim())
                                 .Where(v => !string.IsNullOrWhiteSpace(v) && int.TryParse(v, out _))
                                 .Select(int.Parse)
                                 .ToList();

                if (values.Count > 0)
                {
                    var cell = new HashSet<int>(values);
                    dataSets.Add(cell);
                    allCells.AddRange(cell);
                }
            }
            var maxCell = allCells.Count > 0 ? allCells.Max() + 100 : 0;
            var minCell = allCells.Count > 0 ? allCells.Min() - 100 : 0;
            return (dataSets, maxCell, minCell);
        }
    }
}
```
## HeatmapInputModel

The following C# code defines the `HeatmapInputModel` class, which is utilized for managing input related to a heatmap:

```csharp
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
```
## IPath

The code defines an interface named `IPath` with a single method `GetFolderPath()`, which returns a string representing a folder path. It's part of the `MauiApp1.Services` namespace.

```csharp
namespace MauiApp1.Services
{
    public interface IPath
    {
        string GetFolderPath();
    }
}
```

## PathService

The provided code defines a class named `PathService` within the `MauiApp1.Services` namespace. This class implements the `IPath` interface. The `GetFolderPath()` method of the `PathService` class returns the application data directory path using the `FileSystem.AppDataDirectory` property.

In short:
- The `PathService` class implements the `IPath` interface.
- It has a single method `GetFolderPath()` which returns the application data directory path.

This code is a part of a service that provides functionality related to file paths in a Maui application.

```csharp
namespace MauiApp1.Services
{
    public class PathService : IPath
    {
        public string GetFolderPath()
        {
            return FileSystem.AppDataDirectory;
        }
    }
}
```
### `HandleValidSubmit()` Method Overview

The `HandleValidSubmit()` method is an asynchronous function designed to manage form submissions. Below is a succinct breakdown of its functionality:

- **File Input Handling:**
  - The method first checks if the form utilizes file input (`heatmapInputModel.UseFileInput`) and whether a CSV file is provided (`heatmapInputModel.CsvFile != null`). 
  - If true, it proceeds to read the CSV data from the file using the `CsvDataService.ReadDataFromCsvAsync()` method.

- **Content Input Handling:**
  - If the form does not employ file input and contains CSV content (`!heatmapInputModel.UseFileInput && !string.IsNullOrWhiteSpace(heatmapInputModel.CsvContent)`), it reads the CSV data from the content using `CsvDataService.ReadDataFromCsvContent()` method.

- **Property Assignment:**
  - Following CSV data retrieval, the method sets various properties based on the form input, including `highlightTouch`, `figureName`, `xAxisTitle`, `yAxisTitle`, `isHorizontal`, `numTouches`, and `count`.

This method effectively manages form input, facilitating CSV data retrieval from either a file or content, and subsequent property assignment as per the form input.

```csharp
private async Task HandleValidSubmit()
{
	if (heatmapInputModel.UseFileInput && heatmapInputModel.CsvFile != null)
	{
		var maxFileSize = 1024 * 1024;
		var stream = heatmapInputModel.CsvFile.OpenReadStream(maxFileSize);
		var result = await CsvDataService.ReadDataFromCsvAsync(stream);
		activeCellsColumn = result.Item1;
	}
	else if (!heatmapInputModel.UseFileInput && !string.IsNullOrWhiteSpace(heatmapInputModel.CsvContent))
	{
		var result = CsvDataService.ReadDataFromCsvContent(heatmapInputModel.CsvContent);
		activeCellsColumn = result.Item1;
	}

	highlightTouch = heatmapInputModel.HighlightTouch - 1;
	figureName = heatmapInputModel.FigureName;
	xAxisTitle = heatmapInputModel.XAxisTitle;
	yAxisTitle = heatmapInputModel.YAxisTitle;
	isHorizontal = heatmapInputModel.IsHorizontal;
	numTouches = Math.Min(activeCellsColumn.Count, heatmapInputModel.MaxCycles.HasValue ?
		heatmapInputModel.MaxCycles.Value : 1000);
	count = 0;
```
### `CalculateChartDimensions()` Method Overview

The `CalculateChartDimensions()` method calculates dimensions dynamically for a chart based on the provided data. Here's a concise summary of its functionality:

- **Data Processing:**
  - It iterates through each column of active cells to find the minimum and maximum values (`minCell` and `maxCell`).

- **Chart Dimension Calculation:**
  - Based on whether the chart is horizontal or vertical (`isHorizontal`), it adjusts the chart dimensions accordingly.
  - For horizontal charts, it calculates the chart width and height considering the cell range span and the number of touches.
  - For vertical charts, it calculates the chart height and width similarly.

- **Default Values:**
  - If no active cells are found, it sets default values for `minCell` and `maxCell` to 0.

This method effectively processes the data and determines the appropriate dimensions for the chart based on the provided input.

```csharp
private void CalculateChartDimensions()
{
	minCell = int.MaxValue;
	maxCell = int.MinValue;

	foreach (var column in activeCellsColumn)
	{
		if (column.Any())
		{
			int currentMin = column.Min();
			int currentMax = column.Max();

			if (currentMin < minCell)
				minCell = currentMin;
			if (currentMax > maxCell)
				maxCell = currentMax;
		}
	}

	if (minCell == int.MaxValue && maxCell == int.MinValue)
	{
		minCell = 0;
		maxCell = 0;
	}

	minTouch = 0;
	maxTouch = numTouches;

	if (isHorizontal)
	{
		int cellRangeSpan = activeCellsColumn.Max(col => col.Max()) + 1 - minCell;
		chartWidth = (cellRangeSpan * (cellWidth - 2)) + chartPadding + 200;
		chartHeight = (numTouches * (cellHeight + cellPadding)) + chartPadding + 50;
	}
	else
	{
		int cellRangeSpan = activeCellsColumn.Max(col => col.Max()) + 1 - minCell;
		chartHeight = (cellRangeSpan * (cellHeight - 2)) + chartPadding + 50;
		chartWidth = (numTouches * (cellWidth + cellPadding)) + chartPadding + 200;
	}
}
```

### `GenerateAxisLabelsVertical()` Method Overview

The `GenerateAxisLabelsVertical()` method generates axis labels for a vertical chart. Here's a brief description of its functionality:

- **Range Calculation:**
  - It calculates the range between the minimum and maximum values (`min` and `max`).

- **Step Calculation:**
  - Based on the range, it determines an appropriate step value, considering the magnitude of the range.

- **Label Generation:**
  - It iterates through the range, generating labels at each step.

- **Position Calculation:**
  - For vertical charts (`isVertical`), it calculates the position of each label based on the distance from the minimum value and adjusts it for padding.
  - For horizontal charts, it calculates the position of each label based on the distance from the minimum value.

This method efficiently generates axis labels for vertical charts based on the provided input.

```csharp
private IEnumerable<(string label, int pos)> GenerateAxisLabelsVertical(int min, int max, bool isVertical)
{
	var labels = new List<(string label, int pos)>();
	if (min > 0 && min < 10)
	{
		min = 0;
	}
	int range = max - min;

	int magnitude = (int)Math.Pow(10, (int)Math.Log10(range) - 1);
	int step = (range / magnitude < 5) ? magnitude : magnitude * 5;

	string label;
	int pos;

	for (int i = min; i <= max; i += step)
	{
		label = i.ToString();

		if (isVertical)
		{
			pos = Convert.ToInt32(((float)(i - min) * ((cellHeight - 1) + cellPadding)) + chartPadding) - 12;
		}
		else
		{
			pos = Convert.ToInt32(((float)(i - min) * (cellWidth - 2))) + 100;
		}

		labels.Add((label, pos));
	}
```

### DownloadSVG() and SaveSvgAsImageAsync() Methods

The provided code consists of two methods that work together to asynchronously download an SVG image and save it as a PNG file. Here's the Markdown representation of the code:

```csharp
private async Task DownloadSVG()
{
    //Invoke the JavaScript function to get the SVG content
    var svgContent = await JSRuntime.InvokeAsync<string>("getSvgContent");

    var fileName = "heatmap.png";

    //Convert the SVG content to an image and save it
    await SaveSvgAsImageAsync(svgContent, fileName);

    await DisplayAlert("Download Complete", $"The image has been saved to {fileName}.", "OK");
}

public async Task SaveSvgAsImageAsync(string svgContent, string filename)
{
    //Parse the SVG content
    var svg = new SKSvg();
    svg.FromSvg(svgContent);

    //Create a bitmap to render the SVG onto
    var bitmap = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
    using var canvas = new SKCanvas(bitmap);
    canvas.Clear(SKColors.White);
    canvas.DrawPicture(svg.Picture);
    canvas.Flush();

    //Encode the bitmap as a PNG
    using var image = SKImage.FromBitmap(bitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 80);

    //Get the save path
    string folderPath = FileSystem.CacheDirectory;
    string filePath = Path.Combine(folderPath, filename);

    //Write the image data to a file
    using var stream = File.OpenWrite(filePath);
    data.SaveTo(stream);

    await NotifyUser(filePath);
}
```
# Input SDR Data:
```
29, 158, 180, 200, 230, 293, 321, 350, 389, 410
29, 129, 158, 162, 180, 181, 200, 230, 293,
29, 39, 129, 158, 162, 181, 188, 214, 230, 293, 340,
2, 29, 39, 111, 162, 181, 188, 214, 224, 293, 312, 340,
2, 29, 39, 111, 129, 162, 181, 188, 214, 224, 293, 340,
2, 29, 111, 162, 163, 188, 214, 224, 340, 405,
2, 29, 111, 162, 163, 188, 214, 224, 235, 243, 287, 340, 405,
2, 25, 33, 35, 64, 66, 87, 111, 162, 163, 214, 224, 235, 243, 287, 444,
25, 35, 64, 66, 87, 101, 111, 152, 163, 225, 235, 243, 267, 386, 427,
25, 35, 64, 87, 101, 111, 152, 163, 177, 224, 225, 243, 267, 386, 411,
```
# Result:
After building the MAUI Application and compiling the SDR data via either a CSV file or direct text field input the corresponding outputs will be generated.

### **Vertical Representation**
![Vertical](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/heatmap_vertical.png)
### **Horizontal Representation**
![Horizontal](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/heatmap_horizontal.png)

## MAUI:
### **CSV Data Handling**
![CSV Data Handling](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/InputAsCSVFile.PNG)
### **Text Field Data Handling**
![Text Field Data Handling](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/InputAsTextFile.PNG)
### **Generate Vertical Chart Over Application UI**
![Generate Vertical Chart Over Application UI](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/VerticalOutput.PNG)
### **Generate Horizontal Chart Over Application UI**
![Generate Horizontal Chart Over Application UI](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/HorizontalOutput.PNG)
### **Output File Save and Download**
![Output File Save and Download](https://github.com/KaziMithfa/Representing-SDR-in-MAUI-Group7/blob/main/Output%20Images/DownloadFileAsImageFormat.PNG)
