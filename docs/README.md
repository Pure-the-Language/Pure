# Pure API Documentation

## CentralSnippets

### `M:CentralSnippets.Main.Download(System.String,System.String,System.Boolean)`

Download text or binary content

### `M:CentralSnippets.Main.DownloadUrl(System.String,System.String,System.Boolean)`

Download from HTTP

### `P:CentralSnippets.Main.SnippetsHostSite`

When using HTTP, will fetch using GET; When specifying local disk, will read file

### `P:CentralSnippets.Main.SnippetsRootFolder`

Can be null/empty if no need for appendix

## CLI

### `M:CLI.Main.Get(System.String,System.String[])`

Get from name; Conventional --Key.

### `M:CLI.Main.GetSingle(System.String,System.String[])`

Get from name; Conventional --Key.

### `M:CLI.Main.Map(System.String[])`

Map 1-1 dictionary;
Don't allow toggles.

### `M:CLI.Main.MapMany(System.Boolean,System.String[])`

Map 1-many dictionary; Conventional --keyword;
Also handles toggles.

### `M:CLI.Main.Parse``1(System.String[])`

Parse arguments in a keyword list fashion: e.g. --Keyword [values].
Keyword is case insensitive.
Also handles toggles. Automatically handles array.

### `M:CLI.Main.ParsePositional``1(System.String[])`

Maps properties in order.

## ODBC

### `M:ODBC.DataTableHelper.Print(System.Data.DataTable)`

Print DataTable using ConsoleTables

### `M:ODBC.DataTableHelper.ToCSV(System.Data.DataTable)`

Convert DataTable to CSV string

### `M:ODBC.Main.Command(System.String)`

Execute arbitrary SQL command.
Automatically commits.

### `M:ODBC.Main.Delete(System.String)`

Execute a delete query.
Automatically commits.
Equivalent to Command(); This function is just provided for semantic clarity.

### `M:ODBC.Main.Insert(System.String)`

Execute an insert query.
Automatically commits.
Equivalent to Command(); This function is just provided for semantic clarity.

### `M:ODBC.Main.OpenTransaction`

Starts a transaction. The transaction object has the same sort of interfaces as the Main interface.

### `M:ODBC.Main.Query(System.String)`

Query raw DataTable

### `M:ODBC.Main.Select``1(System.String)`

Select strongly typed rows from the query.
This is one of THE most commonly used function of this library.

### `M:ODBC.Main.Update(System.String)`

Execute an update query.
Automatically commits.
Equivalent to Command(); This function is just provided for semantic clarity.

### `P:ODBC.Main.DSN`

Sets the DSN to connect to.
Always set this prior to calling any query functions.

### `T:ODBC.DataTableHelper`

Provides extension methods for DataTable

## Pipeline

## Plot

### `F:Graphing.PlotType.Histogram`

Histogram

### `F:Graphing.PlotType.Line`

Basic line charat

### `F:Graphing.PlotType.Scatter`

Basic scatter plog

### `F:Graphing.PlotType.Signal`

Evenlly sampled with sample rate

### `M:Graphing.InteractivePlotData.LoadData(System.String)`

Load plot data from compressed file.

### `M:Graphing.InteractivePlotData.ReadFromStream(System.IO.BinaryReader)`

Read plot data from stream.

### `M:Graphing.InteractivePlotData.SaveData(Graphing.InteractivePlotData,System.String)`

Save plot data as compressed file.

### `M:Graphing.InteractivePlotData.WriteToStream(System.IO.BinaryWriter,Graphing.InteractivePlotData)`

Save plot data to stream.

### `M:Graphing.Main.Execute(Graphing.PlotType,System.Double[],System.Collections.Generic.List{System.Double[]},Graphing.PlotOptions)`

Execute graphing per options and plot type.

### `M:Graphing.Main.Make(System.Double[][])`

Make a list.

### `M:Graphing.Main.Make(System.String)`

Make options.

### `M:Graphing.Main.Plot(Graphing.PlotType,System.Double[],System.Collections.Generic.List{System.Double[]},Graphing.PlotOptions)`

Plot interaactively

### `M:Graphing.Main.Plot(System.Double[],System.Double[][])`

Plot interaactively

### `M:Graphing.Main.Plot(System.String,System.Double[],System.Collections.Generic.List{System.Double[]},Graphing.PlotOptions)`

Plot interaactively

### `M:Graphing.Main.Save(Graphing.PlotType,System.Double[],System.Collections.Generic.List{System.Double[]},System.String,Graphing.PlotOptions)`

Save to image

### `M:Graphing.Main.Save(System.String,System.Double[],System.Collections.Generic.List{System.Double[]},System.String,Graphing.PlotOptions)`

Save to image

### `M:Graphing.Plotters.CookOptions(System.String[])`

Make options from string arguments.

### `M:Graphing.Plotters.GetAssemblyFolder`

Get folder path of currently executing assembly

### `M:Graphing.Plotters.Histogram(System.Double[],System.Int32,System.String[])`

Plot histogram into given number of bars.

### `M:Graphing.Plotters.InitializePlot(Graphing.PlotType,System.Double[],System.Collections.Generic.List{System.Double[]},Graphing.PlotOptions)`

Initialize plot based on type and data

### `M:Graphing.Plotters.LineChart(System.Double[],System.Collections.Generic.List{System.Double[]},System.String[])`

Draw or render a line chart

### `M:Graphing.Plotters.LineChart(System.Double[],System.Double[],System.String[])`

Draw or render a line chart

### `M:Graphing.Plotters.Scatter(System.Double[],System.Collections.Generic.List{System.Double[]},System.String[])`

Draw or render a scatter plot; Currently looks the same as line chart

### `M:Graphing.Plotters.Scatter(System.Double[],System.Double[],System.String[])`

Draw or render a scatter plot; Currently looks the same as line chart

### `M:Graphing.Plotters.Signal(System.Collections.Generic.List{System.Double[]},System.Int32,System.String[])`

Draw a signal chart

### `M:Graphing.Plotters.Signal(System.Double[],System.Int32,System.String[])`

Draw a signal chart

### `M:Graphing.Plotters.SummonInteractiveWindow(Graphing.PlotType,System.Double[],System.Collections.Generic.List{System.Double[]},Graphing.PlotOptions)`

Create display using interactive window

### `P:Graphing.InteractivePlotData.Options`

Additional customization options.

### `P:Graphing.InteractivePlotData.PlotType`

Plot type.

### `P:Graphing.InteractivePlotData.X`

Main X-axis numerical data.

### `P:Graphing.InteractivePlotData.Ys`

Main Y-axis values.

### `P:Graphing.Main.DefaultOptions`

Default plot options

### `P:Graphing.PlotOptions.HistogramBars`

Number of bars for histogram; Input data must have more than this number of elements otherwise the value is not used.

### `P:Graphing.PlotOptions.Labels`

Series labels

### `P:Graphing.PlotOptions.SignalSampleRate`

Applies to Signal type plot

### `T:Graphing.InteractivePlotData`

Main interoperability data transfer between this library and display backend (frontend)

### `T:Graphing.Main`

Standard library entry

### `T:Graphing.PlotOptions`

General configurations for plots, certain values are only applicable to specific plots.
Serialized in

### `T:Graphing.Plotters`

Static class of various specific plotting types

### `T:Graphing.PlotType`

Available plot types

## Python

## Razor

## Vector

### `M:Math.Main.Vector`

Create empty vector

### `M:Math.Main.Vector(System.Collections.Generic.IEnumerable{System.Boolean})`

Create vector from enumerable

### `M:Math.Main.Vector(System.Collections.Generic.IEnumerable{System.Double})`

Create vector from enumerable

### `M:Math.Main.Vector(System.Collections.Generic.IEnumerable{System.Int32})`

Create vector from enumerable

### `M:Math.Main.Vector(System.Collections.Generic.IEnumerable{System.Single})`

Create vector from enumerable

### `M:Math.Main.Vector(System.Collections.Generic.IEnumerable{System.String})`

Create vector from enumerable

### `M:Math.Main.Vector(System.Double[])`

Create vector from variable length arguments

### `M:Math.Main.Vector(System.String)`

Create vector from string

### `M:Math.Vector1D.#ctor`

Construct empty.

### `M:Math.Vector1D.#ctor(System.Collections.Generic.IEnumerable{System.Boolean})`

Construct from (copy of) values.

### `M:Math.Vector1D.#ctor(System.Collections.Generic.IEnumerable{System.Double})`

Construct from (copy of) values.

### `M:Math.Vector1D.#ctor(System.Collections.Generic.IEnumerable{System.Int32})`

Construct from (copy of) values.

### `M:Math.Vector1D.#ctor(System.Collections.Generic.IEnumerable{System.Single})`

Construct from (copy of) values.

### `M:Math.Vector1D.#ctor(System.Collections.Generic.IEnumerable{System.String})`

Construct from (copy of) values.

### `M:Math.Vector1D.#ctor(System.Double[])`

Construct from param arguments.

### `M:Math.Vector1D.#ctor(System.String)`

Construct from string, either comma delimited or space delimited.

### `M:Math.Vector1D.Apply(System.Func{System.Double,System.Double})`

Apply element-wise arbitrary function

### `M:Math.Vector1D.Copy`

Make a copy

### `M:Math.Vector1D.Correlation(Math.Vector1D)`

Compute correlation

### `M:Math.Vector1D.Cos`

Compute cos element-wise

### `M:Math.Vector1D.Cosh`

Compute cosh element-wise

### `M:Math.Vector1D.Covariance(Math.Vector1D)`

Compute covariance

### `M:Math.Vector1D.Norm`

Another name for length

### `M:Math.Vector1D.op_Addition(Math.Vector1D,Math.Vector1D)`

Adds two vectors

### `M:Math.Vector1D.op_Addition(Math.Vector1D,System.Double)`

Adds a scalar to every element

### `M:Math.Vector1D.op_BitwiseOr(Math.Vector1D,Math.Vector1D)`

Append an entire vector

### `M:Math.Vector1D.op_BitwiseOr(Math.Vector1D,System.Double)`

Append an element

### `M:Math.Vector1D.op_Division(Math.Vector1D,Math.Vector1D)`

Divide element-wise

### `M:Math.Vector1D.op_Division(Math.Vector1D,System.Double)`

Multiply every element by a scalar

### `M:Math.Vector1D.op_ExclusiveOr(Math.Vector1D,System.Double)`

Exponent element-wise

### `M:Math.Vector1D.op_Multiply(Math.Vector1D,Math.Vector1D)`

Multiply element-wise

### `M:Math.Vector1D.op_Multiply(Math.Vector1D,System.Double)`

Multiply every element by a scalar

### `M:Math.Vector1D.op_Subtraction(Math.Vector1D,Math.Vector1D)`

Subtract two vectors

### `M:Math.Vector1D.op_Subtraction(Math.Vector1D,System.Double)`

Subtract a scalar to every element

### `M:Math.Vector1D.op_UnaryNegation(Math.Vector1D)`

Gets negative

### `M:Math.Vector1D.op_UnaryPlus(Math.Vector1D)`

Identity (no copy is made)

### `M:Math.Vector1D.Pow(System.Double)`

Compute pow element-wise

### `M:Math.Vector1D.Sin`

Compute sin element-wise

### `M:Math.Vector1D.Sinh`

Compute sinh element-wise

### `M:Math.Vector1D.Sqrt`

Compute sqrt element-wise

### `M:Math.Vector1D.ToString`

ToString override

### `P:Math.Vector1D.Average`

Get average (same as mean).

### `P:Math.Vector1D.Length`

Get length of vector

### `P:Math.Vector1D.Max`

Get max.

### `P:Math.Vector1D.Mean`

Get mean.

### `P:Math.Vector1D.Min`

Get min.

### `P:Math.Vector1D.PopulationSTD`

Get population std.

### `P:Math.Vector1D.PopulationVariance`

Get population variance.

### `P:Math.Vector1D.Raw`

Get raw data.

### `P:Math.Vector1D.Size`

Get string representation of size.

### `P:Math.Vector1D.STD`

Get std.

### `P:Math.Vector1D.Sum`

Get sum.

### `P:Math.Vector1D.Variance`

Get variance.

### `T:Math.Main`

Library entrance

### `T:Math.Vector1D`