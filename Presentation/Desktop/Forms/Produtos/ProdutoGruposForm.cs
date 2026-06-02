using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Produtos;

public class ProdutoGruposForm : Form
{
    private readonly DataGridView dgv;
    private List<ProdutoGrupo> _items = [];

    public ProdutoGruposForm()
    {
        Text = "Grupos de Produtos";
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
            _items = await ApiClient.Instance.GetAsync<List<ProdutoGrupo>>("api/produtogrupos") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Nome", "Nome");
            dgv.Columns.Add("Ativo", "Ativo");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.Nome, item.Ativo ? "Sim" : "Não");
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private ProdutoGrupo? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _items.Count)
            return _items[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um grupo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = FormComponents.CreateDialog("Visualizar Grupo", 350, 160);
        var tbl = FormComponents.CreateDialogLayout(2, 3, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Nome), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateCheckBox("Ativo", item.Ativo, enabled: false), 1, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        var btnDelete = FormComponents.CreateDeleteButton(80);
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };
        var btnFechar = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnFechar);
        tbl.Controls.Add(btnPanel, 0, 2);
        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnFechar;
        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Grupo", 350, 160);
        var tbl = FormComponents.CreateDialogLayout(2, 3, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txt = FormComponents.CreateTextBox();
        tbl.Controls.Add(txt, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 1);
        var chkAtivo = FormComponents.CreateCheckBox("Ativo", true);
        tbl.Controls.Add(chkAtivo, 1, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PostAsync("api/produtogrupos", new { nome = txt.Text.Trim(), ativo = chkAtivo.Checked });
                if (response.IsSuccessStatusCode)
                {
                    txt.Clear(); chkAtivo.Checked = true;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                    await LoadData();
                }
                else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 2);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        dialog.ShowDialog(this);
    }

    private async Task EditItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um grupo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = FormComponents.CreateDialog("Editar Grupo", 350, 160);
        var tbl = FormComponents.CreateDialogLayout(2, 3, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txt = FormComponents.CreateTextBox(item.Nome);
        tbl.Controls.Add(txt, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 1);
        var chkAtivo = FormComponents.CreateCheckBox("Ativo", item.Ativo);
        tbl.Controls.Add(chkAtivo, 1, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PutAsync($"api/produtogrupos/{item.Id}", new { id = item.Id, nome = txt.Text.Trim(), ativo = chkAtivo.Checked });
                if (response.IsSuccessStatusCode)
                {
                    txt.Clear(); chkAtivo.Checked = item.Ativo;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                    await LoadData();
                }
                else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 2);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        dialog.ShowDialog(this);
    }

    private async Task DeleteItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um grupo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir grupo '{item.Nome}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/produtogrupos/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
