using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Products;

public class ProdutoEstoquesForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchableComboBox cmbProduto;
    private readonly TextBox txtSearch;
    private List<ProdutoEstoque> _items = [];
    private List<Produto> _produtos = [];

    public ProdutoEstoquesForm()
    {
        Text = "Estoques";
        Dock = DockStyle.Fill;

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var btnRefresh = new Button { Text = "Carregar", Width = 90, Height = 32, Cursor = Cursors.Hand };
        btnRefresh.Click += async (_, _) => await LoadData();
        topPanel.Controls.Add(btnRefresh);
        topPanel.Controls.Add(new Label { Width = 10 });
        var btnAdd = new Button { Text = "Adicionar", Width = 90, Height = 32, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnAdd.Click += async (_, _) => await AddItem();
        topPanel.Controls.Add(btnAdd);
        var btnDelete = new Button { Text = "Excluir", Width = 90, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnDelete.Click += async (_, _) => await DeleteItem();
        topPanel.Controls.Add(btnDelete);
        tbl.Controls.Add(topPanel, 0, 0);

        var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        searchPanel.Controls.Add(new Label { Text = "Produto:", TextAlign = ContentAlignment.MiddleLeft, Width = 65, Height = 32 });
        cmbProduto = new SearchableComboBox { Width = 250, Height = 32, PlaceholderText = "Selecione um produto..." };
        searchPanel.Controls.Add(cmbProduto);
        searchPanel.Controls.Add(new Label { Text = " Lote:", TextAlign = ContentAlignment.MiddleLeft, Width = 45, Height = 32 });
        txtSearch = new TextBox { Width = 150, Height = 32, Font = new Font("Segoe UI", 10) };
        searchPanel.Controls.Add(txtSearch);
        var btnSearch = new Button { Text = "Buscar", Width = 70, Height = 32, Cursor = Cursors.Hand };
        btnSearch.Click += async (_, _) => await LoadData();
        searchPanel.Controls.Add(btnSearch);
        tbl.Controls.Add(searchPanel, 0, 1);

        dgv = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(dgv, 0, 2);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        statusPanel.Controls.Add(lblCount);
        tbl.Controls.Add(statusPanel, 0, 3);

        Controls.Add(tbl);

        Load += async (_, _) =>
        {
            _produtos = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];
            cmbProduto.DataSource = _produtos;
            cmbProduto.DisplayMember = "Descricao";
            cmbProduto.ValueMember = "Id";
        };
    }

    private async Task LoadData()
    {
        if (cmbProduto.SelectedValue == null)
        {
            MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var prodId = (int)cmbProduto.SelectedValue;

        try
        {
            var lote = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(lote))
            {
                try
                {
                    var item = await ApiClient.Instance.GetAsync<ProdutoEstoque>($"api/produtos/{prodId}/estoques/{lote}");
                    _items = item != null ? [item] : [];
                }
                catch { _items = []; }
            }
            else
            {
                _items = await ApiClient.Instance.GetAsync<List<ProdutoEstoque>>($"api/produtos/{prodId}/estoques") ?? [];
            }

            dgv.Columns.Clear();
            dgv.Columns.Add("ProdutoId", "Produto ID");
            dgv.Columns.Add("Lote", "Lote");
            dgv.Columns.Add("Quantidade", "Qtd");
            dgv.Columns.Add("RegistroMS", "Registro MS");
            dgv.Columns.Add("Fabricacao", "Fabricação");
            dgv.Columns.Add("Validade", "Validade");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.ProdutoId, item.Lote ?? "", item.Quantidade, item.RegistroMS ?? "", item.Fabricacao?.ToString("dd/MM/yyyy") ?? "", item.Validade?.ToString("dd/MM/yyyy") ?? "");
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
        if (cmbProduto.SelectedValue != null)
            cmbProd.SelectedValue = cmbProduto.SelectedValue;
        tbl.Controls.Add(cmbProd, 1, 0);

        tbl.Controls.Add(new Label { Text = "Lote:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtLote = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtLote, 1, 1);

        tbl.Controls.Add(new Label { Text = "Quantidade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtQtd = new TextBox { Text = "1", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtQtd, 1, 2);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtRegMs = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtRegMs, 1, 3);

        tbl.Controls.Add(new Label { Text = "Fabricação:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var txtFab = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtFab, 1, 4);

        tbl.Controls.Add(new Label { Text = "Validade:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var txtVal = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtVal, 1, 5);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 6);
        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk; dialog.CancelButton = btnCancel;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var prodId = (int)(cmbProd.SelectedValue ?? 0);
            var request = new
            {
                produtoId = prodId,
                lote = txtLote.Text.Trim(),
                quantidade = int.Parse(txtQtd.Text.Trim()),
                registroMS = string.IsNullOrEmpty(txtRegMs.Text.Trim()) ? null : txtRegMs.Text.Trim(),
                fabricacao = string.IsNullOrEmpty(txtFab.Text.Trim()) ? null : txtFab.Text.Trim(),
                validade = string.IsNullOrEmpty(txtVal.Text.Trim()) ? null : txtVal.Text.Trim()
            };
            var response = await ApiClient.Instance.PostAsync($"api/produtos/{prodId}/estoques", request);
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await response.Content.ReadAsStringAsync()}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task DeleteItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um estoque.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (string.IsNullOrEmpty(item.Lote)) { MessageBox.Show("Este registro não possui lote para exclusão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show($"Excluir estoque Lote '{item.Lote}' do Produto {item.ProdutoId}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/produtos/{item.ProdutoId}/estoques/{item.Lote}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await response.Content.ReadAsStringAsync()}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
