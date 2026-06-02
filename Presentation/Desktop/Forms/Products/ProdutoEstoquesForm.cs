using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Products;

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
        dgv.CellDoubleClick += async (_, _) => await EditItem();
        tbl.Controls.Add(dgv, 0, 1);
        _searchFilter = new SearchFilter(topPanel, dgv);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        statusPanel.Controls.Add(lblCount);
        tbl.Controls.Add(statusPanel, 0, 2);

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

    private async Task AddItem()
    {
        using var dialog = new Form { Text = "Novo Estoque", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(450, 290) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Produto:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbProd.DataSource = _produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(new Label { Text = "Lote:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtLote = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtLote, 1, 1);

        tbl.Controls.Add(new Label { Text = "Quantidade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 999999, Value = 1 };
        tbl.Controls.Add(nudQtd, 1, 2);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(mtxtRegMs, 1, 3);

        tbl.Controls.Add(new Label { Text = "Fabricação:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var dtpFab = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        tbl.Controls.Add(dtpFab, 1, 4);

        tbl.Controls.Add(new Label { Text = "Validade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var dtpVal = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        tbl.Controls.Add(dtpVal, 1, 5);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
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

        using var dialog = new Form { Text = "Editar Estoque", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(450, 290) };
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Produto:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbProd.DataSource = _produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        cmbProd.SelectedValue = item.ProdutoId;
        cmbProd.Enabled = false;
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(new Label { Text = "Lote:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtLote = new TextBox { Text = item.Lote ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), ReadOnly = true };
        tbl.Controls.Add(txtLote, 1, 1);

        tbl.Controls.Add(new Label { Text = "Quantidade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 999999, Value = item.Quantidade };
        tbl.Controls.Add(nudQtd, 1, 2);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        if (!string.IsNullOrEmpty(item.RegistroMS))
        {
            mtxtRegMs.Text = item.RegistroMS;
        }
        tbl.Controls.Add(mtxtRegMs, 1, 3);

        tbl.Controls.Add(new Label { Text = "Fabricação:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var dtpFab = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        if (item.Fabricacao.HasValue) dtpFab.Value = item.Fabricacao.Value;
        tbl.Controls.Add(dtpFab, 1, 4);

        tbl.Controls.Add(new Label { Text = "Validade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var dtpVal = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        if (item.Validade.HasValue) dtpVal.Value = item.Validade.Value;
        tbl.Controls.Add(dtpVal, 1, 5);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
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
