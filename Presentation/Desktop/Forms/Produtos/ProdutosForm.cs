using System.Globalization;
using minipdv.Domain.Entities;
using minipdv.Presentation.Desktop.Components.Controls;
using minipdv.Presentation.Desktop.Components.Helpers;

namespace minipdv.Presentation.Desktop.Forms.Produtos;

public class ProdutosForm : Form
{
    private readonly DataGridView dgv;
    private readonly SearchFilter _searchFilter;
    private List<Produto> _produtos = [];

    public ProdutosForm()
    {
        Text = "Produtos";
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

        using var dialog = FormComponents.CreateDialog("Visualizar Produto", 450, 420);
        var tbl = FormComponents.CreateDialogLayout(2, 10, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Descrição:"), 0, 0);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Descricao), 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Cód. Barras:"), 0, 1);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.CodBarra.ToString()), 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Dosagem:"), 0, 2);
        tbl.Controls.Add(FormComponents.CreateReadOnlyTextBox(item.Dosagem), 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Grupo:"), 0, 3);
        var cmbGrupo = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        cmbGrupo.SelectedValue = item.ProdutoGrupoId;
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Princ. Ativo:"), 0, 4);
        var cmbPrinc = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        cmbPrinc.SelectedValue = item.PrincipioAtivoId;
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricante:"), 0, 5);
        var cmbFab = new SearchableComboBox { Enabled = false, Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        if (item.FabricanteId.HasValue)
            cmbFab.SelectedValue = item.FabricanteId.Value;
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 6);
        tbl.Controls.Add(FormComponents.CreateCheckBox("Ativo", item.Ativo, enabled: false), 1, 6);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Controlado:"), 0, 7);
        tbl.Controls.Add(FormComponents.CreateCheckBox("Controlado", item.Controlado, enabled: false), 1, 7);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Text = item.RegistroMS ?? "", Enabled = false, ReadOnly = true, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnEdit = FormComponents.CreateEditButton(80);
        var btnDelete = FormComponents.CreateDeleteButton(80);
        var btnClose = FormComponents.CreateCloseButton(80);
        btnPanel.Controls.Add(btnEdit); btnPanel.Controls.Add(btnDelete); btnPanel.Controls.Add(btnClose);
        tbl.Controls.Add(btnPanel, 0, 9);

        dialog.Controls.Add(tbl);
        dialog.CancelButton = btnClose;

        btnEdit.Click += async (_, _) => { dialog.Close(); await EditItem(); };
        btnDelete.Click += async (_, _) => { dialog.Close(); await DeleteItem(); };

        dialog.ShowDialog(this);
    }

    private async Task AddItem()
    {
        var grupos = await ApiClient.Instance.GetAsync<List<ProdutoGrupo>>("api/produtogrupos") ?? [];
        var principios = await ApiClient.Instance.GetAsync<List<PrincipioAtivo>>("api/principiosativos") ?? [];
        var fabricantes = await ApiClient.Instance.GetAsync<List<Fabricante>>("api/fabricantes") ?? [];

        using var dialog = FormComponents.CreateDialog("Novo Produto", 450, 420);
        var tbl = FormComponents.CreateDialogLayout(2, 10, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Descrição:"), 0, 0);
        var txtDesc = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtDesc, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Cód. Barras:"), 0, 1);
        var txtCod = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtCod, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Dosagem:"), 0, 2);
        var txtDosagem = FormComponents.CreateTextBox();
        tbl.Controls.Add(txtDosagem, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Grupo:"), 0, 3);
        var cmbGrupo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Princ. Ativo:"), 0, 4);
        var cmbPrinc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricante:"), 0, 5);
        var cmbFab = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 6);
        var chkAtivo = FormComponents.CreateCheckBox("Ativo", true);
        tbl.Controls.Add(chkAtivo, 1, 6);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Controlado:"), 0, 7);
        var chkControlado = FormComponents.CreateCheckBox("Controlado");
        tbl.Controls.Add(chkControlado, 1, 7);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
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

        using var dialog = FormComponents.CreateDialog("Editar Produto", 450, 420);
        var tbl = FormComponents.CreateDialogLayout(2, 10, 120);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Descrição:"), 0, 0);
        var txtDesc = FormComponents.CreateTextBox(item.Descricao);
        tbl.Controls.Add(txtDesc, 1, 0);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Cód. Barras:"), 0, 1);
        var txtCod = FormComponents.CreateTextBox(item.CodBarra.ToString());
        tbl.Controls.Add(txtCod, 1, 1);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Dosagem:"), 0, 2);
        var txtDosagem = FormComponents.CreateTextBox(item.Dosagem);
        tbl.Controls.Add(txtDosagem, 1, 2);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Grupo:"), 0, 3);
        var cmbGrupo = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbGrupo.DataSource = grupos;
        cmbGrupo.DisplayMember = "Nome";
        cmbGrupo.ValueMember = "Id";
        cmbGrupo.SelectedValue = item.ProdutoGrupoId;
        tbl.Controls.Add(cmbGrupo, 1, 3);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Princ. Ativo:"), 0, 4);
        var cmbPrinc = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "Selecione..." };
        cmbPrinc.DataSource = principios;
        cmbPrinc.DisplayMember = "Nome";
        cmbPrinc.ValueMember = "Id";
        cmbPrinc.SelectedValue = item.PrincipioAtivoId;
        tbl.Controls.Add(cmbPrinc, 1, 4);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Fabricante:"), 0, 5);
        var cmbFab = new SearchableComboBox { Dock = DockStyle.Fill, PlaceholderText = "(opcional)" };
        cmbFab.DataSource = fabricantes;
        cmbFab.DisplayMember = "NomeFantasia";
        cmbFab.ValueMember = "Id";
        if (item.FabricanteId.HasValue)
            cmbFab.SelectedValue = item.FabricanteId.Value;
        tbl.Controls.Add(cmbFab, 1, 5);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Ativo:"), 0, 6);
        var chkAtivo = FormComponents.CreateCheckBox("Ativo", item.Ativo);
        tbl.Controls.Add(chkAtivo, 1, 6);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Controlado:"), 0, 7);
        var chkControlado = FormComponents.CreateCheckBox("Controlado", item.Controlado);
        tbl.Controls.Add(chkControlado, 1, 7);

        tbl.Controls.Add(FormComponents.CreateFieldLabel("Registro MS:"), 0, 8);
        var mtxtRegMs = new MaskedTextBox { Mask = "0.0000.0000.000-0", Culture = CultureInfo.InvariantCulture, Text = item.RegistroMS ?? "", Dock = DockStyle.Fill, Font = FormComponents.DefaultFont, HidePromptOnLeave = true };
        tbl.Controls.Add(mtxtRegMs, 1, 8);

        var btnPanel = FormComponents.CreateDialogButtonPanel();
        tbl.SetColumnSpan(btnPanel, 2);
        var btnOk = FormComponents.CreateSaveButton();
        var btnCancel = FormComponents.CreateCancelButton();
        btnPanel.Controls.Add(btnOk); btnPanel.Controls.Add(btnCancel);
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
