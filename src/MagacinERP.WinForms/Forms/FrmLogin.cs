using MagacinERP.Services;

namespace MagacinERP.WinForms.Forms;

public class FrmLogin : Form
{
    private TextBox txtKorisnickoIme = null!;
    private TextBox txtLozinka = null!;
    private TextBox txtServer = null!;
    private TextBox txtBaza = null!;
    private Button btnPrijava = null!;
    private Button btnIzlaz = null!;
    private Label lblNaslov = null!;
    private Label lblKorisnickoIme = null!;
    private Label lblLozinka = null!;
    private Label lblServer = null!;
    private Label lblBaza = null!;
    private Label lblGreska = null!;
    private GroupBox grpKonekcija = null!;
    private GroupBox grpPrijava = null!;

    public FrmLogin()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "MAGACIN ERP - Prijava na sistem";
        this.Size = new System.Drawing.Size(450, 420);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Naslov
        lblNaslov = new Label
        {
            Text = "MAGACIN ERP SISTEM",
            Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
            ForeColor = System.Drawing.Color.DarkBlue,
            AutoSize = true,
            Location = new System.Drawing.Point(75, 15)
        };
        this.Controls.Add(lblNaslov);

        // Grupa: Konekcija na bazu
        grpKonekcija = new GroupBox
        {
            Text = "Konekcija na bazu podataka",
            Location = new System.Drawing.Point(20, 55),
            Size = new System.Drawing.Size(400, 100)
        };

        lblServer = new Label { Text = "Server:", Location = new System.Drawing.Point(15, 30), AutoSize = true };
        txtServer = new TextBox { Location = new System.Drawing.Point(110, 27), Size = new System.Drawing.Size(270, 23), Text = "localhost" };
        lblBaza = new Label { Text = "Baza podataka:", Location = new System.Drawing.Point(15, 60), AutoSize = true };
        txtBaza = new TextBox { Location = new System.Drawing.Point(110, 57), Size = new System.Drawing.Size(270, 23), Text = "MagacinERP" };

        grpKonekcija.Controls.AddRange(new Control[] { lblServer, txtServer, lblBaza, txtBaza });
        this.Controls.Add(grpKonekcija);

        // Grupa: Prijava
        grpPrijava = new GroupBox
        {
            Text = "Korisnički podaci",
            Location = new System.Drawing.Point(20, 165),
            Size = new System.Drawing.Size(400, 110)
        };

        lblKorisnickoIme = new Label { Text = "Korisničko ime:", Location = new System.Drawing.Point(15, 30), AutoSize = true };
        txtKorisnickoIme = new TextBox { Location = new System.Drawing.Point(130, 27), Size = new System.Drawing.Size(250, 23) };
        lblLozinka = new Label { Text = "Lozinka:", Location = new System.Drawing.Point(15, 65), AutoSize = true };
        txtLozinka = new TextBox { Location = new System.Drawing.Point(130, 62), Size = new System.Drawing.Size(250, 23), PasswordChar = '*' };

        grpPrijava.Controls.AddRange(new Control[] { lblKorisnickoIme, txtKorisnickoIme, lblLozinka, txtLozinka });
        this.Controls.Add(grpPrijava);

        // Dugmad
        btnPrijava = new Button
        {
            Text = "Prijavi se",
            Location = new System.Drawing.Point(120, 295),
            Size = new System.Drawing.Size(100, 35),
            BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
            ForeColor = System.Drawing.Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnPrijava.Click += BtnPrijava_Click;
        this.Controls.Add(btnPrijava);

        btnIzlaz = new Button
        {
            Text = "Izlaz",
            Location = new System.Drawing.Point(230, 295),
            Size = new System.Drawing.Size(100, 35),
            FlatStyle = FlatStyle.Flat
        };
        btnIzlaz.Click += (s, e) => Application.Exit();
        this.Controls.Add(btnIzlaz);

        // Greška
        lblGreska = new Label
        {
            Text = "",
            ForeColor = System.Drawing.Color.Red,
            Location = new System.Drawing.Point(20, 345),
            Size = new System.Drawing.Size(400, 35),
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };
        this.Controls.Add(lblGreska);

        this.AcceptButton = btnPrijava;
        this.CancelButton = btnIzlaz;
    }

    private void BtnPrijava_Click(object? sender, EventArgs e)
    {
        lblGreska.Text = "";

        string korisnickoIme = txtKorisnickoIme.Text.Trim();
        string lozinka = txtLozinka.Text;
        string server = txtServer.Text.Trim();
        string baza = txtBaza.Text.Trim();

        if (string.IsNullOrWhiteSpace(korisnickoIme) || string.IsNullOrWhiteSpace(lozinka))
        {
            lblGreska.Text = "Unesite korisničko ime i lozinku.";
            return;
        }

        if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(baza))
        {
            lblGreska.Text = "Unesite server i naziv baze podataka.";
            return;
        }

        // Postavi connection string
        string connStr = $"Server={server};Database={baza};Trusted_Connection=True;TrustServerCertificate=True;";
        AppContext.SetConnectionString(connStr);

        try
        {
            var korisnikService = AppContext.KorisnikService;
            var (korisnikId, uloga) = korisnikService.ValidacijaKorisnika(korisnickoIme, lozinka);

            if (korisnikId < 0)
            {
                lblGreska.Text = "Neispravno korisničko ime ili lozinka.";
                txtLozinka.Clear();
                txtLozinka.Focus();
                return;
            }

            // Uspešna prijava
            AppContext.TrenutniKorisnik = korisnickoIme;
            AppContext.TrenutnaUloga = uloga;

            this.Hide();
            var glavna = new FrmGlavna(korisnickoIme, uloga ?? "Pregled");
            glavna.FormClosed += (s, args) => this.Close();
            glavna.Show();
        }
        catch (Exception ex)
        {
            lblGreska.Text = $"Greška pri povezivanju: {ex.Message}";
        }
    }
}
