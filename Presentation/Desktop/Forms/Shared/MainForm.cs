using minipdv.Presentation.Desktop.Forms.Auth;
using minipdv.Presentation.Desktop.Forms.Customers;
using minipdv.Presentation.Desktop.Forms.Products;
using minipdv.Presentation.Desktop.Forms.Sales;
using minipdv.Presentation.Desktop.Forms.Services;

namespace minipdv.Presentation.Desktop.Forms.Shared;

public class MainForm : Form
{
    private readonly Panel pnlContent;
    private readonly FlowLayoutPanel flowMenu;

    public MainForm()
    {
        var tipo = ApiClient.Instance.UserTipo;

        Text = $"MiniPDV - {ApiClient.Instance.UserName}";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Size = new Size(1200, 800);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel1,
            IsSplitterFixed = true,
            SplitterWidth = 1,
            Panel1MinSize = 160,
            SplitterDistance = 200
        };

        var pnlMenu = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.FromArgb(45, 45, 48)
        };
        pnlMenu.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        pnlMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblUserInfo = new Label
        {
            Text = $"{ApiClient.Instance.UserName} ({tipo})",
            Dock = DockStyle.Fill,
            AutoSize = false,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(30, 30, 33),
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Padding = new Padding(5)
        };
        pnlMenu.Controls.Add(lblUserInfo, 0, 0);

        flowMenu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(5),
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.FromArgb(45, 45, 48)
        };

        var btnPos = CreateMenuButton("PDV (Vendas)", "Registrar vendas", Color.FromArgb(0, 200, 83));
        btnPos.Click += (_, _) => OpenForm<PosForm>();
        flowMenu.Controls.Add(btnPos);

        if (tipo == "Farmaceutico" || tipo == "Administrador")
        {
            AddMenuSeparator(flowMenu, "Cadastros");

            var btnClientes = CreateMenuButton("Clientes", "Gerenciar clientes", Color.FromArgb(33, 150, 243));
            btnClientes.Click += (_, _) => OpenForm<ClientesForm>();
            flowMenu.Controls.Add(btnClientes);

            var btnProdutos = CreateMenuButton("Produtos", "Gerenciar produtos", Color.FromArgb(33, 150, 243));
            btnProdutos.Click += (_, _) => OpenForm<ProdutosForm>();
            flowMenu.Controls.Add(btnProdutos);

            var btnGrupos = CreateMenuButton("Grupos de Produtos", "", Color.FromArgb(33, 150, 243));
            btnGrupos.Click += (_, _) => OpenForm<ProdutoGruposForm>();
            flowMenu.Controls.Add(btnGrupos);

            var btnFabricantes = CreateMenuButton("Fabricantes", "", Color.FromArgb(33, 150, 243));
            btnFabricantes.Click += (_, _) => OpenForm<FabricantesForm>();
            flowMenu.Controls.Add(btnFabricantes);

            var btnPrincipios = CreateMenuButton("Princípios Ativos", "", Color.FromArgb(33, 150, 243));
            btnPrincipios.Click += (_, _) => OpenForm<PrincipiosAtivosForm>();
            flowMenu.Controls.Add(btnPrincipios);

            var btnEstoques = CreateMenuButton("Estoques", "", Color.FromArgb(33, 150, 243));
            btnEstoques.Click += (_, _) => OpenForm<ProdutoEstoquesForm>();
            flowMenu.Controls.Add(btnEstoques);

            AddMenuSeparator(flowMenu, "Serviços");

            var btnPrescritores = CreateMenuButton("Prescritores", "", Color.FromArgb(0, 188, 212));
            btnPrescritores.Click += (_, _) => OpenForm<PrescritoresForm>();
            flowMenu.Controls.Add(btnPrescritores);

            var btnReceitas = CreateMenuButton("Receitas", "", Color.FromArgb(0, 188, 212));
            btnReceitas.Click += (_, _) => OpenForm<ReceitasForm>();
            flowMenu.Controls.Add(btnReceitas);

            AddMenuSeparator(flowMenu, "Consultas");

            var btnVendas = CreateMenuButton("Histórico de Vendas", "", Color.FromArgb(255, 152, 0));
            btnVendas.Click += (_, _) => OpenForm<VendasForm>();
            flowMenu.Controls.Add(btnVendas);
        }

        if (tipo == "Administrador")
        {
            AddMenuSeparator(flowMenu, "Administração");

            var btnUsuarios = CreateMenuButton("Usuários", "", Color.FromArgb(233, 30, 99));
            btnUsuarios.Click += (_, _) => OpenForm<UsuariosForm>();
            flowMenu.Controls.Add(btnUsuarios);

            var btnFarmaceuticos = CreateMenuButton("Farmacêuticos", "", Color.FromArgb(233, 30, 99));
            btnFarmaceuticos.Click += (_, _) => OpenForm<FarmaceuticosForm>();
            flowMenu.Controls.Add(btnFarmaceuticos);
        }

        AddMenuSeparator(flowMenu, "");

        var btnLogout = CreateMenuButton("Sair", "Fazer logout", Color.FromArgb(244, 67, 54));
        btnLogout.Click += BtnLogout_Click;
        flowMenu.Controls.Add(btnLogout);

        pnlMenu.Controls.Add(flowMenu, 0, 1);
        split.Panel1.Controls.Add(pnlMenu);

        Load += (_, _) =>
        {
            OpenForm<PosForm>();

            BeginInvoke(() =>
            {
                flowMenu.PerformLayout();
                var maxWidth = flowMenu.Controls
                    .OfType<Button>()
                    .Select(b => b.PreferredSize.Width)
                    .DefaultIfEmpty(200)
                    .Max() + 20;
                split.SplitterDistance = Math.Max(160, maxWidth);
                split.Panel1MinSize = split.SplitterDistance;
            });
        };

        pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(230, 230, 235)
        };
        split.Panel2.Controls.Add(pnlContent);

        Controls.Add(split);
    }

    private static Button CreateMenuButton(string text, string tooltip)
    {
        return CreateMenuButton(text, tooltip, Color.FromArgb(92, 92, 99));
    }

    private static Button CreateMenuButton(string text, string tooltip, Color accent)
    {
        var btn = new Button
        {
            Text = text,
            Width = 200,
            Height = 38,
            FlatStyle = FlatStyle.Flat,
            FlatAppearance = { BorderSize = 0 },
            BackColor = Color.FromArgb(45, 45, 48),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            Cursor = Cursors.Hand,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Image = CreateIcon(accent),
            ImageAlign = ContentAlignment.MiddleLeft
        };
        if (!string.IsNullOrEmpty(tooltip))
            new ToolTip().SetToolTip(btn, tooltip);
        btn.MouseEnter += (_, _) => btn.BackColor = Color.FromArgb(70, 70, 75);
        btn.MouseLeave += (_, _) => btn.BackColor = Color.FromArgb(45, 45, 48);
        return btn;
    }

    private static Image CreateIcon(Color color)
    {
        var bmp = new Bitmap(16, 16);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(color);
        g.FillEllipse(brush, 3, 3, 10, 10);
        return bmp;
    }

    private static void AddMenuSeparator(FlowLayoutPanel parent, string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            parent.Controls.Add(new Label
            {
                Text = title,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Width = 200,
                Height = 20,
                Padding = new Padding(10, 5, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            });
        }
        parent.Controls.Add(new Label { Width = 200, Height = 3 });
    }

    private void OpenForm<T>() where T : Form, new()
    {
        pnlContent.Controls.Clear();

        var form = new T
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill
        };

        pnlContent.Controls.Add(form);
        form.Show();
    }

    private async void BtnLogout_Click(object? sender, EventArgs e)
    {
        await ApiClient.Instance.LogoutAsync();

        var loginForm = new LoginForm();
        loginForm.FormClosed += (_, _) => System.Windows.Forms.Application.Exit();
        loginForm.Show();

        Close();
    }
}
