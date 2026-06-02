using System.Globalization;
using System.Text.Json;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Products;

public class FabricantesForm : Form
{
    private readonly DataGridView dgv;
    private List<Fabricante> _items = [];

    public FabricantesForm()
    {
        Text = "Fabricantes";
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
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
        dgv.CellDoubleClick += (_, _) => ViewItem();
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
            _items = await ApiClient.Instance.GetAsync<List<Fabricante>>("api/fabricantes") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("NomeFantasia", "Nome Fantasia");
            dgv.Columns.Add("RazaoSocial", "Razão Social");
            dgv.Columns.Add("Cnpj", "CNPJ");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Telefone", "Telefone");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.NomeFantasia, item.RazaoSocial, item.Cnpj, item.Contato?.Email ?? "", item.Contato?.Telefone ?? "");
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private Fabricante? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _items.Count)
            return _items[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um fabricante.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = new Form { Text = "Visualizar Fabricante", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 280) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome Fantasia:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNF = new TextBox { Text = item.NomeFantasia, ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNF, 1, 0);

        tbl.Controls.Add(new Label { Text = "Razão Social:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtRS = new TextBox { Text = item.RazaoSocial, ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtRS, 1, 1);

        tbl.Controls.Add(new Label { Text = "CNPJ:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cnpj, Enabled = false, ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtEmail = new TextBox { Text = contato?.Email ?? "", ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtEmail, 1, 3);

        tbl.Controls.Add(new Label { Text = "Telefone:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var txtTelefone = new TextBox { Text = contato?.Telefone ?? "", ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtTelefone, 1, 4);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = new Button { Text = "Editar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnDelete = new Button { Text = "Excluir", Width = 80, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnClose = new Button { Text = "Fechar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 5);
        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) =>
        {
            dialog.Close();
            await EditItem();
        };
        btnDelete.Click += async (_, _) =>
        {
            dialog.Close();
            await DeleteItem();
        };

        dialog.ShowDialog(this);
    }

    private async Task<int?> CreateOrUpdateContatoAsync(int? contatoId, string email, string telefone)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(telefone))
            return contatoId;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (contatoId.HasValue)
        {
            await ApiClient.Instance.PutAsync($"api/contatos/{contatoId.Value}", new { id = contatoId.Value, email, telefone });
            return contatoId;
        }

        var response = await ApiClient.Instance.PostAsync("api/contatos", new { email, telefone });
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var contato = JsonSerializer.Deserialize<Contato>(json, jsonOptions);
        return contato?.Id;
    }

    private async Task AddItem()
    {
        using var dialog = new Form { Text = "Novo Fabricante", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 280) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome Fantasia:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNF = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNF, 1, 0);

        tbl.Controls.Add(new Label { Text = "Razão Social:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtRS = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtRS, 1, 1);

        tbl.Controls.Add(new Label { Text = "CNPJ:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtEmail = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        txtEmail.KeyPress += (_, e) => { if (e.KeyChar == ' ') e.Handled = true; };
        tbl.Controls.Add(txtEmail, 1, 3);

        tbl.Controls.Add(new Label { Text = "Telefone:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var txtTelefone = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtTelefone, 1, 4);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 5);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await CreateOrUpdateContatoAsync(null, email, telefone);

                var cnpj = mtxtCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
                var response = await ApiClient.Instance.PostAsync("api/fabricantes", new { nomeFantasia = txtNF.Text.Trim(), razaoSocial = txtRS.Text.Trim(), cnpj, contatoId });
                if (response.IsSuccessStatusCode)
                {
                    txtNF.Clear(); txtRS.Clear(); mtxtCNPJ.Clear(); txtEmail.Clear(); txtTelefone.Clear();
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
        if (item == null) { MessageBox.Show("Selecione um fabricante.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = new Form { Text = "Editar Fabricante", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(400, 280) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Nome Fantasia:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtNF = new TextBox { Text = item.NomeFantasia, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtNF, 1, 0);

        tbl.Controls.Add(new Label { Text = "Razão Social:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtRS = new TextBox { Text = item.RazaoSocial, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtRS, 1, 1);

        tbl.Controls.Add(new Label { Text = "CNPJ:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cnpj, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtEmail = new TextBox { Text = contato?.Email ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        txtEmail.KeyPress += (_, e) => { if (e.KeyChar == ' ') e.Handled = true; };
        tbl.Controls.Add(txtEmail, 1, 3);

        tbl.Controls.Add(new Label { Text = "Telefone:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var txtTelefone = new TextBox { Text = contato?.Telefone ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtTelefone, 1, 4);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 5);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await CreateOrUpdateContatoAsync(item.ContatoId, email, telefone);

                var cnpj = mtxtCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
                var response = await ApiClient.Instance.PutAsync($"api/fabricantes/{item.Id}", new { id = item.Id, nomeFantasia = txtNF.Text.Trim(), razaoSocial = txtRS.Text.Trim(), cnpj, contatoId });
                if (response.IsSuccessStatusCode)
                {
                    txtNF.Clear(); txtRS.Clear(); mtxtCNPJ.Clear(); txtEmail.Clear(); txtTelefone.Clear();
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
        if (item == null) { MessageBox.Show("Selecione um fabricante.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir fabricante '{item.NomeFantasia}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/fabricantes/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
