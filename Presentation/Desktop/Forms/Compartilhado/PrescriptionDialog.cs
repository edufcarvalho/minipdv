using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Compartilhado;

public class PrescriptionDialog : Form
{
    private readonly DataGridView dgvPendentes;
    private readonly DataGridView dgvReceitas;
    private readonly Button btnFinalizar;
    private readonly Label lblPendentes;
    private readonly List<Cliente> _clientes;
    private readonly List<Prescritor> _prescritores;
    private readonly List<Produto> _produtos;
    private readonly List<Receita> _availableReceitas;
    private readonly List<ControlledItem> _pendingItems;
    private readonly List<ReceitaInfo> _createdReceitas = [];

    public class ControlledItem
    {
        public int ProdutoId { get; set; }
        public string Descricao { get; set; } = "";
        public string Lote { get; set; } = "";
        public int Quantidade { get; set; }
    }
    public record ReceitaInfo(int Id, List<ControlledItem> Items);

    public List<ReceitaInfo> CreatedReceitas => _createdReceitas;

    public PrescriptionDialog(
        List<ControlledItem> controlledItems,
        List<Cliente> clientes,
        List<Prescritor> prescritores,
        List<Produto> produtos,
        List<Receita> availableReceitas)
    {
        _pendingItems = controlledItems.Select(ci => new ControlledItem
        {
            ProdutoId = ci.ProdutoId,
            Descricao = ci.Descricao,
            Lote = ci.Lote,
            Quantidade = ci.Quantidade
        }).ToList();
        _clientes = clientes;
        _prescritores = prescritores;
        _produtos = produtos;
        _availableReceitas = availableReceitas;

        Text = "Receitas para Produtos Controlados";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(750, 600);

        var mainTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(10) };
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

        lblPendentes = new Label { Text = "Produtos controlados pendentes de receita:", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        mainTbl.Controls.Add(lblPendentes, 0, 0);

        dgvPendentes = FormComponents.CreateDataGridView();
        dgvPendentes.Columns.Add("Produto", "Produto");
        dgvPendentes.Columns.Add("Lote", "Lote");
        dgvPendentes.Columns.Add("Pendente", "Qtd Pendente");
        dgvPendentes.Columns["Produto"]!.FillWeight = 40;
        dgvPendentes.Columns["Produto"]!.MinimumWidth = 120;
        mainTbl.Controls.Add(dgvPendentes, 0, 1);

        var lblReceitas = new Label { Text = "Receitas criadas:", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        mainTbl.Controls.Add(lblReceitas, 0, 2);

        var bottomTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        bottomTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bottomTbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

        dgvReceitas = FormComponents.CreateDataGridView();
        dgvReceitas.Columns.Add("ReceitaId", "Receita ID");
        dgvReceitas.Columns.Add("Produtos", "Produtos Cobertos");
        dgvReceitas.Columns["ReceitaId"]!.FillWeight = 15;
        dgvReceitas.Columns["Produtos"]!.FillWeight = 40;
        dgvReceitas.Columns["Produtos"]!.MinimumWidth = 150;
        bottomTbl.Controls.Add(dgvReceitas, 0, 0);

        var btnPanel = FormComponents.CreateDialogButtonPanel();

        btnFinalizar = new Button { Text = "Finalizar Venda", Width = 130, Height = 32, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(0), Enabled = false };
        btnFinalizar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };

        var btnAddReceita = new Button { Text = "Nova Receita", Width = 120, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnAddReceita.Click += async (_, _) => await AddReceita();

        var btnVincular = new Button { Text = "Vincular Existente", Width = 150, Height = 32, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnVincular.Click += (_, _) => VincularReceitaExistente();

        btnPanel.Controls.Add(btnFinalizar); btnPanel.Controls.Add(btnVincular); btnPanel.Controls.Add(btnAddReceita);
        bottomTbl.Controls.Add(btnPanel, 0, 1);
        mainTbl.Controls.Add(bottomTbl, 0, 3);

        Controls.Add(mainTbl);

        Load += (_, _) => RefreshPendentes();
    }

    private void RefreshPendentes()
    {
        dgvPendentes.Rows.Clear();
        var remaining = _pendingItems.Where(p => p.Quantidade > 0).ToList();
        foreach (var item in remaining)
            dgvPendentes.Rows.Add(item.Descricao, item.Lote, item.Quantidade);

        lblPendentes.Text = $"Produtos controlados pendentes de receita: {remaining.Sum(p => p.Quantidade)} itens";
        btnFinalizar.Enabled = remaining.Count == 0;
    }

    private void RefreshReceitas()
    {
        dgvReceitas.Rows.Clear();
        foreach (var rec in _createdReceitas)
        {
            var desc = string.Join(", ", rec.Items.Select(i =>
            {
                var prod = _produtos.FirstOrDefault(p => p.Id == i.ProdutoId);
                return $"{prod?.Descricao ?? $"ID {i.ProdutoId}"} (Lote: {i.Lote}, Qtd: {i.Quantidade})";
            }));
            dgvReceitas.Rows.Add(rec.Id, desc);
        }
    }

    private async Task AddReceita()
    {
        var availableItems = _pendingItems.Where(p => p.Quantidade > 0).ToList();
        if (availableItems.Count == 0) return;

        var font = new Font("Segoe UI", 10);

        using var dialog = new Form
        {
            Text = "Nova Receita para Controlados",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(650, 520)
        };

        var outerLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(10) };
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

        var topTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(5) };
        topTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        topTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        topTbl.Controls.Add(new Label { Text = "Prescritor:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 0);
        var cmbPresc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbPresc.DataSource = _prescritores;
        cmbPresc.DisplayMember = "Nome";
        cmbPresc.ValueMember = "Id";
        topTbl.Controls.Add(cmbPresc, 1, 0);

        topTbl.Controls.Add(new Label { Text = "Paciente:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 1);
        var cmbPac = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbPac.DataSource = _clientes;
        cmbPac.DisplayMember = "Nome";
        cmbPac.ValueMember = "Id";
        topTbl.Controls.Add(cmbPac, 1, 1);

        topTbl.Controls.Add(new Label { Text = "Comprador:", TextAlign = ContentAlignment.MiddleLeft, Font = font }, 0, 2);
        var cmbComp = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione...", Font = font };
        cmbComp.DataSource = _clientes;
        cmbComp.DisplayMember = "Nome";
        cmbComp.ValueMember = "Id";
        topTbl.Controls.Add(cmbComp, 1, 2);

        outerLayout.Controls.Add(topTbl, 0, 0);

        var uniqueProducts = availableItems.GroupBy(i => i.ProdutoId).Select(g => g.First()).ToList();

        var cmbProd = new ComboBox { Dock = DockStyle.Fill, Font = font, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbProd.Items.Add("-- Selecione o produto --");
        foreach (var item in uniqueProducts) cmbProd.Items.Add(item.Descricao);
        cmbProd.SelectedIndex = 0;
        outerLayout.Controls.Add(cmbProd, 0, 1);

        var inputTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Padding = new Padding(5) };
        inputTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        inputTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        inputTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
        inputTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

        var cmbLote = new ComboBox { Dock = DockStyle.Fill, Font = font, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbLote.Items.Add("-- Selecione o lote --");
        cmbLote.SelectedIndex = 0;
        inputTbl.Controls.Add(cmbLote, 0, 0);

        var nudQtd = new NumericUpDown { Dock = DockStyle.Fill, Font = font, Minimum = 1, Maximum = 9999, Value = 1 };
        inputTbl.Controls.Add(nudQtd, 1, 0);

        var btnAddProd = new Button { Text = "Adicionar", Height = 32, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = font };
        inputTbl.Controls.Add(btnAddProd, 2, 0);

        var btnRemoveProd = new Button { Text = "Remover", Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = font };
        inputTbl.Controls.Add(btnRemoveProd, 3, 0);

        outerLayout.Controls.Add(inputTbl, 0, 2);

        var dgvProdutos = FormComponents.CreateDataGridView();
        dgvProdutos.Columns.Add("Produto", "Produto");
        dgvProdutos.Columns.Add("Lote", "Lote");
        dgvProdutos.Columns.Add("Quantidade", "Qtd");
        dgvProdutos.Columns["Produto"]!.FillWeight = 40;
        dgvProdutos.Columns["Produto"]!.MinimumWidth = 120;
        outerLayout.Controls.Add(dgvProdutos, 0, 3);

        var receitaItems = new List<(int ProdutoId, string Lote, int Quantidade, string Descricao)>();

        cmbProd.SelectedIndexChanged += (_, _) =>
        {
            cmbLote.Items.Clear();
            cmbLote.Items.Add("-- Selecione o lote --");
            cmbLote.SelectedIndex = 0;
            cmbLote.Tag = null;
            nudQtd.Maximum = 9999;
            nudQtd.Value = 1;

            if (cmbProd.SelectedIndex <= 0) return;

            var selectedIdx = cmbProd.SelectedIndex - 1;
            if (selectedIdx >= uniqueProducts.Count) return;

            var selectedProduct = uniqueProducts[selectedIdx];
            var availableNow = _pendingItems.Where(p => p.Quantidade > 0).ToList();
            var lotsForProduct = availableNow.Where(p => p.ProdutoId == selectedProduct.ProdutoId).ToList();

            foreach (var lot in lotsForProduct)
                cmbLote.Items.Add($"{lot.Lote} (Pendente: {lot.Quantidade})");
            cmbLote.Tag = lotsForProduct;
        };

        void RefreshLoteCombo()
        {
            if (cmbProd.SelectedIndex <= 0) return;

            var selectedIdx = cmbProd.SelectedIndex - 1;
            if (selectedIdx >= uniqueProducts.Count) return;

            var selectedProduct = uniqueProducts[selectedIdx];
            var availableNow = _pendingItems.Where(p => p.Quantidade > 0).ToList();
            var lotsForProduct = availableNow.Where(p => p.ProdutoId == selectedProduct.ProdutoId).ToList();

            cmbLote.Items.Clear();
            cmbLote.Items.Add("-- Selecione o lote --");
            foreach (var lot in lotsForProduct)
                cmbLote.Items.Add($"{lot.Lote} (Pendente: {lot.Quantidade})");
            cmbLote.Tag = lotsForProduct;
            cmbLote.SelectedIndex = 0;
            nudQtd.Value = 1;
        }

        btnAddProd.Click += (_, _) =>
        {
            if (cmbLote.SelectedIndex <= 0 || cmbLote.Tag is not List<ControlledItem> lotItems) return;
            var lotIdx = cmbLote.SelectedIndex - 1;
            if (lotIdx < 0 || lotIdx >= lotItems.Count) return;

            var selectedLot = lotItems[lotIdx];
            var qtd = (int)nudQtd.Value;
            if (qtd > selectedLot.Quantidade) { MessageBox.Show($"Quantidade máxima pendente: {selectedLot.Quantidade}", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var pending = _pendingItems.First(p => p.ProdutoId == selectedLot.ProdutoId && p.Lote == selectedLot.Lote);
            receitaItems.Add((pending.ProdutoId, pending.Lote, qtd, pending.Descricao));
            dgvProdutos.Rows.Add(pending.Descricao, pending.Lote, qtd);
            pending.Quantidade -= qtd;
            nudQtd.Value = 1;
            RefreshPendentes();
            RefreshLoteCombo();
        };

        btnRemoveProd.Click += (_, _) =>
        {
            if (dgvProdutos.SelectedRows.Count == 0) return;
            var idx = dgvProdutos.SelectedRows[0].Index;
            if (idx < 0 || idx >= receitaItems.Count) return;
            var removed = receitaItems[idx];
            var pending = _pendingItems.First(p => p.ProdutoId == removed.ProdutoId && p.Lote == removed.Lote);
            pending.Quantidade += removed.Quantidade;
            receitaItems.RemoveAt(idx);
            dgvProdutos.Rows.RemoveAt(idx);
            RefreshPendentes();
            RefreshLoteCombo();
        };

        var btnPanel2 = FormComponents.CreateDialogButtonPanel();
        var btnOk = new Button { Text = "Salvar Receita", Width = 120, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = font };
        var btnCancel = new Button { Text = "Cancelar", Width = 90, Height = 32, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel, Font = font };
        btnPanel2.Controls.Add(btnOk); btnPanel2.Controls.Add(btnCancel);
        outerLayout.Controls.Add(btnPanel2, 0, 4);

        dialog.Controls.Add(outerLayout);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        btnOk.Click += async (_, _) =>
        {
            if (cmbPresc.SelectedValue == null || cmbPac.SelectedValue == null || cmbComp.SelectedValue == null) { MessageBox.Show("Selecione prescritor, paciente e comprador.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (receitaItems.Count == 0) { MessageBox.Show("Adicione pelo menos um produto à receita.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                dialog.Enabled = false;

                var response = await ApiClient.Instance.PostAsync("api/receitas", new
                {
                    prescritorId = (int)cmbPresc.SelectedValue,
                    pacienteId = (int)cmbPac.SelectedValue,
                    compradorId = (int)cmbComp.SelectedValue,
                    dataReceita = DateTime.Now,
                    dataCadastro = DateTime.Now,
                    produtos = receitaItems.Select(r => new { produtoId = r.ProdutoId, lote = r.Lote, quantidade = r.Quantidade }).ToList()
                });

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var createdReceita = System.Text.Json.JsonSerializer.Deserialize<Receita>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (createdReceita != null)
                    {
                        _createdReceitas.Add(new ReceitaInfo(createdReceita.Id, receitaItems.Select(r => new ControlledItem { ProdutoId = r.ProdutoId, Descricao = r.Descricao, Lote = r.Lote, Quantidade = r.Quantidade }).ToList()));

                        foreach (var ri in receitaItems)
                        {
                            var pending = _pendingItems.First(p => p.ProdutoId == ri.ProdutoId && p.Lote == ri.Lote);
                            pending.Quantidade -= ri.Quantidade;
                        }
                    }

                    RefreshReceitas();
                    RefreshPendentes();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    var err = await ErrorHelper.ExtractAsync(response);
                    MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    foreach (var ri in receitaItems)
                    {
                        var pending = _pendingItems.First(p => p.ProdutoId == ri.ProdutoId && p.Lote == ri.Lote);
                        pending.Quantidade += ri.Quantidade;
                    }
                    RefreshPendentes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                foreach (var ri in receitaItems)
                {
                    var pending = _pendingItems.First(p => p.ProdutoId == ri.ProdutoId && p.Lote == ri.Lote);
                    pending.Quantidade += ri.Quantidade;
                }
                RefreshPendentes();
            }
            finally { dialog.Enabled = true; }
        };

        dialog.ShowDialog(this);
    }

    private void VincularReceitaExistente()
    {
        var availableItems = _pendingItems.Where(p => p.Quantidade > 0).ToList();
        if (availableItems.Count == 0) return;

        var availableReceitasList = _availableReceitas.Where(r => r.ReceitaProdutoEstoques is { Count: > 0 }).ToList();
        if (availableReceitasList.Count == 0) { MessageBox.Show("Não há receitas sem venda disponíveis para vincular.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        using var dialog = new Form { Text = "Vincular Receita Existente", StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, ClientSize = new Size(700, 450) };

        var mainTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainTbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        mainTbl.Controls.Add(new Label { Text = "Selecione uma receita sem venda vinculada:", Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);

        var dgv = FormComponents.CreateDataGridView();
        dgv.Columns.Add("Id", "ID");
        dgv.Columns.Add("DataReceita", "Data Receita");
        dgv.Columns.Add("Prescritor", "Prescritor");
        dgv.Columns.Add("Paciente", "Paciente");
        dgv.Columns.Add("Produtos", "Produtos");
        dgv.Columns["Produtos"]!.FillWeight = 30;
        dgv.Columns["Produtos"]!.MinimumWidth = 150;
        dgv.Columns["Prescritor"]!.FillWeight = 20;
        dgv.Columns["Prescritor"]!.MinimumWidth = 100;
        dgv.Columns["Paciente"]!.FillWeight = 20;
        dgv.Columns["Paciente"]!.MinimumWidth = 100;

        foreach (var rec in availableReceitasList)
        {
            var produtosStr = string.Join(", ", rec.ReceitaProdutoEstoques.Select(rpe => $"{rpe.ProdutoEstoque?.Produto?.Descricao ?? $"ID {rpe.ProdutoId}"} ({rpe.Lote}, Qtd: {rpe.Quantidade})"));
            dgv.Rows.Add(rec.Id, rec.DataReceita.ToString("dd/MM/yyyy"), rec.Prescritor?.Nome ?? "", rec.Paciente?.Nome ?? "", produtosStr);
        }
        mainTbl.Controls.Add(dgv, 0, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        var btnVincular = new Button { Text = "Vincular Selecionada", Width = 150, Height = 32, BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 90, Height = 32, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
        btnPanel.Controls.Add(btnVincular); btnPanel.Controls.Add(btnCancel);
        mainTbl.Controls.Add(btnPanel, 0, 2);

        dialog.Controls.Add(mainTbl);
        dialog.CancelButton = btnCancel;

        btnVincular.Click += (_, _) =>
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma receita.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var idx = dgv.SelectedRows[0].Index;
            if (idx < 0 || idx >= availableReceitasList.Count) return;

            var receita = availableReceitasList[idx];
            if (_createdReceitas.Any(r => r.Id == receita.Id)) { MessageBox.Show("Esta receita já foi vinculada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var matchedItems = new List<ControlledItem>();
            foreach (var rpe in receita.ReceitaProdutoEstoques)
            {
                var pending = _pendingItems.FirstOrDefault(p => p.ProdutoId == rpe.ProdutoId && p.Lote == rpe.Lote && p.Quantidade > 0);
                if (pending != null)
                {
                    var matchedQty = Math.Min(pending.Quantidade, rpe.Quantidade);
                    matchedItems.Add(new ControlledItem { ProdutoId = rpe.ProdutoId, Descricao = rpe.ProdutoEstoque?.Produto?.Descricao ?? $"ID {rpe.ProdutoId}", Lote = rpe.Lote ?? "", Quantidade = matchedQty });
                    pending.Quantidade -= matchedQty;
                }
            }

            _createdReceitas.Add(new ReceitaInfo(receita.Id, matchedItems));
            _availableReceitas.Remove(receita);

            RefreshReceitas();
            RefreshPendentes();
            dialog.DialogResult = DialogResult.OK;
            dialog.Close();
        };

        dialog.ShowDialog(this);
    }
}
