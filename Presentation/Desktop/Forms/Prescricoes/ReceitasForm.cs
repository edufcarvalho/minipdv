using minipdv.Application.DTOs;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Prescricoes;

public class ReceitasForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Receita> _items = [];

    public ReceitasForm()
    {
        Text = "Receitas";
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
        Load += async (_, _) => await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            _items = await ApiClient.Instance.GetAsync<List<Receita>>("api/receitas") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("DataReceita", "Data Receita");
            dgv.Columns.Add("DataCadastro", "Data Cadastro");
            dgv.Columns.Add("PrescritorNome", "Prescritor");
            dgv.Columns.Add("PacienteNome", "Paciente");
            dgv.Columns.Add("CompradorNome", "Comprador");
            dgv.Columns.Add("VendaId", "Venda ID");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.DataReceita.ToString("dd/MM/yyyy"), item.DataCadastro.ToString("dd/MM/yyyy"), item.Prescritor?.Nome ?? "", item.Paciente?.Nome ?? "", item.Comprador?.Nome ?? "", item.VendaId?.ToString() ?? "");
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task ViewItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma receita.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        Receita? fullReceita = null;
        try { fullReceita = await ApiClient.Instance.GetAsync<Receita>($"api/receitas/{item.Id}"); }
        catch (Exception ex) { MessageBox.Show($"Erro ao carregar receita: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        if (fullReceita == null) { MessageBox.Show("Receita não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        using var dialog = FormComponents.CreateDialog("Visualizar Receita", 550, 400);
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        var infoTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(5) };
        infoTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        infoTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        infoTbl.Controls.Add(new Label { Text = "Data Receita:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var dtpDataReceita = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Enabled = false, Value = fullReceita.DataReceita };
        infoTbl.Controls.Add(dtpDataReceita, 1, 0);

        infoTbl.Controls.Add(new Label { Text = "Data Cadastro:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var dtpDataCadastro = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Enabled = false, Value = fullReceita.DataCadastro };
        infoTbl.Controls.Add(dtpDataCadastro, 1, 1);

        infoTbl.Controls.Add(new Label { Text = "Prescritor:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtPresc = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Control, Text = fullReceita.Prescritor?.Nome ?? "" };
        infoTbl.Controls.Add(txtPresc, 1, 2);

        infoTbl.Controls.Add(new Label { Text = "Paciente:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var txtPac = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Control, Text = fullReceita.Paciente?.Nome ?? "" };
        infoTbl.Controls.Add(txtPac, 1, 3);

        infoTbl.Controls.Add(new Label { Text = "Comprador:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var txtComp = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, BackColor = SystemColors.Control, Text = fullReceita.Comprador?.Nome ?? "" };
        infoTbl.Controls.Add(txtComp, 1, 4);

        tbl.Controls.Add(infoTbl, 0, 0);

        var dgvProd = FormComponents.CreateDataGridView();
        dgvProd.Columns.Add("Produto", "Produto");
        dgvProd.Columns.Add("Lote", "Lote");
        dgvProd.Columns.Add("Quantidade", "Qtd");
        dgvProd.Columns["Produto"]!.FillWeight = 40;
        dgvProd.Columns["Produto"]!.MinimumWidth = 120;
        foreach (var rpe in fullReceita.ReceitaProdutoEstoques)
            dgvProd.Rows.Add(rpe.ProdutoEstoque?.Produto?.Descricao ?? $"ID {rpe.ProdutoId}", rpe.Lote, rpe.Quantidade);
        tbl.Controls.Add(dgvProd, 0, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        var btnDelete = new Button { Text = "Excluir", Width = 90, Height = 35, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 10, 0) };
        var btnClose = new Button { Text = "Fechar", Width = 90, Height = 35, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnClose); btnPanel.Controls.Add(btnDelete);
        tbl.Controls.Add(btnPanel, 0, 2);

        dialog.Controls.Add(tbl);

        btnDelete.Click += (_, _) => { dialog.Close(); _ = DeleteItem(); };
        btnClose.Click += (_, _) => dialog.Close();

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        var prescritores = await ApiClient.Instance.GetAsync<List<Prescritor>>("api/prescritores") ?? [];
        var clientes = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes") ?? [];
        var produtos = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];

        var receitaProdutos = new List<ReceitaProdutoItem>();
        var font = new Font("Segoe UI", 10);

        using var dialog = new Form { Text = "Nova Receita", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(650, 570) };

        var outerLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(10) };
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 210));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        var topTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(5) };
        topTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        topTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        topTbl.Controls.Add(new Label { Text = "Data Receita:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 0);
        var dtpDataReceita = new DateTimePicker { Dock = DockStyle.Fill, Font = font, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        topTbl.Controls.Add(dtpDataReceita, 1, 0);

        topTbl.Controls.Add(new Label { Text = "Data Cadastro:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 1);
        var dtpDataCadastro = new DateTimePicker { Dock = DockStyle.Fill, Font = font, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        topTbl.Controls.Add(dtpDataCadastro, 1, 1);

        topTbl.Controls.Add(new Label { Text = "Prescritor:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 2);
        var cmbPresc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbPresc.DataSource = prescritores;
        cmbPresc.DisplayMember = "Nome";
        cmbPresc.ValueMember = "Id";
        topTbl.Controls.Add(cmbPresc, 1, 2);

        topTbl.Controls.Add(new Label { Text = "Paciente:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 3);
        var cmbPac = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbPac.DataSource = clientes;
        cmbPac.DisplayMember = "Nome";
        cmbPac.ValueMember = "Id";
        topTbl.Controls.Add(cmbPac, 1, 3);

        topTbl.Controls.Add(new Label { Text = "Comprador:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 4);
        var cmbComp = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbComp.DataSource = clientes;
        cmbComp.DisplayMember = "Nome";
        cmbComp.ValueMember = "Id";
        topTbl.Controls.Add(cmbComp, 1, 4);

        outerLayout.Controls.Add(topTbl, 0, 0);

        var addTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 2, Padding = new Padding(5) };
        addTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        addTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        addTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55));
        addTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        addTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        addTbl.Controls.Add(new Label { Text = "Produto:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 0);
        var cmbProd = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbProd.DataSource = produtos;
        cmbProd.DisplayMember = "Descricao";
        cmbProd.ValueMember = "Id";
        addTbl.Controls.Add(cmbProd, 1, 0);

        addTbl.Controls.Add(new Label { Text = "Lote:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 2, 0);
        var cmbLote = new ComboBox { Dock = DockStyle.Fill, Font = font, DropDownStyle = ComboBoxStyle.DropDownList };
        addTbl.Controls.Add(cmbLote, 3, 0);

        var btnAddProd = new Button { Text = "Adicionar", Width = 90, Height = 30, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        addTbl.Controls.Add(btnAddProd, 4, 0);

        addTbl.Controls.Add(new Label { Text = "Qtd:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 1);
        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = font, Minimum = 1, Maximum = 9999, Value = 1 };
        addTbl.Controls.Add(nudQtd, 1, 1);

        var btnRemoveProd = new Button { Text = "Remover selecionado", Width = 140, Height = 30, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        addTbl.Controls.Add(btnRemoveProd, 3, 1);

        outerLayout.Controls.Add(addTbl, 0, 1);

        var prodLabel = new Label { Text = "Produtos na Receita:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Fill };
        outerLayout.Controls.Add(prodLabel, 0, 2);

        var dgvProdutos = FormComponents.CreateDataGridView();
        dgvProdutos.Columns.Add("Produto", "Produto");
        dgvProdutos.Columns.Add("Lote", "Lote");
        dgvProdutos.Columns.Add("Quantidade", "Qtd");
        dgvProdutos.Columns["Produto"]!.FillWeight = 40;
        dgvProdutos.Columns["Produto"]!.MinimumWidth = 120;
        outerLayout.Controls.Add(dgvProdutos, 0, 2);

        var btnPanel2 = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnOk = new Button { Text = "Salvar", Width = 90, Height = 35, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = font };
        var btnCancel = new Button { Text = "Cancelar", Width = 90, Height = 35, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0), Font = font };
        btnPanel2.Controls.Add(btnOk);
        btnPanel2.Controls.Add(btnCancel);
        outerLayout.Controls.Add(btnPanel2, 0, 3);

        dialog.Controls.Add(outerLayout);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        cmbProd.SelectedValueChanged += async (_, _) =>
        {
            cmbLote.Items.Clear();
            if (cmbProd.SelectedValue is int prodId)
            {
                try
                {
                    var estoques = await ApiClient.Instance.GetAsync<List<ProdutoEstoque>>($"api/produtos/{prodId}/estoques") ?? [];
                    foreach (var e in estoques.Where(e => !string.IsNullOrEmpty(e.Lote)))
                        cmbLote.Items.Add(e.Lote!);
                    if (cmbLote.Items.Count > 0)
                        cmbLote.SelectedIndex = 0;
                }
                catch { }
            }
        };

        void RefreshProdGrid()
        {
            dgvProdutos.Rows.Clear();
            foreach (var ri in receitaProdutos)
            {
                var prodDesc = produtos.FirstOrDefault(p => p.Id == ri.ProdutoId)?.Descricao ?? $"ID {ri.ProdutoId}";
                dgvProdutos.Rows.Add(prodDesc, ri.Lote, ri.Quantidade);
            }
        }

        btnAddProd.Click += (_, _) =>
        {
            if (cmbProd.SelectedValue == null) { MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbLote.SelectedItem == null) { MessageBox.Show("Selecione um lote.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var prodId = (int)cmbProd.SelectedValue;
            var lote = cmbLote.SelectedItem.ToString()!;

            if (receitaProdutos.Any(p => p.ProdutoId == prodId && p.Lote == lote))
            {
                MessageBox.Show("Este produto/lote já foi adicionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            receitaProdutos.Add(new ReceitaProdutoItem(prodId, lote, (int)nudQtd.Value));
            RefreshProdGrid();
            nudQtd.Value = 1;
        };

        btnRemoveProd.Click += (_, _) =>
        {
            if (dgvProdutos.SelectedRows.Count == 0) return;
            var rowIdx = dgvProdutos.SelectedRows[0].Index;
            if (rowIdx >= 0 && rowIdx < receitaProdutos.Count)
            {
                receitaProdutos.RemoveAt(rowIdx);
                RefreshProdGrid();
            }
        };

        btnOk.Click += async (_, _) =>
        {
            if (cmbPresc.SelectedValue == null || cmbPac.SelectedValue == null || cmbComp.SelectedValue == null)
            {
                MessageBox.Show("Selecione prescritor, paciente e comprador.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PostAsync("api/receitas", new
                {
                    prescritorId = (int)cmbPresc.SelectedValue,
                    pacienteId = (int)cmbPac.SelectedValue,
                    compradorId = (int)cmbComp.SelectedValue,
                    dataReceita = dtpDataReceita.Value,
                    dataCadastro = dtpDataCadastro.Value,
                    produtos = receitaProdutos.Count > 0 ? receitaProdutos : null
                });
                if (response.IsSuccessStatusCode)
                {
                    receitaProdutos.Clear();
                    dgvProdutos.Rows.Clear();
                    dtpDataReceita.Value = DateTime.Today;
                    dtpDataCadastro.Value = DateTime.Today;
                    cmbPresc.ClearSelection(); cmbPac.ClearSelection(); cmbComp.ClearSelection();
                    cmbProd.ClearSelection(); cmbLote.Items.Clear(); cmbLote.SelectedIndex = -1; nudQtd.Value = 1;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                    await LoadData();
                }
                else
                    MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { dialog.Enabled = true; }
        };

        dialog.ShowDialog(this);
    }

    private async Task DeleteItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma receita.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        if (MessageBox.Show($"Excluir receita #{item.Id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/receitas/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
