namespace MagacinERP.WinForms.Forms;

public partial class FrmGlavna : Form
{
    private readonly string _korisnik;
    private readonly string _uloga;

    private MenuStrip menuStrip = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblStatus = null!;
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

        // Menu Strip
        menuStrip = new MenuStrip();

        // Master podaci
        var mnuMasterPodaci = new ToolStripMenuItem("Master podaci");
        mnuMasterPodaci.DropDownItems.Add("Artikli", null, (s, e) => OtvoriFormu(new FrmArtikli()));
        mnuMasterPodaci.DropDownItems.Add("Dobavljači", null, (s, e) => MessageBox.Show("Modul u izradi.", "Info"));
        mnuMasterPodaci.DropDownItems.Add("Lokacije", null, (s, e) => MessageBox.Show("Modul u izradi.", "Info"));

        // Transakcije
        var mnuTransakcije = new ToolStripMenuItem("Transakcije");
        mnuTransakcije.DropDownItems.Add("Prijema materijala", null, (s, e) => OtvoriFormu(new FrmPrijema(_korisnik)));
        mnuTransakcije.DropDownItems.Add("Izdavanje materijala", null, (s, e) => OtvoriFormu(new FrmIzdavanje(_korisnik)));
        mnuTransakcije.DropDownItems.Add("Transfer", null, (s, e) => OtvoriFormu(new FrmTransfer(_korisnik)));

        // Inventura
        var mnuInventura = new ToolStripMenuItem("Inventura");
        mnuInventura.DropDownItems.Add("Prebrojavanje", null, (s, e) => OtvoriFormu(new FrmInventura(_korisnik)));

        // Administracija
        var mnuAdministracija = new ToolStripMenuItem("Administracija");
        mnuAdministracija.DropDownItems.Add("Korisnici", null, (s, e) => OtvoriFormu(new FrmKorisnici()));
        if (_uloga != "Admin")
            mnuAdministracija.Enabled = false;

        // Izlaz
        var mnuIzlaz = new ToolStripMenuItem("Izlaz");
        mnuIzlaz.Click += (s, e) => this.Close();

        menuStrip.Items.AddRange(new ToolStripItem[] { mnuMasterPodaci, mnuTransakcije, mnuInventura, mnuAdministracija, mnuIzlaz });

        // Status Strip
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel($"Korisnik: {_korisnik} | Uloga: {_uloga} | {DateTime.Now:dd.MM.yyyy}");
        statusStrip.Items.Add(lblStatus);

        // Content Panel
        pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = System.Drawing.Color.WhiteSmoke
        };

        var lblDobrodoslica = new Label
        {
            Text = $"Dobrodošli u MAGACIN ERP Sistem\n\nKorisnik: {_korisnik}\nUloga: {_uloga}\n\nIzaberite modul iz menija.",
            Font = new System.Drawing.Font("Segoe UI", 14),
            AutoSize = true,
            Location = new System.Drawing.Point(50, 50)
        };
        pnlContent.Controls.Add(lblDobrodoslica);

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(pnlContent);
        this.Controls.Add(statusStrip);
        this.Controls.Add(menuStrip);
    }

    private void OtvoriFormu(Form forma)
    {
        forma.MdiParent = this;
        forma.Show();
    }
}
