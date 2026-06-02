using System.Text.Json;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Shared;

public class PosForm : Form
{
    private readonly TextBox txtSearch;
    private readonly System.Windows.Forms.Timer _debounceTimer;
    private readonly DataGridView dgvProducts;
    private readonly DataGridView dgvCart;
    private readonly SearchableComboBox cmbCliente;
    private readonly Label lblTotalItens;
    private readonly Button btnFinalizar;
    private readonly Button btnRemover;
    private List<Produto> _searchResults = [];
    private readonly List<CartItem> _cart = [];

    private class CartItem
    {
        public int ProdutoId { get; set; }
        public int CodBarra { get; set; }
        public string Descricao { get; set; } = "";
        public string Lote { get; set; } = "";
        public int Quantidade { get; set; } = 1;
        public bool Controlado { get; set; }
    }

    public PosForm()
    {
        Text = "PDV - Registrar Venda";
        Dock = DockStyle.Fill;

        var mainTable = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(10)
        };
        mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

        _debounceTimer = new System.Windows.Forms.Timer { Interval = 300 };
        _debounceTimer.Tick += async (_, _) => { _debounceTimer.Stop(); await SearchProducts(); };

        txtSearch = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            PlaceholderText = "Buscar produto por código de barras ou descrição..."
        };
        txtSearch.TextChanged += (_, _) => { _debounceTimer.Stop(); _debounceTimer.Start(); };

        var btnSearch = new Button
        {
            Text = "Buscar",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            BackColor = Color.DarkBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSearch.Click += async (_, _) => await SearchProducts();

        searchPanel.Controls.Add(txtSearch, 0, 0);
        searchPanel.Controls.Add(btnSearch, 1, 0);

        mainTable.Controls.Add(searchPanel, 0, 0);
        mainTable.SetColumnSpan(searchPanel, 2);

        dgvProducts = CreateDataGrid();
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.CellDoubleClick += DgvProducts_CellDoubleClick;
        mainTable.Controls.Add(dgvProducts, 0, 1);

        var rightPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        dgvCart = CreateDataGrid();
        dgvCart.Columns.Add("Posicao", "#");
        dgvCart.Columns.Add("CodBarra", "Cód. Barras");
        dgvCart.Columns.Add("Descricao", "Descrição");
        dgvCart.Columns.Add("Lote", "Lote");
        dgvCart.Columns.Add("Quantidade", "Qtd");
        dgvCart.Columns["Descricao"]!.FillWeight = 40;
        dgvCart.Columns["Descricao"]!.MinimumWidth = 120;
        dgvCart.ReadOnly = false;
        dgvCart.CellEndEdit += DgvCart_CellEndEdit;
        rightPanel.Controls.Add(dgvCart, 0, 0);

        var bottomPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 5, 0, 0)
        };

        bottomPanel.Controls.Add(new Label
        {
            Text = "Cliente:",
            TextAlign = ContentAlignment.MiddleLeft,
            Width = 55,
            Height = 30,
            Font = new Font("Segoe UI", 10)
        });

        cmbCliente = new SearchableComboBox { Width = 260, Height = 30, PlaceholderText = "Buscar cliente..." };
        bottomPanel.Controls.Add(cmbCliente);

        rightPanel.Controls.Add(bottomPanel, 0, 1);

        var actionPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 5, 0, 0)
        };

        lblTotalItens = new Label
        {
            Text = "0 itens",
            TextAlign = ContentAlignment.MiddleRight,
            Width = 100,
            Height = 30,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DarkBlue
        };

        btnRemover = new Button
        {
            Text = "Remover Item",
            Width = 120,
            Height = 30,
            BackColor = Color.Coral,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnRemover.Click += (_, _) => RemoveSelectedFromCart();

        btnFinalizar = new Button
        {
            Text = "Finalizar Venda",
            Width = 140,
            Height = 30,
            BackColor = Color.Green,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnFinalizar.Click += async (_, _) => await FinalizarVenda();

        actionPanel.Controls.Add(btnFinalizar);
        actionPanel.Controls.Add(btnRemover);
        actionPanel.Controls.Add(lblTotalItens);

        rightPanel.Controls.Add(actionPanel, 0, 2);

        mainTable.Controls.Add(rightPanel, 1, 1);

        var statusBar = new Label
        {
            Text = $"Vendedor: {ApiClient.Instance.UserName}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.Gray,
            Font = new Font("Segoe UI", 9)
        };
        mainTable.Controls.Add(statusBar, 0, 2);
        mainTable.SetColumnSpan(statusBar, 2);

        Controls.Add(mainTable);

        Load += async (_, _) =>
        {
            await SearchProducts();
            var clientes = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes") ?? [];
            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "Nome";
            cmbCliente.ValueMember = "Id";
        };
    }

    private static DataGridView CreateDataGrid()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 10)
        };
        return dgv;
    }

    private async Task SearchProducts()
    {
        var query = txtSearch.Text.Trim();
        if (string.IsNullOrEmpty(query))
        {
            try
            {
                _searchResults = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];
            }
            catch
            {
                _searchResults = [];
            }
        }
        else
        {
            try
            {
                var all = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];
                _searchResults = all.Where(p =>
                    p.Descricao.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.CodBarra.ToString().Contains(query)).ToList();
            }
            catch
            {
                _searchResults = [];
            }
        }

        BindProductGrid();
    }

    private void BindProductGrid()
    {
        dgvProducts.Columns.Clear();
        dgvProducts.Columns.Add("CodBarra", "Cód. Barras");
        dgvProducts.Columns.Add("Descricao", "Descrição");
        dgvProducts.Columns.Add("Estoque", "Estoque");
        dgvProducts.Columns.Add("Dosagem", "Dosagem");
        dgvProducts.Columns.Add("Controlado", "Controlado");
        dgvProducts.Columns["Descricao"]!.FillWeight = 40;
        dgvProducts.Columns["Descricao"]!.MinimumWidth = 120;

        foreach (var p in _searchResults)
        {
            var inCart = _cart.Where(c => c.ProdutoId == p.Id).Sum(c => c.Quantidade);
            dgvProducts.Rows.Add(p.CodBarra, p.Descricao, p.Estoque - inCart, p.Dosagem, p.Controlado ? "Sim" : "Não");
        }
    }

    private async void DgvProducts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _searchResults.Count) return;

        var produto = _searchResults[e.RowIndex];

        var lote = "";

        if (produto.Controlado)
        {
            var estoques = await ApiClient.Instance.GetAsync<List<ProdutoEstoque>>($"api/produtos/{produto.Id}/estoques") ?? [];
            var availableLotes = estoques.Where(es => es.Quantidade > 0 && !string.IsNullOrEmpty(es.Lote)).ToList();

            if (availableLotes.Count == 0)
            {
                MessageBox.Show($"Não há lotes em estoque para o produto controlado: {produto.Descricao}", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (availableLotes.Count == 1)
            {
                lote = availableLotes[0].Lote!;
            }
            else
            {
                using var lotDialog = new Form
                {
                    Text = $"Selecionar Lote - {produto.Descricao}",
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ClientSize = new Size(350, 130)
                };

                var ltbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
                ltbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
                ltbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
                ltbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

                ltbl.Controls.Add(new Label { Text = "Selecione o lote:", TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10) }, 0, 0);

                var cmbLote = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione o lote..." };
                cmbLote.DataSource = availableLotes;
                cmbLote.DisplayMember = "Lote";
                ltbl.Controls.Add(cmbLote, 0, 1);

                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
                var btnOk = new Button { Text = "OK", Width = 80, Height = 30, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 30, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
                btnPanel.Controls.Add(btnOk);
                btnPanel.Controls.Add(btnCancel);
                ltbl.Controls.Add(btnPanel, 0, 2);

                lotDialog.Controls.Add(ltbl);
                lotDialog.AcceptButton = btnOk;
                lotDialog.CancelButton = btnCancel;

                if (lotDialog.ShowDialog(this) != DialogResult.OK || cmbLote.SelectedItem is not ProdutoEstoque selected)
                    return;

                lote = selected.Lote!;
            }
        }

        var existing = _cart.FirstOrDefault(c => c.ProdutoId == produto.Id && c.Lote == lote);
        if (existing != null)
        {
            existing.Quantidade++;
        }
        else
        {
            _cart.Add(new CartItem
            {
                ProdutoId = produto.Id,
                CodBarra = produto.CodBarra,
                Descricao = produto.Descricao,
                Lote = lote,
                Quantidade = 1,
                Controlado = produto.Controlado
            });
        }

        BindCartGrid();
        BindProductGrid();
    }

    private void BindCartGrid()
    {
        dgvCart.Rows.Clear();
        for (int i = 0; i < _cart.Count; i++)
        {
            var item = _cart[i];
            dgvCart.Rows.Add(i + 1, item.CodBarra, item.Descricao, item.Lote, item.Quantidade);
        }
        lblTotalItens.Text = $"{_cart.Sum(c => c.Quantidade)} itens";
    }

    private void DgvCart_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _cart.Count) return;
        if (e.ColumnIndex == 4)
        {
            if (int.TryParse(dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out var qtd) && qtd > 0)
                _cart[e.RowIndex].Quantidade = qtd;
            else
                _cart[e.RowIndex].Quantidade = 1;

            BindCartGrid();
            BindProductGrid();
        }
    }

    private void RemoveSelectedFromCart()
    {
        if (dgvCart.SelectedRows.Count > 0 && dgvCart.SelectedRows[0].Index < _cart.Count)
        {
            _cart.RemoveAt(dgvCart.SelectedRows[0].Index);
            BindCartGrid();
            BindProductGrid();
        }
    }

    private async Task FinalizarVenda()
    {
        if (_cart.Count == 0)
        {
            MessageBox.Show("Adicione pelo menos um produto ao carrinho.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbCliente.SelectedValue == null)
        {
            MessageBox.Show("Selecione um cliente para a venda.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var controlados = _cart.Where(c => c.Controlado).ToList();

        var receitaIds = new List<int>();

        if (controlados.Count > 0)
        {
            var clientes = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes") ?? [];
            var prescritores = await ApiClient.Instance.GetAsync<List<Prescritor>>("api/prescritores") ?? [];
            var allProdutos = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];
            var todasReceitas = await ApiClient.Instance.GetAsync<List<Receita>>("api/receitas") ?? [];
            var availableReceitas = todasReceitas.Where(r => r.VendaId == null).ToList();

            var controlledItems = controlados.Select(c =>
                new PrescriptionDialog.ControlledItem
                {
                    ProdutoId = c.ProdutoId,
                    Descricao = c.Descricao,
                    Lote = c.Lote,
                    Quantidade = c.Quantidade
                }).ToList();

            using var prescriptionDialog = new PrescriptionDialog(controlledItems, clientes, prescritores, allProdutos, availableReceitas);
            if (prescriptionDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            receitaIds = prescriptionDialog.CreatedReceitas.Select(r => r.Id).ToList();
        }

        btnFinalizar.Enabled = false;

        try
        {
            var request = new
            {
                vendedorId = ApiClient.Instance.UserId,
                clienteId = (int)cmbCliente.SelectedValue,
                produtos = _cart.Select(c => new
                {
                    produtoId = c.ProdutoId,
                    quantidade = c.Quantidade
                }).ToList(),
                receitaIds = receitaIds.Count > 0 ? receitaIds : null
            };

            var response = await ApiClient.Instance.PostAsync("api/vendas", request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Venda registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _cart.Clear();
                BindCartGrid();
                cmbCliente.ClearSelection();
                await SearchProducts();
            }
            else
            {
                MessageBox.Show($"Erro ao registrar venda: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao registrar venda: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnFinalizar.Enabled = true;
        }
    }
}
