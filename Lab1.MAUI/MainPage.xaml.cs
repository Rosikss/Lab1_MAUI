using Grid = Microsoft.Maui.Controls.Grid;
using System.Text;
using LabCalculator;


namespace Lab1.MAUI
{

    public partial class MainPage : ContentPage
    {
        private Dictionary<string, string> cellExpressions = new Dictionary<string, string>();
        private bool showExpression = false; 
        const int CountColumn = 20;
        const int CountRow = 30;

        public MainPage()
        {
            InitializeComponent();
            CreateGrid();
            InitializeGridWithZeros();
        }

        private void CreateGrid()
        {
            AddColumnsAndColumnLabels();
            AddRowsAndCellEntries();
        }

        private void AddColumnsAndColumnLabels()
        {
            for (int col = 0; col < CountColumn + 1; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = GetColumnName(col),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    Grid.SetRow(label, 0);
                    Grid.SetColumn(label, col);
                    grid.Children.Add(label);
                }
            }
        }

        private string GetColumnName(int colIndex)
        {
            StringBuilder columnName = new StringBuilder();
            while (colIndex > 0)
            {
                int modulo = (colIndex - 1) % 26;
                columnName.Insert(0, Convert.ToChar(65 + modulo));
                colIndex = (colIndex - modulo) / 26;
            }
            return columnName.ToString();
        }

        private void AddRowsAndCellEntries()
        {
            for (int row = 0; row < CountRow + 2; row++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                var label = new Label
                {
                    Text = (row + 1).ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                Grid.SetRow(label, row + 1);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);
                for (int col = 0; col < CountColumn; col++)
                {
                    var entry = new Entry
                    {
                        Text = "",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    entry.Unfocused += Entry_Unfocused;
                    Grid.SetRow(entry, row + 1);
                    Grid.SetColumn(entry, col + 1);
                    grid.Children.Add(entry);
                }
            }
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                cellExpressions[cellId] = entry.Text;
            }
        }

        private void InitializeGridWithZeros()
        {
            foreach (Entry entry in grid.Children.OfType<Entry>())
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                cellExpressions[cellId] = "0";
                entry.Text = "0";
            }
        }
        private void AddRowButton_Clicked(object sender, EventArgs e)
        {
            int newRow = grid.RowDefinitions.Count;
            grid.RowDefinitions.Add(new RowDefinition());

            var label = new Label
            {
                Text = newRow.ToString(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, newRow);
            Grid.SetColumn(label, 0);
            grid.Children.Add(label);

            for (int col = 1; col < grid.ColumnDefinitions.Count; col++)
            {
                var entry = new Entry
                {
                    Text = "0",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, newRow);
                Grid.SetColumn(entry, col);
                grid.Children.Add(entry);
                var cellId = $"{GetColumnName(col)}{newRow}";
                cellExpressions[cellId] = "0";
            }
        }

        private void AddColumnButton_Clicked(object sender, EventArgs e)
        {
            int newColumn = grid.ColumnDefinitions.Count;
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var label = new Label
            {
                Text = GetColumnName(newColumn),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, newColumn);
            grid.Children.Add(label);

            for (int row = 1; row < grid.RowDefinitions.Count; row++)
            {
                var entry = new Entry
                {
                    Text = "0",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                };
                entry.Unfocused += Entry_Unfocused;
                Grid.SetRow(entry, row);
                Grid.SetColumn(entry, newColumn);
                grid.Children.Add(entry);
                var cellId = $"{GetColumnName(newColumn)}{row}";
                cellExpressions[cellId] = "0";
            }
        }

        private void DeleteRowButton_Clicked(object sender, EventArgs e)
        {
            if (grid.RowDefinitions.Count > 1)
            {
                int lastRowIndex = grid.RowDefinitions.Count - 1;


                var entriesToDelete = grid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) == lastRowIndex).ToList();
                foreach (var entry in entriesToDelete)
                {
                    var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                    var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                    cellExpressions.Remove(cellId);
                    grid.Children.Remove(entry);
                }


                var rowLabelToDelete = grid.Children.OfType<Label>().FirstOrDefault(l => Grid.GetRow(l) == lastRowIndex && Grid.GetColumn(l) == 0);
                if (rowLabelToDelete != null)
                {
                    grid.Children.Remove(rowLabelToDelete);
                }


                foreach (var label in grid.Children.OfType<Label>().Where(l => Grid.GetRow(l) > lastRowIndex && Grid.GetColumn(l) == 0))
                {   
                    Grid.SetRow(label, Grid.GetRow(label) - 1);
                    label.Text = (Grid.GetRow(label)).ToString();
                }

                grid.RowDefinitions.RemoveAt(lastRowIndex);
            }
        }

        private void DeleteColumnButton_Clicked(object sender, EventArgs e)
        {
            if (grid.ColumnDefinitions.Count > 1)
            {
                int lastColumnIndex = grid.ColumnDefinitions.Count - 1;


                var entriesToDelete = grid.Children.OfType<Entry>().Where(e => Grid.GetColumn(e) == lastColumnIndex).ToList();
                foreach (var entry in entriesToDelete)
                {
                    var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                    var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                    cellExpressions.Remove(cellId);
                    grid.Children.Remove(entry);
                }


                var columnLabelToDelete = grid.Children.OfType<Label>().FirstOrDefault(l => Grid.GetRow(l) == 0 && Grid.GetColumn(l) == lastColumnIndex);
                if (columnLabelToDelete != null)
                {
                    grid.Children.Remove(columnLabelToDelete);
                }

                grid.ColumnDefinitions.RemoveAt(lastColumnIndex);
            }
        }

        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            await GoogleDriveManager.InitClientAsync();
            string fileId = await DisplayPromptAsync("Введіть ID файлу", "Будь ласка, введіть ID файлу, який ви хочете завантажити:");
            if (string.IsNullOrWhiteSpace(fileId))
            {
                await DisplayAlert("Помилка", "Введений ID файлу є неправильним.", "OK");
                return;
            }

            string fileContent = await GoogleDriveManager.ReadFileContentAsync(fileId);
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                await DisplayAlert("Помилка", "Неможливо завантажити вміст файлу.", "OK");
                return;
            }

            PopulateGridWithData(grid, fileContent);
        }

        private void PopulateGridWithData(Grid grid, string fileContent)
        {
            var lines = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var gridSize = lines[0].Split(';');
            if (gridSize.Length == 2 && int.TryParse(gridSize[0], out int rows) && int.TryParse(gridSize[1], out int columns))
            {
                UpdateGridSize(grid, rows, columns);
            }

            var cells = lines[1].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int cellIndex = 0;

            foreach (Entry entry in grid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) > 0))
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";

                if (cellIndex < cells.Length)
                {
                    var cellData = cells[cellIndex].Split('|');
                    var expression = cellData.Length > 0 ? cellData[0] : "";
                    var value = cellData.Length > 1 ? cellData[1] : "";

                    cellExpressions[cellId] = expression;
                    entry.Text = showExpression ? expression : value;

                    cellIndex++;
                }
            }
        }
        
        private void UpdateGridSize(Grid grid, int rows, int columns)
        {
            while (grid.RowDefinitions.Count < rows) AddRowButton_Clicked(null, null);
            while (grid.ColumnDefinitions.Count < columns) AddColumnButton_Clicked(null, null);
            while (grid.ColumnDefinitions.Count > columns) DeleteColumnButton_Clicked(null, null);
            while (grid.RowDefinitions.Count > rows) DeleteRowButton_Clicked(null, null);
        }

        private void CalculateButton_Clicked(object sender, EventArgs e)
        {
            foreach (Entry entry in grid.Children.OfType<Entry>())
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                if (cellExpressions.TryGetValue(cellId, out string expression))
                {
                    try
                    {
                        double result = Calculator.Evaluate(expression, cellExpressions);
                        if (double.IsNaN(result))
                        {
                            entry.Text = "Error";
                        }
                        else
                        {
                            entry.Text = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        entry.Text = "Error";
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        private void ToggleModeButton_Clicked(object sender, EventArgs e)
        {
            showExpression = !showExpression;
            UpdateCellDisplays();
        }

        private void UpdateCellDisplays()
        {
            foreach (Entry entry in grid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) > 0))
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                if (cellExpressions.ContainsKey(cellId))
                {
                    entry.Text = showExpression
                        ? cellExpressions[cellId]
                        : Calculator.Evaluate(cellExpressions[cellId], cellExpressions).ToString();
                }
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            await GoogleDriveManager.InitClientAsync();

            string fileName = await DisplayPromptAsync("Введіть назву файлу", "Будь ласка, введіть назву для файлу, який ви хочете зберегти:");
            if (string.IsNullOrWhiteSpace(fileName))
            {
                await DisplayAlert("Помилка", "Введена назва файлу є неправильною.", "OK");
                return;
            }

            string data = ConvertGridToText(grid);
            string resultMessage = await GoogleDriveManager.WriteFileAsync(fileName, data);
            var idStart = resultMessage.LastIndexOf(":") + 2;
            var fileId = resultMessage.Substring(idStart);
            fileIdEntry.Text = fileId;
            await DisplayAlert("Результат", resultMessage, "OK");
        }

        private string ConvertGridToText(Grid grid)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{grid.RowDefinitions.Count};{grid.ColumnDefinitions.Count}");
            foreach (Entry entry in grid.Children.OfType<Entry>().Where(e => Grid.GetRow(e) > 0))
            {
                var (row, col) = (Grid.GetRow(entry) - 1, Grid.GetColumn(entry) - 1);
                var cellId = $"{GetColumnName(col + 1)}{row + 1}";
                var expression = cellExpressions.ContainsKey(cellId) ? cellExpressions[cellId] : "";
                sb.Append($"{expression}|{entry.Text};");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        private async void HelpButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Довідка", "Лабораторна робота 1.Росновського Ярослава Сергійовича", "OK");
        }

        private async void ExitButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Підтвердження", "Ви дійсно хочете вийти ? ", "Так", "Ні");
            if (answer) System.Environment.Exit(0);
        }
    }
}