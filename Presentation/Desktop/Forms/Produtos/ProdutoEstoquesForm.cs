using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Produtos;

public class ProdutoEstoquesForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<ProdutoEstoque> _items = [];
    private List<Produto> _produtos = [];

    public ProdutoEstoquesForm()
    {
        Text = "Estoques";
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
        dgv.CellDoubleClick += (_, _) => ViewItem();
        tbl.Controls.Add(dgv, 0, 1);
        _searchFilter = new SearchFilter(topPanel, dgv);

        tbl.Controls.Add(FormComponents.CreateStatusBar(dgv), 0, 2);

        Controls.Add(tbl);

        Load += async (_, _) =>
        {
            _produtos = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];
            await LoadData();
        };
    }

    private async Task LoadData()
    {
        try
        {
            _items = await ApiClient.Instance.GetAsync<List<ProdutoEstoque>>("api/estoques") ?? [];

            dgv.Columns.Clear();
            dgv.Columns.Add("Produto", "Produto");
            dgv.Columns.Add("Lote", "Lote");
            dgv.Columns.Add("Quantidade", "Qtd");
            dgv.Columns.Add("RegistroMS", "Registro MS");
            dgv.Columns.Add("Fabricacao", "Fabricação");
            dgv.Columns.Add("Validade", "Validade");
            dgv.Columns["Produto"]!.FillWeight = 40;
            dgv.Columns["Produto"]!.MinimumWidth = 120;
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Produto?.Descricao ?? $"ID {item.ProdutoId}", item.Lote ?? "", item.Quantidade, item.RegistroMS ?? "", item.Fabricacao?.ToString("dd/MM/yyyy") ?? "", item.Validade?.ToString("dd/MM/yyyy") ?? "");
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private ProdutoEstoque? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _items.Count)
            return _items[dgv.SelectedRows[0].Index];
        return null;
    }

    private void ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um estoque.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = FormComponents.CreateDialog("Visualizar Estoque", 450, 290);
        var tbl = FormComponents.CreateDialogLayout(2, 7, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Produto:"), 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbProd.DataSource = _produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        cmbProd.SelectedValue = item.ProdutoId;
        cmbProd.Enabled = false;
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Lote:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Lote ?? ""), 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Quantidade:"), 0, 2);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Minimum = 1, Maximum = 999999, Value = item.Quantidade, Enabled = false };
        tbl.Controls.Add(nudQtd, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 3);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Enabled = false };
        if (!string.IsNullOrEmpty(item.RegistroMS))
            mtxtRegMs.Text = item.RegistroMS;
        tbl.Controls.Add(mtxtRegMs, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricação:"), 0, 4);
        var dtpFab = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short, Enabled = false };
        if (item.Fabricacao.HasValue) dtpFab.Value = item.Fabricacao.Value;
        tbl.Controls.Add(dtpFab, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Validade:"), 0, 5);
        var dtpVal = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short, Enabled = false };
        if (item.Validade.HasValue) dtpVal.Value = item.Validade.Value;
        tbl.Controls.Add(dtpVal, 1, 5);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        var btnDelete = FormComponents.CreateDeleteButton(80);
        var btnClose = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnClose); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnEdit);
        tbl.Controls.Add(btnPanel, 0, 6);
        dialog.Controls.Add(tbl);

        btnEdit.Click += (_, _) => { dialog.Close(); _ = EditItem(); };
        btnDelete.Click += (_, _) => { dialog.Close(); _ = DeleteItem(); };
        btnClose.Click += (_, _) => dialog.Close();

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        using var dialog = FormComponents.CreateDialog("Novo Estoque", 450, 290);
        var tbl = FormComponents.CreateDialogLayout(2, 7, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Produto:"), 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbProd.DataSource = _produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Lote:"), 0, 1);
        var txtLote = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtLote, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Quantidade:"), 0, 2);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Minimum = 1, Maximum = 999999, Value = 1 };
        tbl.Controls.Add(nudQtd, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 3);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        tbl.Controls.Add(mtxtRegMs, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricação:"), 0, 4);
        var dtpFab = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short };
        tbl.Controls.Add(dtpFab, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Validade:"), 0, 5);
        var dtpVal = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short };
        tbl.Controls.Add(dtpVal, 1, 5);

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
            var prodId = (int)(cmbProd.SelectedValue ?? 0);
            if (prodId == 0) { MessageBox.Show("Selecione um produto.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(txtLote.Text)) { MessageBox.Show("Informe o lote.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                dialog.Enabled = false;

                var regMs = mtxtRegMs.Text.Trim();
                var response = await ApiClient.Instance.PostAsync($"api/produtos/{prodId}/estoques", new
                {
                    produtoId = prodId,
                    lote = txtLote.Text.Trim(),
                    quantidade = (int)nudQtd.Value,
                    registroMS = mtxtRegMs.MaskCompleted ? regMs : null,
                    fabricacao = dtpFab.Value.ToString("yyyy-MM-dd"),
                    validade = dtpVal.Value.ToString("yyyy-MM-dd")
                });
                if (response.IsSuccessStatusCode)
                {
                    txtLote.Clear(); mtxtRegMs.Clear(); nudQtd.Value = 1;
                    cmbProd.ClearSelection();
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
        if (item == null) { MessageBox.Show("Selecione um estoque.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = FormComponents.CreateDialog("Editar Estoque", 450, 290);
        var tbl = FormComponents.CreateDialogLayout(2, 7, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Produto:"), 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbProd.DataSource = _produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        cmbProd.SelectedValue = item.ProdutoId;
        cmbProd.Enabled = false;
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Lote:"), 0, 1);
        var txtLote = FormComponents.CreateReadOnlyTextBox(item.Lote ?? "");
        tbl.Controls.Add(txtLote, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Quantidade:"), 0, 2);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Minimum = 1, Maximum = 999999, Value = item.Quantidade };
        tbl.Controls.Add(nudQtd, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 3);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont };
        if (!string.IsNullOrEmpty(item.RegistroMS))
            mtxtRegMs.Text = item.RegistroMS;
        tbl.Controls.Add(mtxtRegMs, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricação:"), 0, 4);
        var dtpFab = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short };
        if (item.Fabricacao.HasValue) dtpFab.Value = item.Fabricacao.Value;
        tbl.Controls.Add(dtpFab, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Validade:"), 0, 5);
        var dtpVal = new DateTimePicker { Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, Format = DateTimePickerFormat.Short };
        if (item.Validade.HasValue) dtpVal.Value = item.Validade.Value;
        tbl.Controls.Add(dtpVal, 1, 5);

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

                var regMs = mtxtRegMs.Text.Trim();
                var response = await ApiClient.Instance.PutAsync($"api/produtos/{item.ProdutoId}/estoques/{item.Lote}", new
                {
                    produtoId = item.ProdutoId,
                    lote = item.Lote,
                    quantidade = (int)nudQtd.Value,
                    registroMS = mtxtRegMs.MaskCompleted ? regMs : null,
                    fabricacao = dtpFab.Value.ToString("yyyy-MM-dd"),
                    validade = dtpVal.Value.ToString("yyyy-MM-dd")
                });
                if (response.IsSuccessStatusCode)
                {
                    mtxtRegMs.Clear(); nudQtd.Value = 1;
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
        if (item == null) { MessageBox.Show("Selecione um estoque.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (string.IsNullOrEmpty(item.Lote)) { MessageBox.Show("Este registro não possui lote para exclusão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir estoque Lote '{item.Lote}' do Produto {(item.Produto?.Descricao ?? $"ID {item.ProdutoId}")}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/produtos/{item.ProdutoId}/estoques/{item.Lote}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
