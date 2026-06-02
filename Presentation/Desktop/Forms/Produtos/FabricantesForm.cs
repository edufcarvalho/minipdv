using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Produtos;

public class FabricantesForm : Form
{
    private readonly DataGridView dgv;
    private List<Fabricante> _items = [];

    public FabricantesForm()
    {
        Text = "Fabricantes";
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
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = FormComponents.CreateDataGridView();
        dgv.CellDoubleClick += async (_, _) => await ViewItem();
        tbl.Controls.Add(dgv, 0, 1);

        tbl.Controls.Add(FormComponents.CreateStatusBar(dgv), 0, 2);

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

        using var dialog = FormComponents.CreateDialog("Visualizar Fabricante", 400, 280);
        var tbl = FormComponents.CreateDialogLayout(2, 6, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome Fantasia:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.NomeFantasia), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Razão Social:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.RazaoSocial), 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CNPJ:"), 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cnpj, Enabled = false, ReadOnly = true, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 3);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Email ?? ""), 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 4);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(contato?.Telefone ?? ""), 1, 4);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        var btnDelete = FormComponents.CreateDeleteButton(80);
        var btnClose = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 5);
        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Fabricante", 400, 280);
        var tbl = FormComponents.CreateDialogLayout(2, 6, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome Fantasia:"), 0, 0);
        var txtNF = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtNF, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Razão Social:"), 0, 1);
        var txtRS = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtRS, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CNPJ:"), 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 3);
        var txtEmail = FormComponents.CreateTextBox();
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 4);
        var txtTelefone = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtTelefone, 1, 4);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
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
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(null, email, telefone);

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

        using var dialog = FormComponents.CreateDialog("Editar Fabricante", 400, 280);
        var tbl = FormComponents.CreateDialogLayout(2, 6, 100);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome Fantasia:"), 0, 0);
        var txtNF = FormComponents.CreateTextBox(item.NomeFantasia);
        tbl.Controls.Add(txtNF, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Razão Social:"), 0, 1);
        var txtRS = FormComponents.CreateTextBox(item.RazaoSocial);
        tbl.Controls.Add(txtRS, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("CNPJ:"), 0, 2);
        var mtxtCNPJ = new MaskedTextBox { Mask = "00.000.000/0000-00", Culture = CultureInfo.InvariantCulture, Text = item.Cnpj, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtCNPJ, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Email:"), 0, 3);
        var txtEmail = FormComponents.CreateTextBox(contato?.Email ?? "");
        FormComponents.BlockSpaceChar(txtEmail);
        tbl.Controls.Add(txtEmail, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Telefone:"), 0, 4);
        var txtTelefone = FormComponents.CreateTextBox(contato?.Telefone ?? "");
        tbl.Controls.Add(txtTelefone, 1, 4);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
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
                var contatoId = await ContatoHelper.CreateOrUpdateAsync(item.ContatoId, email, telefone);

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
