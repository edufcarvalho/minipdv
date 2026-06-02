using minipdv.Domain.Entities;
using minipdv.Domain.Enums;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Prescricoes;

public class PrescritoresForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Prescritor> _items = [];

    public PrescritoresForm()
    {
        Text = "Prescritores";
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

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um prescritor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = FormComponents.CreateDialog("Visualizar Prescritor", 400, 200);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Nome), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Número:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Numero), 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Conselho:"), 0, 2);
        var cmbConselho = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Enabled = false };
        cmbConselho.DataSource = Enum.GetValues<Conselho>().Cast<object>().ToList();
        cmbConselho.SelectedValue = item.Conselho;
        tbl.Controls.Add(cmbConselho, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("UF:"), 0, 3);
        var cmbUf = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Enabled = false };
        cmbUf.DataSource = Enum.GetValues<UF>().Cast<object>().ToList();
        cmbUf.SelectedValue = item.Uf;
        tbl.Controls.Add(cmbUf, 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        var btnDelete = FormComponents.CreateDeleteButton(80);
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };
        var btnFechar = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnFechar);
        tbl.Controls.Add(btnPanel, 0, 4);
        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnFechar;
        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Prescritor", 400, 200);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Número:"), 0, 1);
        var txtNumero = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtNumero, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Conselho:"), 0, 2);
        var cmbConselho = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbConselho.DataSource = Enum.GetValues<Conselho>().Cast<object>().ToList();
        tbl.Controls.Add(cmbConselho, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("UF:"), 0, 3);
        var cmbUf = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbUf.DataSource = Enum.GetValues<UF>().Cast<object>().ToList();
        tbl.Controls.Add(cmbUf, 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
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

        using var dialog = FormComponents.CreateDialog("Editar Prescritor", 400, 200);
        var tbl = FormComponents.CreateDialogLayout(2, 5, 80);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Nome:"), 0, 0);
        var txtNome = FormComponents.CreateTextBox(item.Nome);
        tbl.Controls.Add(txtNome, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Número:"), 0, 1);
        var txtNumero = FormComponents.CreateTextBox(item.Numero);
        tbl.Controls.Add(txtNumero, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Conselho:"), 0, 2);
        var cmbConselho = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbConselho.DataSource = Enum.GetValues<Conselho>().Cast<object>().ToList();
        cmbConselho.SelectedValue = item.Conselho;
        tbl.Controls.Add(cmbConselho, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("UF:"), 0, 3);
        var cmbUf = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbUf.DataSource = Enum.GetValues<UF>().Cast<object>().ToList();
        cmbUf.SelectedValue = item.Uf;
        tbl.Controls.Add(cmbUf, 1, 3);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
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
