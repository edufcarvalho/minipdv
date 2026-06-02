namespace minipdv.Presentation.Desktop.Components.Modals;

public class InputDialog : Form
{
    private readonly TableLayoutPanel tbl;
    private readonly Button btnOk;
    private readonly Button btnCancel;
    public Dictionary<string, Control> Fields { get; } = [];

    public InputDialog(string title, params (string Label, string? DefaultValue)[] fields)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(400, 50 + fields.Length * 45);
        ShowInTaskbar = false;

        tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = fields.Length + 1,
            Padding = new Padding(15),
            AutoSize = true
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (int i = 0; i < fields.Length; i++)
        {
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            tbl.Controls.Add(new Label
            {
                Text = fields[i].Label,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            }, 0, i);

            var txt = new TextBox
            {
                Text = fields[i].DefaultValue ?? "",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };
            Fields[fields[i].Label] = txt;
            tbl.Controls.Add(txt, 1, i);
        }

        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };
        tbl.SetColumnSpan(btnPanel, 2);

        btnOk = new Button
        {
            Text = "OK",
            Width = 80,
            Height = 32,
            BackColor = Color.DarkBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnOk.Click += (_, _) => DialogResult = DialogResult.OK;

        btnCancel = new Button
        {
            Text = "Cancelar",
            Width = 80,
            Height = 32,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 10, 0)
        };
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        btnPanel.Controls.Add(btnOk);
        btnPanel.Controls.Add(btnCancel);

        tbl.Controls.Add(btnPanel, 0, fields.Length);

        Controls.Add(tbl);
        AcceptButton = btnOk;
        CancelButton = btnCancel;
    }

    public string GetValue(string label)
    {
        return Fields.TryGetValue(label, out var ctrl) ? ctrl.Text.Trim() : "";
    }

    public static string? Show(IWin32Window owner, string title, params (string Label, string? DefaultValue)[] fields)
    {
        using var dialog = new InputDialog(title, fields);
        return dialog.ShowDialog(owner) == DialogResult.OK ? dialog.GetValue(fields[0].Label) : null;
    }
}
