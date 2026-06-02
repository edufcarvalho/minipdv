using System.Text.Json;
using minipdv.Application.DTOs.Auth;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Autenticacao;

public class UsuariosForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Usuario> _items = [];

    public UsuariosForm()
    {
        Text = "Usuários";
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

        tbl.Controls.Add(FormComponents.CreateStatusBar(dgv), 0, 2);

        Controls.Add(tbl);
        Load += async (_, _) => await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            _items = await ApiClient.Instance.GetAsync<List<Usuario>>("api/usuarios") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Nome", "Nome");
            dgv.Columns.Add("Login", "Login");
            dgv.Columns.Add("Tipo", "Tipo");
            dgv.Columns.Add("Ativo", "Ativo");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Telefone", "Telefone");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.Nome, item.Login, item.TipoUsuario, item.Ativo ? "Sim" : "Não", item.Contato?.Email ?? "", item.Contato?.Telefone ?? "");
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private Usuario? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _items.Count)
            return _items[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um usuário.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = FormComponents.CreateDialog("Visualizar Usuário", 400, 280);
        var tbl = FormComponents.CreateDialogLayout(2, 7, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Nome), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Login:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Login), 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Tipo:"), 0, 2);
        var cmbTipo = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbTipo.DataSource = new List<string> { "Usuario", "Farmaceutico", "Administrador" };
        cmbTipo.SelectedValue = item.TipoUsuario;
        tbl.Controls.Add(cmbTipo, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 3);
        tbl.Controls.Add(FormComponents.CreateCheckBox("Ativo", item.Ativo, enabled: false), 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 4);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Email ?? ""), 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 5);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Telefone ?? ""), 1, 5);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        var btnDelete = FormComponents.CreateDeleteButton(80);
        var btnClose = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 6);

        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Usuário", 400, 280);
        var tbl = FormComponents.CreateDialogLayout(2, 7, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Login:"), 0, 1);
        var txtLogin = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtLogin, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Senha:"), 0, 2);
        var txtSenha = FormComponents.CreatePasswordTextBox();
        tbl.Controls.Add(txtSenha, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Tipo:"), 0, 3);
        var cmbTipo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbTipo.DataSource = new List<string> { "Usuario", "Farmaceutico", "Administrador" };
        cmbTipo.SelectedValue = "Usuario";
        tbl.Controls.Add(cmbTipo, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 4);
        var txtEmail = FormComponents.CreateTextBox();
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 5);
        var txtTelefone = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtTelefone, 1, 5);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 6);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var response = await ApiClient.Instance.PostAsync("api/auth/register", new
                {
                    nome = txtNome.Text.Trim(),
                    login = txtLogin.Text.Trim(),
                    password = txtSenha.Text,
                    tipo = cmbTipo.SelectedValue?.ToString() ?? "Usuario"
                });
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var authResult = JsonSerializer.Deserialize<AuthResponse>(json, jsonOptions);
                if (authResult == null || authResult.Id == 0) return;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(null, email, telefone);

                if (contatoId.HasValue)
                {
                    var user = await ApiClient.Instance.GetAsync<Usuario>($"api/usuarios/{authResult.Id}");
                    if (user != null)
                    {
                        await ApiClient.Instance.PutAsync($"api/usuarios/{authResult.Id}", new
                        {
                            id = authResult.Id,
                            nome = user.Nome,
                            login = user.Login,
                            passwordHash = user.PasswordHash,
                            ativo = user.Ativo,
                            tipoUsuario = user.TipoUsuario,
                            contatoId
                        });
                    }
                }

                txtNome.Clear(); txtLogin.Clear(); txtSenha.Clear(); txtEmail.Clear(); txtTelefone.Clear();
                cmbTipo.SelectedValue = "Usuario";
                await LoadData();
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };
        dialog.ShowDialog(this);
    }

    private async Task EditItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um usuário.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        Contato? contato = null;
        if (item.ContatoId.HasValue)
        {
            try { contato = await ApiClient.Instance.GetAsync<Contato>($"api/contatos/{item.ContatoId.Value}"); }
            catch { }
        }

        using var dialog = FormComponents.CreateDialog("Editar Usuário", 400, 320);
        var tbl = FormComponents.CreateDialogLayout(2, 8, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox(item.Nome);
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Login:"), 0, 1);
        var txtLogin = FormComponents.CreateTextBox(item.Login);
        tbl.Controls.Add(txtLogin, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Senha:"), 0, 2);
        var txtSenha = FormComponents.CreatePasswordTextBox();
        tbl.Controls.Add(txtSenha, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Tipo:"), 0, 3);
        var cmbTipo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbTipo.DataSource = new List<string> { "Usuario", "Farmaceutico", "Administrador" };
        cmbTipo.SelectedValue = item.TipoUsuario;
        tbl.Controls.Add(cmbTipo, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 4);
        var chkAtivo = FormComponents.CreateCheckBox("Ativo", item.Ativo);
        tbl.Controls.Add(chkAtivo, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 5);
        var txtEmail = FormComponents.CreateTextBox(contato?.Email ?? "");
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 5);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 6);
        var txtTelefone = FormComponents.CreateTextBox(contato?.Telefone ?? "");
        tbl.Controls.Add(txtTelefone, 1, 6);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 7);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var email = txtEmail.Text.Trim();
                var telefone = txtTelefone.Text.Trim();
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(item.ContatoId, email, telefone);

                var response = await ApiClient.Instance.PutAsync($"api/usuarios/{item.Id}", new
                {
                    id = item.Id,
                    nome = txtNome.Text.Trim(),
                    login = txtLogin.Text.Trim(),
                    passwordHash = item.PasswordHash,
                    ativo = chkAtivo.Checked,
                    tipoUsuario = cmbTipo.SelectedValue?.ToString() ?? item.TipoUsuario,
                    contatoId
                });
                if (response.IsSuccessStatusCode)
                {
                    txtNome.Clear(); txtLogin.Clear(); txtSenha.Clear(); txtEmail.Clear(); txtTelefone.Clear();
                    cmbTipo.SelectedValue = item.TipoUsuario;
                    chkAtivo.Checked = item.Ativo;
                    await LoadData();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
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
        if (item == null) { MessageBox.Show("Selecione um usuário.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir usuário '{item.Nome}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/usuarios/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
