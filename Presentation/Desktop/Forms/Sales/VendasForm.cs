using System.Text.Json;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Sales;

public class VendasForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Venda> _items = [];

    public VendasForm()
    {
        Text = "Histórico de Vendas";
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
        var btnDelete = new Button { Text = "Cancelar Venda", Width = 110, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        btnDelete.Click += async (_, _) => await DeleteItem();
        topPanel.Controls.Add(btnDelete);
        topPanel.Controls.Add(new Label { Width = 10 });
        tbl.Controls.Add(topPanel, 0, 0);

        dgv = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(dgv, 0, 1);
        _searchFilter = new SearchFilter(topPanel, dgv);

        var statusPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        var lblCount = new Label { TextAlign = ContentAlignment.MiddleLeft, Width = 200, Height = 32, ForeColor = Color.Gray };
        dgv.DataSourceChanged += (_, _) => lblCount.Text = $"Registros: {dgv.Rows.Count}";
        statusPanel.Controls.Add(lblCount);
        tbl.Controls.Add(statusPanel, 0, 2);

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
            dgv.Columns.Add("ClienteId", "Cliente ID");
            dgv.Columns.Add("VendedorId", "Vendedor ID");
            dgv.Columns.Add("ReceitaId", "Receita ID");
            dgv.Columns.Add("Data", "Data");
            dgv.Rows.Clear();
            foreach (var item in _items)
                dgv.Rows.Add(item.Id, item.ClienteId, item.VendedorId, item.ReceitaId?.ToString() ?? "-", item.CriadoEm.ToString("dd/MM/yyyy HH:mm"));
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private async Task DeleteItem()
    {
        if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selecione uma venda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        var idx = dgv.SelectedRows[0].Index;
        if (idx < 0 || idx >= _items.Count) return;
        var item = _items[idx];

        if (MessageBox.Show($"Cancelar venda #{item.Id}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/vendas/{item.Id}");
            if (response.IsSuccessStatusCode) await LoadData();
            else MessageBox.Show($"Erro: {await response.Content.ReadAsStringAsync()}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
