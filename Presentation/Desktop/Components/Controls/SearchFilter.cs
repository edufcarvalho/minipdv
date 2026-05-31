using System.ComponentModel;

namespace minipdv.Presentation.Desktop.Components.Controls;

public class SearchFilter
{
    private readonly TextBox _txtSearch;
    private readonly System.Windows.Forms.Timer _debounceTimer;
    private readonly DataGridView _dgv;

    public SearchFilter(Panel parent, DataGridView dgv)
    {
        _dgv = dgv;

        _txtSearch = new TextBox
        {
            Width = 250,
            Height = 32,
            Font = new Font("Segoe UI", 10),
            PlaceholderText = "Buscar..."
        };
        parent.Controls.Add(_txtSearch);

        _debounceTimer = new System.Windows.Forms.Timer { Interval = 300 };
        _debounceTimer.Tick += (_, _) => ApplyFilter();

        _txtSearch.TextChanged += (_, _) =>
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        };
    }

    public TextBox TextBox => _txtSearch;

    public void ApplyFilter()
    {
        _debounceTimer.Stop();
        var filter = _txtSearch.Text.Trim();

        foreach (DataGridViewRow row in _dgv.Rows)
        {
            if (row.IsNewRow) continue;

            if (string.IsNullOrEmpty(filter))
            {
                row.Visible = true;
                continue;
            }

            var match = false;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString()!.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    match = true;
                    break;
                }
            }
            row.Visible = match;
        }
    }
}
