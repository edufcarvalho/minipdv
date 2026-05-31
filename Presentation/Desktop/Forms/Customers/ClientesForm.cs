using System.Globalization;
using System.Text.Json;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.Desktop.Forms.Customers;

public class ClientesForm : Form
{
    private readonly DataGridView dgv;
    private readonly TextBox txtSearch;
    private List<Cliente> _clientes = [];

    public ClientesForm()
    {
        Text = "Clientes";
        Dock = DockStyle.Fill;

        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(10)
        };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

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

        tbl.Controls.Add(topPanel, 0, 0);

        var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        searchPanel.Controls.Add(new Label { Text = "Buscar:", TextAlign = ContentAlignment.MiddleLeft, Width = 55, Height = 32 });
        txtSearch = new TextBox { Width = 300, Height = 32, Font = new Font("Segoe UI", 10) };
        txtSearch.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await LoadData(); } };
        searchPanel.Controls.Add(txtSearch);
        tbl.Controls.Add(searchPanel, 0, 1);

        dgv = new DataGridView
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
            Font = new Font("Segoe UI", 10)
        };
        tbl.Controls.Add(dgv, 0, 2);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        statusPanel.Controls.Add(lblCount);
        tbl.Controls.Add(statusPanel, 0, 3);

        Controls.Add(tbl);
        Load += async (_, _) => await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var all = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes") ?? [];
            var search = txtSearch.Text.Trim();
            _clientes = string.IsNullOrEmpty(search)
                ? all
                : all.Where(c => c.Nome.Contains(search, StringComparison.OrdinalIgnoreCase) || c.Cpf.Contains(search)).ToList();

            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Nome", "Nome");
            dgv.Columns.Add("Cpf", "CPF");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Telefone", "Telefone");

            dgv.Rows.Clear();
            foreach (var c in _clientes)
                dgv.Rows.Add(c.Id, c.Nome, c.Cpf, c.Contato?.Email ?? "", c.Contato?.Telefone ?? "");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Cliente? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _clientes.Count)
            return _clientes[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task<int?> CreateOrUpdateContatoAsync(int? contatoId, string email, string telefone)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(telefone))
            return contatoId;

        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };

        if (contatoId.HasValue)
        {
            var payload = new { id = contatoId.Value, email, telefone };
            var response = await ApiClient.Instance.PutAsync($"api/contatos/{contatoId.Value}", payload);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar contato: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return contatoId;
        }

        var payload2 = new { email, telefone };
        var response2 = await ApiClient.Instance.PostAsync("api/contatos", payload2);
        if (!response2.IsSuccessStatusCode)
        {
            var err2 = await response2.Content.ReadAsStringAsync();
            MessageBox.Show($"Erro ao criar contato: {err2}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }

        var json = await response2.Content.ReadAsStringAsync();
        var contato = System.Text.Json.JsonSerializer.Deserialize<Contato>(json, jsonOptions);
        return contato?.Id;
    }

    private async Task AddItem()
    {
        using var dialog = new Form
        {
            Text = "Novo Cliente",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(400, 240)
        };

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNome = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(new Label { Text = "CPF:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var mtxtCpf = new MaskedTextBox { Mask = "000.000.000-00", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtCpf, 1, 1);

        tbl.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtEmail = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        txtEmail.KeyPress += (_, e) => { if (e.KeyChar == ' ') e.Handled = true; };
        tbl.Controls.Add(txtEmail, 1, 2);

        tbl.Controls.Add(new Label { Text = "Telefone:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtTelefone = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtTelefone, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk);
        btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        var nome = txtNome.Text.Trim();
        var cpf = mtxtCpf.Text.Trim();
        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cpf))
        {
            MessageBox.Show("Nome e CPF são obrigatórios.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var email = txtEmail.Text.Trim();
            var telefone = txtTelefone.Text.Trim();
            var contatoId = await CreateOrUpdateContatoAsync(null, email, telefone);

            var response = await ApiClient.Instance.PostAsync("api/clientes", new { nome, cpf, contatoId });
            if (response.IsSuccessStatusCode)
                await LoadData();
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task EditItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = new Form
        {
            Text = "Editar Cliente",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(400, 240)
        };

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNome = new TextBox { Text = item.Nome, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(new Label { Text = "CPF:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var mtxtCpf = new MaskedTextBox { Mask = "000.000.000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cpf, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtCpf, 1, 1);

        tbl.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtEmail = new TextBox { Text = contato?.Email ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        txtEmail.KeyPress += (_, e) => { if (e.KeyChar == ' ') e.Handled = true; };
        tbl.Controls.Add(txtEmail, 1, 2);

        tbl.Controls.Add(new Label { Text = "Telefone:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtTelefone = new TextBox { Text = contato?.Telefone ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtTelefone, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk);
        btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var email = txtEmail.Text.Trim();
            var telefone = txtTelefone.Text.Trim();
            var contatoId = await CreateOrUpdateContatoAsync(item.ContatoId, email, telefone);

            var response = await ApiClient.Instance.PutAsync($"api/clientes/{item.Id}", new { id = item.Id, nome = txtNome.Text.Trim(), cpf = mtxtCpf.Text.Trim(), contatoId });
            if (response.IsSuccessStatusCode)
                await LoadData();
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task DeleteItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        if (MessageBox.Show($"Excluir cliente '{item.Nome}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/clientes/{item.Id}");
            if (response.IsSuccessStatusCode)
                await LoadData();
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
