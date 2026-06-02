using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;

namespace minipdv.Presentation.Desktop.Forms.Products;

public class ProdutosForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Produto> _produtos = [];

    public ProdutosForm()
    {
        Text = "Produtos";
        Dock = DockStyle.Fill;

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(10) };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

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

        dgv = new DataGridView
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
        dgv.CellDoubleClick += (_, _) => ViewItem();
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
            _produtos = await ApiClient.Instance.GetAsync<List<Produto>>("api/produtos") ?? [];

            dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("CodBarra", "Cód. Barras");
            dgv.Columns.Add("Descricao", "Descrição");
            dgv.Columns.Add("Dosagem", "Dosagem");
            dgv.Columns.Add("Estoque", "Estoque");
            dgv.Columns.Add("Ativo", "Ativo");
            dgv.Columns.Add("Controlado", "Controlado");
            dgv.Columns.Add("RegistroMS", "Reg. MS");
            dgv.Columns.Add("GrupoNome", "Grupo");
            dgv.Columns["Descricao"]!.FillWeight = 40;
            dgv.Columns["Descricao"]!.MinimumWidth = 120;

            dgv.Rows.Clear();
            foreach (var p in _produtos)
                dgv.Rows.Add(p.Id, p.CodBarra, p.Descricao, p.Dosagem, p.Estoque, p.Ativo ? "Sim" : "Não", p.Controlado ? "Sim" : "Não", p.RegistroMS ?? "", p.Grupo?.Nome ?? "");
            _searchFilter.ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Produto? GetSelected()
    {
        if (dgv.SelectedRows.Count > 0 && dgv.SelectedRows[0].Index < _produtos.Count)
            return _produtos[dgv.SelectedRows[0].Index];
        return null;
    }

    private async Task ViewItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        var grupos = await ApiClient.Instance.GetAsync<List<ProdutoGrupo>>("api/produtogrupos") ?? [];
        var principios = await ApiClient.Instance.GetAsync<List<PrincipioAtivo>>("api/principiosativos") ?? [];
        var fabricantes = await ApiClient.Instance.GetAsync<List<Fabricante>>("api/fabricantes") ?? [];

        using var dialog = new Form
        {
            Text = "Visualizar Produto",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(450, 420)
        };

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 10, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Descrição:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtDesc = new TextBox { Text = item.Descricao, ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDesc, 1, 0);

        tbl.Controls.Add(new Label { Text = "Cód. Barras:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtCod = new TextBox { Text = item.CodBarra.ToString(), ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtCod, 1, 1);

        tbl.Controls.Add(new Label { Text = "Dosagem:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtDosagem = new TextBox { Text = item.Dosagem, ReadOnly = true, BackColor = SystemColors.Control, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDosagem, 1, 2);

        tbl.Controls.Add(new Label { Text = "Grupo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var cmbGrupo = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        cmbGrupo.SelectedValue = item.ProdutoGrupoId;
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(new Label { Text = "Princ. Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var cmbPrinc = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        cmbPrinc.SelectedValue = item.PrincipioAtivoId;
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(new Label { Text = "Fabricante:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var cmbFab = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        if (item.FabricanteId.HasValue)
            cmbFab.SelectedValue = item.FabricanteId.Value;
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(new Label { Text = "Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 6);
        var chkAtivo = new CheckBox { Text = "Ativo", Checked = item.Ativo, Enabled = false, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkAtivo, 1, 6);

        tbl.Controls.Add(new Label { Text = "Controlado:", TextAlign = ContentAlignment.MiddleLeft }, 0, 7);
        var chkControlado = new CheckBox { Text = "Controlado", Checked = item.Controlado, Enabled = false, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkControlado, 1, 7);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Text = item.RegistroMS ?? "", Enabled = false, ReadOnly = true, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = new Button { Text = "Editar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnDelete = new Button { Text = "Excluir", Width = 80, Height = 32, BackColor = Color.Crimson, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnClose = new Button { Text = "Fechar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnEdit);
        btnPanel.Controls.Add(btnDelete);
        btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 9);

        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) =>
        {
            dialog.Close();
            await EditItem();
        };
        btnDelete.Click += async (_, _) =>
        {
            dialog.Close();
            await DeleteItem();
        };

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        var grupos = await ApiClient.Instance.GetAsync<List<ProdutoGrupo>>("api/produtogrupos") ?? [];
        var principios = await ApiClient.Instance.GetAsync<List<PrincipioAtivo>>("api/principiosativos") ?? [];
        var fabricantes = await ApiClient.Instance.GetAsync<List<Fabricante>>("api/fabricantes") ?? [];

        using var dialog = new Form
        {
            Text = "Novo Produto",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(450, 420)
        };

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 10, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Descrição:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtDesc = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDesc, 1, 0);

        tbl.Controls.Add(new Label { Text = "Cód. Barras:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtCod = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtCod, 1, 1);

        tbl.Controls.Add(new Label { Text = "Dosagem:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtDosagem = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDosagem, 1, 2);

        tbl.Controls.Add(new Label { Text = "Grupo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var cmbGrupo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(new Label { Text = "Princ. Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var cmbPrinc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(new Label { Text = "Fabricante:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var cmbFab = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(new Label { Text = "Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 6);
        var chkAtivo = new CheckBox { Text = "Ativo", Checked = true, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkAtivo, 1, 6);

        tbl.Controls.Add(new Label { Text = "Controlado:", TextAlign = ContentAlignment.MiddleLeft }, 0, 7);
        var chkControlado = new CheckBox { Text = "Controlado", Checked = false, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkControlado, 1, 7);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk);
        btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 9);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var isControlado = chkControlado.Checked;
                var regMs = mtxtRegMs.Text.Trim();
                var request = new
                {
                    descricao = txtDesc.Text.Trim(),
                    ativo = chkAtivo.Checked,
                    codBarra = int.Parse(txtCod.Text.Trim()),
                    controlado = isControlado,
                    dosagem = txtDosagem.Text.Trim(),
                    registroMS = mtxtRegMs.MaskCompleted ? regMs : null,
                    produtoGrupoId = (int)(cmbGrupo.SelectedValue ?? 0),
                    fabricanteId = (int?)cmbFab.SelectedValue,
                    principioAtivoId = (int)(cmbPrinc.SelectedValue ?? 0)
                };

                var response = await ApiClient.Instance.PostAsync("api/produtos", request);
                if (response.IsSuccessStatusCode)
                {
                    txtDesc.Clear(); txtCod.Clear(); txtDosagem.Clear(); mtxtRegMs.Clear();
                    cmbGrupo.ClearSelection(); cmbPrinc.ClearSelection(); cmbFab.ClearSelection();
                    chkAtivo.Checked = true; chkControlado.Checked = false;
                    await LoadData();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    var err = await ErrorHelper.ExtractAsync(response);
                    MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dialog.Enabled = true;
            }
        };

        dialog.ShowDialog(this);
    }

    private async Task EditItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        var grupos = await ApiClient.Instance.GetAsync<List<ProdutoGrupo>>("api/produtogrupos") ?? [];
        var principios = await ApiClient.Instance.GetAsync<List<PrincipioAtivo>>("api/principiosativos") ?? [];
        var fabricantes = await ApiClient.Instance.GetAsync<List<Fabricante>>("api/fabricantes") ?? [];

        using var dialog = new Form
        {
            Text = "Editar Produto",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(450, 420)
        };

        var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 10, Padding = new Padding(15) };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        tbl.Controls.Add(new Label { Text = "Descrição:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var txtDesc = new TextBox { Text = item.Descricao, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDesc, 1, 0);

        tbl.Controls.Add(new Label { Text = "Cód. Barras:", TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        var txtCod = new TextBox { Text = item.CodBarra.ToString(), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtCod, 1, 1);

        tbl.Controls.Add(new Label { Text = "Dosagem:", TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        var txtDosagem = new TextBox { Text = item.Dosagem, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(txtDosagem, 1, 2);

        tbl.Controls.Add(new Label { Text = "Grupo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        var cmbGrupo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        cmbGrupo.SelectedValue = item.ProdutoGrupoId;
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(new Label { Text = "Princ. Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
        var cmbPrinc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        cmbPrinc.SelectedValue = item.PrincipioAtivoId;
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(new Label { Text = "Fabricante:", TextAlign = ContentAlignment.MiddleLeft }, 0, 5);
        var cmbFab = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        if (item.FabricanteId.HasValue)
            cmbFab.SelectedValue = item.FabricanteId.Value;
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(new Label { Text = "Ativo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 6);
        var chkAtivo = new CheckBox { Text = "Ativo", Checked = item.Ativo, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkAtivo, 1, 6);

        tbl.Controls.Add(new Label { Text = "Controlado:", TextAlign = ContentAlignment.MiddleLeft }, 0, 7);
        var chkControlado = new CheckBox { Text = "Controlado", Checked = item.Controlado, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
        tbl.Controls.Add(chkControlado, 1, 7);

        tbl.Controls.Add(new Label { Text = "Registro MS:", TextAlign = ContentAlignment.MiddleLeft }, 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Text = item.RegistroMS ?? "", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = new Button { Text = "Salvar", Width = 80, Height = 32, BackColor = Color.DarkBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        var btnCancel = new Button { Text = "Cancelar", Width = 80, Height = 32, Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 10, 0) };
        btnPanel.Controls.Add(btnOk);
        btnPanel.Controls.Add(btnCancel);
        tbl.Controls.Add(btnPanel, 0, 9);

        dialog.Controls.Add(tbl);
        dialog.AcceptButton = btnOk;
        dialog.CancelButton = btnCancel;

        btnOk.Click += async (_, _) =>
        {
            try
            {
                dialog.Enabled = false;

                var isControlado = chkControlado.Checked;
                var regMs = mtxtRegMs.Text.Trim();
                var request = new
                {
                    id = item.Id,
                    descricao = txtDesc.Text.Trim(),
                    ativo = chkAtivo.Checked,
                    codBarra = int.Parse(txtCod.Text.Trim()),
                    controlado = isControlado,
                    dosagem = txtDosagem.Text.Trim(),
                    registroMS = mtxtRegMs.MaskCompleted ? regMs : null,
                    produtoGrupoId = (int)(cmbGrupo.SelectedValue ?? 0),
                    fabricanteId = (int?)cmbFab.SelectedValue,
                    principioAtivoId = (int)(cmbPrinc.SelectedValue ?? 0)
                };

                var response = await ApiClient.Instance.PutAsync($"api/produtos/{item.Id}", request);
                if (response.IsSuccessStatusCode)
                {
                    txtDesc.Clear(); txtCod.Clear(); txtDosagem.Clear(); mtxtRegMs.Clear();
                    cmbGrupo.ClearSelection(); cmbPrinc.ClearSelection(); cmbFab.ClearSelection();
                    chkAtivo.Checked = true; chkControlado.Checked = false;
                    await LoadData();
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                }
                else
                {
                    var err = await ErrorHelper.ExtractAsync(response);
                    MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dialog.Enabled = true;
            }

        };

        dialog.ShowDialog(this);
    }

    private async Task DeleteItem()
    {
        var item = GetSelected();
        if (item == null) { MessageBox.Show("Selecione um produto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

        if (MessageBox.Show($"Excluir produto '{item.Descricao}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var response = await ApiClient.Instance.DeleteAsync($"api/produtos/{item.Id}");
            if (response.IsSuccessStatusCode)
                await LoadData();
            else
            {
                var err = await ErrorHelper.ExtractAsync(response);
                MessageBox.Show($"Erro: {err}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
