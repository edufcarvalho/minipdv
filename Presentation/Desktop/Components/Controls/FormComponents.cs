namespace minipdv.Presentation.Desktop.Components.Controls;

public static class FormComponents
{
    public static readonly Font DefaultFont = new("Segoe UI", 10);

    public static Button CreateRefreshButton(int width = 90)
    {
        return new Button { Text = "Atualizar", Width = width, Height = 32, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, TextAlign = ContentAlignment.MiddleCenter };
    }

    public static Button CreateAddButton(int width = 90)
    {
        return new Button { Text = "Adicionar", Width = width, Height = 32, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };
    }

    public static Button CreateEditButton(int width = 90)
    {
        return new Button { Text = "Editar", Width = width, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };
    }

    public static Button CreateDeleteButton(int width = 90, string text = "Excluir")
    {
        return new Button { Text = text, Width = width, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };
    }

    public static Button CreateSaveButton(int width = 80)
    {
        return new Button { Text = "Salvar", Width = width, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };
    }

    public static Button CreateCancelButton(int width = 80)
    {
        return new Button { Text = "Cancelar", Width = width, Height = 32, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, TextAlign = ContentAlignment.MiddleCenter, DialogResult = DialogResult.Cancel };
    }

    public static Button CreateCloseButton(int width = 80)
    {
        return new Button { Text = "Fechar", Width = width, Height = 32, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, BackColor = SystemColors.Control, TextAlign = ContentAlignment.MiddleCenter, DialogResult = DialogResult.Cancel };
    }

    public static DataGridView CreateDataGridView()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            Font = DefaultFont
        };
    }

    public static TableLayoutPanel CreateStandardLayout(int toolbarHeight = 45, int statusHeight = 35)
    {
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, toolbarHeight));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, statusHeight));
        return tbl;
    }

    public static FlowLayoutPanel CreateToolbar()
    {
        return new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
    }

    public static FlowLayoutPanel CreateStatusBar(DataGridView dgv)
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        panel.Controls.Add(lblCount);
        return panel;
    }

    public static Form CreateDialog(string title, int width, int height)
    {
        return new Form
        {
            Text = title,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(width, height)
        };
    }

    public static TableLayoutPanel CreateDialogLayout(int columns, int rows, int labelWidth, int padding = 15)
    {
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = columns, RowCount = rows, Padding = new Padding(padding) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, labelWidth));
        for (int i = 1; i < columns; i++)
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        return tbl;
    }

    public static FlowLayoutPanel CreateDialogButtonPanel()
    {
        return new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
    }

    public static void BlockSpaceChar(TextBox txt)
    {
        txt.KeyPress += (_, e) => { if (e.KeyChar == ' ') e.Handled = true; };
    }

    public static Label CreateFieldLabel(string text)
    {
        return new Label { Text = text, TextAlign = ContentAlignment.MiddleLeft };
    }

    public static TextBox CreateReadOnlyTextBox(string text = "")
    {
        return new TextBox { Text = text, ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = DefaultFont };
    }

    public static TextBox CreateTextBox(string text = "")
    {
        return new TextBox { Text = text, Dock = DockStyle.Fill, Font = DefaultFont };
    }

    public static TextBox CreatePasswordTextBox()
    {
        return new TextBox { Dock = DockStyle.Fill, Font = DefaultFont, UseSystemPasswordChar = true };
    }

    public static CheckBox CreateCheckBox(string text, bool @checked = false, bool enabled = true)
    {
        return new CheckBox { Text = text, Checked = @checked, Enabled = enabled, Dock = DockStyle.Fill, Font = DefaultFont };
    }
}
