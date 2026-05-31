using System.Text.Json;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.Desktop.Forms.Shared;

public class PosForm : Form
{
    private readonly TextBox txtSearch;
    private readonly DataGridView dgvProducts;
    private readonly DataGridView dgvCart;
    private readonly TextBox txtCliente;
    private readonly Button btnBuscarCliente;
    private readonly Label lblClienteId;
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

        txtSearch = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            PlaceholderText = "Buscar produto por código de barras ou descrição..."
        };
        txtSearch.KeyDown += TxtSearch_KeyDown;

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
        dgvCart.Columns.Add("CodBarra", "Cód. Barras");
        dgvCart.Columns.Add("Descricao", "Descrição");
        dgvCart.Columns.Add("Lote", "Lote");
        dgvCart.Columns.Add("Quantidade", "Qtd");
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

        txtCliente = new TextBox { Width = 200, Height = 30, Font = new Font("Segoe UI", 10) };

        btnBuscarCliente = new Button
        {
            Text = "Buscar",
            Width = 70,
            Height = 30,
            Font = new Font("Segoe UI", 9),
            Cursor = Cursors.Hand
        };
        btnBuscarCliente.Click += BtnBuscarCliente_Click;

        lblClienteId = new Label
        {
            Text = "",
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleLeft,
            Width = 80,
            Height = 30
        };

        bottomPanel.Controls.Add(txtCliente);
        bottomPanel.Controls.Add(btnBuscarCliente);
        bottomPanel.Controls.Add(lblClienteId);

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

        Load += async (_, _) => await SearchProducts();
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
        dgvProducts.Columns.Add("Dosagem", "Dosagem");
        dgvProducts.Columns.Add("Controlado", "Controlado");

        foreach (var p in _searchResults)
        {
            dgvProducts.Rows.Add(p.CodBarra, p.Descricao, p.Dosagem, p.Controlado ? "Sim" : "Não");
        }
    }

    private async void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            await SearchProducts();
        }
    }

    private void DgvProducts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _searchResults.Count) return;

        var produto = _searchResults[e.RowIndex];

        var existing = _cart.FirstOrDefault(c => c.ProdutoId == produto.Id);
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
                Lote = "",
                Quantidade = 1
            });
        }

        BindCartGrid();
    }

    private void BindCartGrid()
    {
        dgvCart.Rows.Clear();
        foreach (var item in _cart)
        {
            dgvCart.Rows.Add(item.CodBarra, item.Descricao, item.Lote, item.Quantidade);
        }
        lblTotalItens.Text = $"{_cart.Sum(c => c.Quantidade)} itens";
    }

    private void DgvCart_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _cart.Count) return;
        if (e.ColumnIndex == 3)
        {
            if (int.TryParse(dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(), out var qtd) && qtd > 0)
                _cart[e.RowIndex].Quantidade = qtd;
            else
                _cart[e.RowIndex].Quantidade = 1;

            BindCartGrid();
        }
        else if (e.ColumnIndex == 2)
        {
            _cart[e.RowIndex].Lote = dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
        }
    }

    private void RemoveSelectedFromCart()
    {
        if (dgvCart.SelectedRows.Count > 0 && dgvCart.SelectedRows[0].Index < _cart.Count)
        {
            _cart.RemoveAt(dgvCart.SelectedRows[0].Index);
            BindCartGrid();
        }
    }

    private async void BtnBuscarCliente_Click(object? sender, EventArgs e)
    {
        var nome = txtCliente.Text.Trim();
        if (string.IsNullOrEmpty(nome)) return;

        try
        {
            var clientes = await ApiClient.Instance.GetAsync<List<Cliente>>("api/clientes");
            var match = clientes?.FirstOrDefault(c =>
                c.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase) ||
                c.Cpf.Contains(nome));

            if (match != null)
            {
                txtCliente.Text = match.Nome;
                lblClienteId.Text = $"ID: {match.Id}";
                lblClienteId.Tag = match.Id;
            }
            else
            {
                MessageBox.Show("Cliente não encontrado.", "Busca", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao buscar cliente: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task FinalizarVenda()
    {
        if (_cart.Count == 0)
        {
            MessageBox.Show("Adicione pelo menos um produto ao carrinho.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (lblClienteId.Tag == null)
        {
            MessageBox.Show("Selecione um cliente para a venda.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        foreach (var item in _cart)
        {
            if (string.IsNullOrEmpty(item.Lote))
            {
                MessageBox.Show($"Informe o lote para o produto: {item.Descricao}", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        btnFinalizar.Enabled = false;

        try
        {
            var request = new
            {
                vendedorId = ApiClient.Instance.UserId,
                clienteId = (int)lblClienteId.Tag,
                receitaId = (int?)null,
                produtos = _cart.Select(c => new
                {
                    produtoId = c.ProdutoId,
                    lote = c.Lote,
                    quantidade = c.Quantidade
                }).ToList()
            };

            var response = await ApiClient.Instance.PostAsync("api/vendas", request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Venda registrada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _cart.Clear();
                BindCartGrid();
                txtCliente.Clear();
                lblClienteId.Text = "";
                lblClienteId.Tag = null;
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var msg = json;
                try
                {
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("error", out var err))
                        msg = err.GetString() ?? json;
                    else if (doc.RootElement.TryGetProperty("errors", out var errors))
                        msg = errors.ToString();
                }
                catch { }

                MessageBox.Show($"Erro ao registrar venda: {msg}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
