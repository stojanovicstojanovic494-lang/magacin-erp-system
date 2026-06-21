namespace MagacinERP.WinForms.Forms;

public class FrmGlavna : Form
{
    private readonly string _korisnik;
    private readonly string _uloga;

    private MenuStrip menuStrip = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblStatusKorisnik = null!;
    private ToolStripStatusLabel lblStatusDatum = null!;
    private Panel pnlContent = null!;

    public FrmGlavna(string korisnik, string uloga)
    {
        _korisnik = korisnik;
        _uloga = uloga;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = $"MAGACIN ERP SISTEM - [{_korisnik}] ({_uloga})";
        this.Size = new System.Drawing.Size(1024, 768);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;
        this.IsMdiContainer = true;

        // ============ MENI ============
        menuStrip = new MenuStrip();
        menuStrip.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
        menuStrip.ForeColor = System.Drawing.Color.White;

        // --- Master podaci ---
        var mnuMasterPodaci = new ToolStripMenuItem("Master podaci");
        mnuMasterPodaci.ForeColor = System.Drawing.Color.White;
        mnuMasterPodaci.DropDownItems.Add("Artikli", null, (s, e) => OtvoriFormu<FrmArtikli>());
        mnuMasterPodaci.DropDownItems.Add("-");
        mnuMasterPodaci.DropDownItems.Add("Pregled zaliha", null, (s, e) => OtvoriFormu<FrmZalihe>());

        // --- Transakcije ---
        var mnuTransakcije = new ToolStripMenuItem("Transakcije");
        mnuTransakcije.ForeColor = System.Drawing.Color.White;
        mnuTransakcije.DropDownItems.Add("Prijema materijala", null, (s, e) => OtvoriFormuParam(() => new FrmPrijema(_korisnik)));
        mnuTransakcije.DropDownItems.Add("Izdavanje materijala", null, (s, e) => OtvoriFormuParam(() => new FrmIzdavanje(_korisnik)));
        mnuTransakcije.DropDownItems.Add("-");
        mnuTransakcije.DropDownItems.Add("Transfer materijala", null, (s, e) => OtvoriFormuParam(() => new FrmTransfer(_korisnik)));

        // --- Inventura ---
        var mnuInventura = new ToolStripMenuItem("Inventura");
        mnuInventura.ForeColor = System.Drawing.Color.White;
        mnuInventura.DropDownItems.Add("Novo prebrojavanje", null, (s, e) => OtvoriFormuParam(() => new FrmInventura(_korisnik)));

        // --- Administracija ---
        var mnuAdministracija = new ToolStripMenuItem("Administracija");
        mnuAdministracija.ForeColor = System.Drawing.Color.White;
        mnuAdministracija.DropDownItems.Add("Korisnici", null, (s, e) => OtvoriFormu<FrmKorisnici>());
        if (_uloga != "Admin")
            mnuAdministracija.Enabled = false;

        // --- Izlaz ---
        var mnuIzlaz = new ToolStripMenuItem("Izlaz");
        mnuIzlaz.ForeColor = System.Drawing.Color.White;
        mnuIzlaz.Click += (s, e) =>
        {
            var result = MessageBox.Show("Da li želite da napustite aplikaciju?", "Potvrda izlaza",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                this.Close();
        };

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuMasterPodaci, mnuTransakcije, mnuInventura, mnuAdministracija, mnuIzlaz });

        // ============ STATUS BAR ============
        statusStrip = new StatusStrip();
        lblStatusKorisnik = new ToolStripStatusLabel($"Korisnik: {_korisnik} | Uloga: {_uloga}");
        lblStatusDatum = new ToolStripStatusLabel($"{DateTime.Now:dd.MM.yyyy HH:mm}") { Alignment = ToolStripItemAlignment.Right };
        statusStrip.Items.Add(lblStatusKorisnik);
        statusStrip.Items.Add(new ToolStripStatusLabel { Spring = true });
        statusStrip.Items.Add(lblStatusDatum);

        // ============ SADRŽAJ ============
        pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(240, 240, 245) };

        var lblDobrodoslica = new Label
        {
            Text = $"Dobrodošli u MAGACIN ERP Sistem\n\n" +
                   $"Korisnik: {_korisnik}\n" +
                   $"Uloga: {_uloga}\n" +
                   $"Datum: {DateTime.Now:dd.MM.yyyy}\n\n" +
                   "Izaberite modul iz menija za rad.",
            Font = new System.Drawing.Font("Segoe UI", 14),
            AutoSize = true,
            Location = new System.Drawing.Point(50, 50)
        };
        pnlContent.Controls.Add(lblDobrodoslica);

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(pnlContent);
        this.Controls.Add(statusStrip);
        this.Controls.Add(menuStrip);

        // Timer za sat
        var timer = new System.Windows.Forms.Timer { Interval = 60000 };
        timer.Tick += (s, e) => lblStatusDatum.Text = $"{DateTime.Now:dd.MM.yyyy HH:mm}";
        timer.Start();
    }

    private void OtvoriFormu<T>() where T : Form, new()
    {
        var forma = new T();
        forma.MdiParent = this;
        forma.Show();
        forma.BringToFront();
    }

    private void OtvoriFormuParam(Func<Form> factory)
    {
        var forma = factory();
        forma.MdiParent = this;
        forma.Show();
        forma.BringToFront();
    }
}
