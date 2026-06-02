using System.Text.Json;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Vendas;

public class VendasForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Venda> _items = [];

    public VendasForm()
    {
        Text = "Histórico de Vendas";
        Dock = DockStyle.Fill;

        var tbl = FormComponents.CreateStandardLayout();
        var topPanel = FormComponents.CreateToolbar();

        var btnRefresh = FormComponents.CreateRefreshButton();
        btnRefresh.Click += async (_, _) => await LoadData();
        topPanel.Controls.Add(btnRefresh);
        topPanel.Controls.Add(new Label { Width = 10 });

        var btnDelete = FormComponents.CreateDeleteButton(110, "Cancelar Venda");
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
            _items = await ApiClient.Instance.GetAsync<List<Venda>>("api/vendas") ?? [];
            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "Venda ID");
            dgv.Columns.Add("ClienteNome", "Cliente");
            dgv.Columns.Add("VendedorNome", "Vendedor");
            dgv.Columns.Add("Data", "Data");
            dgv.Columns.Add("CanceladoEm", "Cancelado Em");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.Cliente?.Nome ?? "", item.Vendedor?.Nome ?? "", item.CriadoEm.ToString("dd/MM/yyyy HH:mm"), item.CanceladoEm?.ToString("dd/MM/yyyy HH:mm") ?? "");
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task ViewItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma venda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        Venda? fullVenda = null;
        try { fullVenda = await ApiClient.Instance.GetAsync<Venda>($"api/vendas/{item.Id}"); }
        catch (Exception ex) { MessageBox.Show($"Erro ao carregar venda: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        if (fullVenda == null) { MessageBox.Show("Venda não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        using var dialog = FormComponents.CreateDialog("Visualizar Venda", 500, 350);
        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        var infoTbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Padding = new Padding(5) };
        infoTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        infoTbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        infoTbl.Controls.Add(new Label { Text = "Cliente:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        infoTbl.Controls.Add(new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Text = fullVenda.Cliente?.Nome ?? "" }, 1, 0);

        infoTbl.Controls.Add(new Label { Text = "Vendedor:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        infoTbl.Controls.Add(new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Text = fullVenda.Vendedor?.Nome ?? "" }, 1, 1);

        infoTbl.Controls.Add(new Label { Text = "Data:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        infoTbl.Controls.Add(new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Text = fullVenda.CriadoEm.ToString("dd/MM/yyyy HH:mm") }, 1, 2);

        infoTbl.Controls.Add(new Label { Text = "Cancelado Em:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        infoTbl.Controls.Add(new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Text = fullVenda.CanceladoEm?.ToString("dd/MM/yyyy HH:mm") ?? "" }, 1, 3);

        tbl.Controls.Add(infoTbl, 0, 0);

        var dgvItems = FormComponents.CreateDataGridView();
        dgvItems.Columns.Add("Produto", "Produto");
        dgvItems.Columns.Add("Quantidade", "Qtd");
        dgvItems.Columns["Produto"]!.FillWeight = 60;
        dgvItems.Columns["Produto"]!.MinimumWidth = 120;
        foreach (var vi in fullVenda.VendaItens)
            dgvItems.Rows.Add(vi.Produto?.Descricao ?? $"ID {vi.ProdutoId}", vi.Quantidade);
        tbl.Controls.Add(dgvItems, 0, 1);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        var btnCancelar = new Button { Text = "Cancelar Venda", Width = 120, Height = 35, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 10, 0) };
        var btnClose = new Button { Text = "Fechar", Width = 90, Height = 35, Cursor = Cursors.Hand, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnClose); btnPanel.Controls.Add(btnCancelar);
        tbl.Controls.Add(btnPanel, 0, 2);

        dialog.Controls.Add(tbl);

        btnCancelar.Click += (_, _) => { dialog.Close(); _ = DeleteItem(); };
        btnClose.Click += (_, _) => dialog.Close();

        dialog.ShowDialog(this);
    }

    private async Task DeleteItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma venda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        if (MessageBox.Show($"Cancelar venda #{item.Id}? O estoque será restaurado.", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/vendas/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await ErrorHelper.ExtractAsync(response)}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
