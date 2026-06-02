using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Clientes;

public class ClientesForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Cliente> _clientes = [];

    public ClientesForm()
    {
        Text = "Clientes";
        Dock = DockStyle.Fill;

        var tbl = FormComponents.CreateStandardLayout();
        var topPanel = FormComponents.CreateToolbar();

        var btnRefresh = FormComponents.CreateRefreshButton();
        btnRefresh.Click += async (_, _) => await LoadData();
        topPanel.Controls.Add(btnRefresh);
        topPanel.Controls.Add(new Label { Width = 10 });

        var btnAdd = FormComponents.CreateAddButton();
        btnAdd.Click += async (_, _) => await AddItem();
        topPanel.Controls.Add(btnAdd);

        var btnEdit = FormComponents.CreateEditButton();
        btnEdit.Click += async (_, _) => await EditItem();
        topPanel.Controls.Add(btnEdit);

        var btnDelete = FormComponents.CreateDeleteButton();
        btnDelete.Click += async (_, _) => await DeleteItem();
        topPanel.Controls.Add(btnDelete);
        topPanel.Controls.Add(new Label { Width = 10 });
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = FormComponents.CreateDataGridView();
        dgv.CellDoubleClick += async (_, _) => await ViewItem();
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
            _clientes = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Nome", "Nome");
            dgv.Columns.Add("Cpf", "CPF");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Telefone", "Telefone");
            dgv.Rows.Clear();
            foreach (var c in _clientes)
                dgv.Rows.Add(c.Id, c.Nome, c.Cpf, c.Contato?.Email ?? "", c.Contato?.Telefone ?? "");
            _searchFilter.ApplyFilter();
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

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = FormComponents.CreateDialog("Visualizar Cliente", 400, 240);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Nome), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CPF:"), 0, 1);
        var mtxtCpf = new MaskedTextBox { Mask = "000.000.000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cpf, Enabled = false, ReadOnly = true, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCpf, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 2);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Email ?? ""), 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 3);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Telefone ?? ""), 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        var btnDelete = FormComponents.CreateDeleteButton(80);
        var btnClose = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 4);

        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Cliente", 400, 240);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CPF:"), 0, 1);
        var mtxtCpf = new MaskedTextBox { Mask = "000.000.000-00", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCpf, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 2);
        var txtEmail = FormComponents.CreateTextBox();
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 3);
        var txtTelefone = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtTelefone, 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        btnOk.Click += async (_, _) =>
        {
            var nome = txtNome.Text.Trim();
            var cpf = mtxtCpf.Text.Trim();
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cpf))
            {
                MessageBox.Show("Nome e CPF são obrigatórios.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dialog.Enabled = false;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(null, email, telefone);

                var response = await ApiClient.Instance.PostAsync("api/clientes", new { nome, cpf, contatoId });
                if (response.IsSuccessStatusCode)
                {
                    txtNome.Clear(); mtxtCpf.Clear(); txtEmail.Clear(); txtTelefone.Clear();
                    await LoadData();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    var err = await ErrorHelper.ExtractAsync(response);
                    MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dialog.Enabled = true;
            }
        };
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        dialog.ShowDialog(this);
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

        using var dialog = FormComponents.CreateDialog("Editar Cliente", 400, 240);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox(item.Nome);
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CPF:"), 0, 1);
        var mtxtCpf = new MaskedTextBox { Mask = "000.000.000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cpf, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCpf, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 2);
        var txtEmail = FormComponents.CreateTextBox(contato?.Email ?? "");
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 3);
        var txtTelefone = FormComponents.CreateTextBox(contato?.Telefone ?? "");
        tbl.Controls.Add(txtTelefone, 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(item.ContatoId, email, telefone);

                var response = await ApiClient.Instance.PutAsync($"api/clientes/{item.Id}", new { id = item.Id, nome = txtNome.Text.Trim(), cpf = mtxtCpf.Text.Trim(), contatoId });
                if (response.IsSuccessStatusCode)
                {
                    txtNome.Clear(); mtxtCpf.Clear(); txtEmail.Clear(); txtTelefone.Clear();
                    await LoadData();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    var err = await ErrorHelper.ExtractAsync(response);
                    MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dialog.Enabled = true;
            }
        };
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 4);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        dialog.ShowDialog(this);
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
                var err = await ErrorHelper.ExtractAsync(response);
                MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
