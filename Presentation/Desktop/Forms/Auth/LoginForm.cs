using System.Windows.Forms;
using minipdv.Presentation.Desktop.Forms.Shared;

namespace minipdv.Presentation.Desktop.Forms.Auth;

public class LoginForm : Form
{
    private readonly TextBox txtLogin;
    private readonly TextBox txtPassword;
    private readonly Button btnEntrar;
    private readonly Label lblError;
    private readonly Label lblTitle;

    public LoginForm()
    {
        Text = "MiniPDV - Login";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(380, 340);

        lblTitle = new Label
        {
            Text = "MiniPDV",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top,
            Height = 50,
            ForeColor = Color.DarkBlue
        };

        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(30, 10, 30, 15)
        };
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

        tbl.Controls.Add(new Label { Text = "Usuário:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        txtLogin = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) };
        tbl.Controls.Add(txtLogin, 0, 1);

        tbl.Controls.Add(new Label { Text = "Senha:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        txtPassword = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), UseSystemPasswordChar = true };
        tbl.Controls.Add(txtPassword, 0, 3);

        btnEntrar = new Button
        {
            Text = "Entrar",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = Color.DarkBlue,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnEntrar.Click += BtnEntrar_Click;
        tbl.Controls.Add(btnEntrar, 0, 4);

        lblError = new Label
        {
            Text = "",
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };
        tbl.Controls.Add(lblError, 0, 5);

        Controls.Add(lblTitle);
        Controls.Add(tbl);

        AcceptButton = btnEntrar;
        txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnEntrar_Click(s, e); };
        txtLogin.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
    }

    private async void BtnEntrar_Click(object? sender, EventArgs e)
    {
        var login = txtLogin.Text.Trim();
        var password = txtPassword.Text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Preencha usuário e senha.";
            return;
        }

        btnEntrar.Enabled = false;
        lblError.Text = "Autenticando...";
        lblError.ForeColor = Color.DarkGray;

        var error = await ApiClient.Instance.LoginAsync(login, password);

        if (error != null)
        {
            lblError.Text = error;
            lblError.ForeColor = Color.Red;
            btnEntrar.Enabled = true;
            return;
        }

        Hide();

        var mainForm = new MainForm();
        mainForm.FormClosed += (_, _) => System.Windows.Forms.Application.Exit();
        mainForm.Show();
    }
}
