using minipdv.Domain.Entities;

namespace minipdv.Presentation.Desktop.Forms.Services;

public class ReceitasForm : Form
{
    private readonly DataGridView dgv;
    private List<Receita> _items = [];

    public ReceitasForm()
    {
        Text = "Receitas";
        Dock = DockStyle.Fill;

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var btnRefresh = new Button { Text = "Atualizar", Width = 90, Height = 32, Cursor = Cursors.Hand };
        btnRefresh.Click += async (_, _) => await LoadData();
        topPanel.Controls.Add(btnRefresh);
        topPanel.Controls.Add(new Label { Width = 10 });
        var btnAdd = new Button { Text = "Adicionar", Width = 90, Height = 32, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnAdd.Click += async (_, _) => await AddItem();
        topPanel.Controls.Add(btnAdd);
        var btnDelete = new Button { Text = "Excluir", Width = 90, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnDelete.Click += async (_, _) => await DeleteItem();
        topPanel.Controls.Add(btnDelete);
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(dgv, 0, 1);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        statusPanel.Controls.Add(lblCount);
        tbl.Controls.Add(statusPanel, 0, 2);

        Controls.Add(tbl);
        Load += async (_, _) => await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            _items = await ApiClient.Instance.GetAsync<List<Receita>>("api/receitas") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("PrescritorId", "Prescritor");
            dgv.Columns.Add("PacienteId", "Paciente");
            dgv.Columns.Add("CompradorId", "Comprador");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.PrescritorId, item.PacienteId, item.CompradorId);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task AddItem()
    {
        using var dialog = new Form { Text = "Nova Receita", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 200) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Prescritor ID:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtPresc = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtPresc, 1, 0);

        tbl.Controls.Add(new Label { Text = "Paciente ID:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtPac = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtPac, 1, 1);

        tbl.Controls.Add(new Label { Text = "Comprador ID:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtComp = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtComp, 1, 2);

        tbl.Controls.Add(new Label { Text = "Produto IDs:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtProd = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtProd, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        if (!int.TryParse(txtPresc.Text.Trim(), out var prescId) ||
            !int.TryParse(txtPac.Text.Trim(), out var pacId) ||
            !int.TryParse(txtComp.Text.Trim(), out var compId))
        {
            MessageBox.Show("IDs inválidos.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var response = await ApiClient.Instance.PostAsync("api/receitas", new
            {
                prescritorId = prescId,
                pacienteId = pacId,
                compradorId = compId,
                produtos = (List<object>?)null
            });
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await response.Content.ReadAsStringAsync()}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task DeleteItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma receita.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        if (MessageBox.Show($"Excluir receita #{item.Id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/receitas/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await response.Content.ReadAsStringAsync()}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
