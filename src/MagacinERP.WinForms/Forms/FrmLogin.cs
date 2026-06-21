using MagacinERP.Core.Interfaces;
using MagacinERP.Services;

namespace MagacinERP.WinForms.Forms;

public partial class FrmLogin : Form
{
    private TextBox txtKorisnickoIme = null!;
    private TextBox txtLozinka = null!;
    private Button btnPrijava = null!;
    private Label lblNaslov = null!;
    private Label lblKorisnickoIme = null!;
    private Label lblLozinka = null!;
    private Label lblGreska = null!;

    public FrmLogin()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "MAGACIN ERP - Prijava";
        this.Size = new System.Drawing.Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        lblNaslov = new Label
        {
            Text = "MAGACIN ERP SISTEM",
            Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
            AutoSize = true,
            Location = new System.Drawing.Point(80, 20)
        };

        lblKorisnickoIme = new Label
        {
            Text = "Korisničko ime:",
            Location = new System.Drawing.Point(50, 80),
            AutoSize = true
        };

        txtKorisnickoIme = new TextBox
        {
            Location = new System.Drawing.Point(50, 100),
            Size = new System.Drawing.Size(280, 25)
        };

        lblLozinka = new Label
        {
            Text = "Lozinka:",
            Location = new System.Drawing.Point(50, 135),
            AutoSize = true
        };

        txtLozinka = new TextBox
        {
            Location = new System.Drawing.Point(50, 155),
            Size = new System.Drawing.Size(280, 25),
            PasswordChar = '*'
        };

        btnPrijava = new Button
        {
            Text = "Prijavi se",
            Location = new System.Drawing.Point(130, 200),
            Size = new System.Drawing.Size(120, 35)
        };
        btnPrijava.Click += BtnPrijava_Click;

        lblGreska = new Label
        {
            Text = "",
            ForeColor = System.Drawing.Color.Red,
            Location = new System.Drawing.Point(50, 245),
            AutoSize = true
        };

        this.Controls.AddRange(new Control[] { lblNaslov, lblKorisnickoIme, txtKorisnickoIme, lblLozinka, txtLozinka, btnPrijava, lblGreska });
        this.AcceptButton = btnPrijava;
    }

    private void BtnPrijava_Click(object? sender, EventArgs e)
    {
        string korisnickoIme = txtKorisnickoIme.Text.Trim();
        string lozinka = txtLozinka.Text;

        if (string.IsNullOrWhiteSpace(korisnickoIme) || string.IsNullOrWhiteSpace(lozinka))
        {
            lblGreska.Text = "Unesite korisničko ime i lozinku.";
            return;
        }

        // TODO: Replace with actual repository implementation
        // For now using demo validation
        if (korisnickoIme == "admin" && lozinka == "admin123")
        {
            this.Hide();
            var glavna = new FrmGlavna(korisnickoIme, "Admin");
            glavna.FormClosed += (s, args) => this.Close();
            glavna.Show();
        }
        else if (korisnickoIme == "magaciner" && lozinka == "magacin123")
        {
            this.Hide();
            var glavna = new FrmGlavna(korisnickoIme, "Magaciner");
            glavna.FormClosed += (s, args) => this.Close();
            glavna.Show();
        }
        else
        {
            lblGreska.Text = "Neispravno korisničko ime ili lozinka.";
            txtLozinka.Clear();
            txtLozinka.Focus();
        }
    }
}
