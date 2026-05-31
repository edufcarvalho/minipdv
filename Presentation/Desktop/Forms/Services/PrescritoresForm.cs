using minipdv.Domain.Entities;
using minipdv.Domain.Enums;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Services;

public class PrescritoresForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Prescritor> _items = [];

    public PrescritoresForm()
    {
        Text = "Prescritores";
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
        var btnEdit = new Button { Text = "Editar", Width = 90, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnEdit.Click += async (_, _) => await EditItem();
        topPanel.Controls.Add(btnEdit);
        var btnDelete = new Button { Text = "Excluir", Width = 90, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnDelete.Click += async (_, _) => await DeleteItem();
        topPanel.Controls.Add(btnDelete);
        topPanel.Controls.Add(new Label { Width = 10 });
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(dgv, 0, 1);
        _searchFilter = new SearchFilter(topPanel, dgv);

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
            _items = await ApiClient.Instance.GetAsync<List<Prescritor>>("api/prescritores") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Nome", "Nome");
            dgv.Columns.Add("Numero", "Número");
            dgv.Columns.Add("Conselho", "Conselho");
            dgv.Columns.Add("UF", "UF");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.Nome, item.Numero, item.Conselho, item.Uf);
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private Prescritor? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _items.Count)
            return _items[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task AddItem()
    {
        using var dialog = new Form { Text = "Novo Prescritor", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 200) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNome = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(new Label { Text = "Número:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtNumero = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNumero, 1, 1);

        tbl.Controls.Add(new Label { Text = "Conselho:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var cmbConselho = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbConselho.DataSource = Enum.GetValues<Conselho>().Cast<object>().ToList();
        tbl.Controls.Add(cmbConselho, 1, 2);

        tbl.Controls.Add(new Label { Text = "UF:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var cmbUf = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbUf.DataSource = Enum.GetValues<UF>().Cast<object>().ToList();
        tbl.Controls.Add(cmbUf, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;

        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PostAsync("api/prescritores", new
                {
                    nome = txtNome.Text.Trim(),
                    numero = txtNumero.Text.Trim(),
                    conselho = cmbConselho.SelectedValue?.ToString() ?? "",
                    uf = cmbUf.SelectedValue?.ToString() ?? ""
                });
                if (response.IsSuccessStatusCode)
                {
                    txtNome.Clear(); txtNumero.Clear();
                    cmbConselho.ClearSelection(); cmbUf.ClearSelection();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                    await LoadData();
                }
                else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };

        dialog.ShowDialog(this);
    }

    private async Task EditItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um prescritor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = new Form { Text = "Editar Prescritor", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 200) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNome = new TextBox { Text = item.Nome, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(new Label { Text = "Número:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtNumero = new TextBox { Text = item.Numero, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNumero, 1, 1);

        tbl.Controls.Add(new Label { Text = "Conselho:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var cmbConselho = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbConselho.DataSource = Enum.GetValues<Conselho>().Cast<object>().ToList();
        cmbConselho.SelectedValue = item.Conselho;
        tbl.Controls.Add(cmbConselho, 1, 2);

        tbl.Controls.Add(new Label { Text = "UF:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var cmbUf = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbUf.DataSource = Enum.GetValues<UF>().Cast<object>().ToList();
        cmbUf.SelectedValue = item.Uf;
        tbl.Controls.Add(cmbUf, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        var capturedItem = item;

        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PutAsync($"api/prescritores/{capturedItem.Id}", new
                {
                    id = capturedItem.Id,
                    nome = txtNome.Text.Trim(),
                    numero = txtNumero.Text.Trim(),
                    conselho = cmbConselho.SelectedValue?.ToString() ?? "",
                    uf = cmbUf.SelectedValue?.ToString() ?? ""
                });
                if (response.IsSuccessStatusCode)
                {
                    txtNome.Clear(); txtNumero.Clear();
                    cmbConselho.ClearSelection(); cmbUf.ClearSelection();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                    await LoadData();
                }
                else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };

        dialog.ShowDialog(this);
    }

    private async Task DeleteItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um prescritor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir prescritor '{item.Nome}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/prescritores/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
