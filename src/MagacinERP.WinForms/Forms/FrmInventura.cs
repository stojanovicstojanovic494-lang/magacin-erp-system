namespace MagacinERP.WinForms.Forms;

public class FrmInventura : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbTipInventure = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private Label lblInventuraId = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnNovaInventura = null!;
    private Button btnSacuvaj = null!;
    private CheckBox chkPrimeniRazlike = null!;

    private ComboBox cmbLokacija = null!;
    private ComboBox cmbArtikal = null!;
    private TextBox txtUtvrdjenaKolicina = null!;

    private int? _inventuraId = null;

    public FrmInventura(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
        UcitajLokacije();
        UcitajArtikle();
    }

    private void InitializeComponent()
    {
        this.Text = "Inventura - Prebrojavanje";
        this.Size = new System.Drawing.Size(930, 680);

        int y = 10;

        this.Controls.Add(new Label { Text = "INVENTURA (PREBROJAVANJE)", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        lblInventuraId = new Label { Text = "", Location = new System.Drawing.Point(700, y + 5), AutoSize = true, ForeColor = System.Drawing.Color.Gray };
        this.Controls.Add(lblInventuraId);
        y += 35;

        // Zaglavlje
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(180, 23) };
        this.Controls.Add(txtBrojDokumenta);

        this.Controls.Add(new Label { Text = "Tip inventure:", Location = new System.Drawing.Point(330, y + 3), AutoSize = true });
        cmbTipInventure = new ComboBox { Location = new System.Drawing.Point(430, y), Size = new System.Drawing.Size(130, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipInventure.Items.AddRange(new object[] { "Parcijalna", "Potpuna" });
        this.Controls.Add(cmbTipInventure);

        lblStatus = new Label { Text = "Status: NOVA", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(600, y + 3), AutoSize = true };
        this.Controls.Add(lblStatus);
        y += 30;

        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(430, 23) };
        this.Controls.Add(txtNapomena);

        btnSacuvaj = new Button { Text = "Sačuvaj zaglavlje", Location = new System.Drawing.Point(600, y - 2), Size = new System.Drawing.Size(130, 27), BackColor = System.Drawing.Color.FromArgb(0, 120, 215), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        this.Controls.Add(btnSacuvaj);
        y += 40;

        // Stavke
        this.Controls.Add(new Label { Text = "── Stavke inventure ──────────────────────────────────", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 22;

        this.Controls.Add(new Label { Text = "Lokacija:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        cmbLokacija = new ComboBox { Location = new System.Drawing.Point(75, y), Size = new System.Drawing.Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbLokacija);

        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(285, y + 3), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(340, y), Size = new System.Drawing.Size(230, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Prebrojano:", Location = new System.Drawing.Point(580, y + 3), AutoSize = true });
        txtUtvrdjenaKolicina = new TextBox { Location = new System.Drawing.Point(655, y), Size = new System.Drawing.Size(70, 23) };
        this.Controls.Add(txtUtvrdjenaKolicina);

        btnDodajStavku = new Button { Text = "+Dodaj", Location = new System.Drawing.Point(735, y - 2), Size = new System.Drawing.Size(65, 27), BackColor = System.Drawing.Color.LightGreen };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);
        y += 32;

        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(890, 250),
            AllowUserToAddRows = false, ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White, RowHeadersVisible = false
        };
        dgvStavke.Columns.Add("LokacijaID", "Lokacija ID");
        dgvStavke.Columns.Add("Lokacija", "Lokacija");
        dgvStavke.Columns.Add("ArtikalID", "Artikal ID");
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Prebrojano", "Prebrojano");
        dgvStavke.Columns.Add("Sistemska", "Sistemska");
        dgvStavke.Columns.Add("Razlika", "Razlika");
        this.Controls.Add(dgvStavke);
        y += 260;

        chkPrimeniRazlike = new CheckBox { Text = "Primeni razlike na zalihe nakon završetka inventure", Location = new System.Drawing.Point(10, y), AutoSize = true, Checked = true };
        this.Controls.Add(chkPrimeniRazlike);
        y += 30;

        btnNovaInventura = new Button { Text = "Nova inventura", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(130, 35) };
        btnNovaInventura.Click += BtnNovaInventura_Click;
        this.Controls.Add(btnNovaInventura);

        btnZavrsi = new Button { Text = "Završi inventuru", Location = new System.Drawing.Point(150, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);
    }

    private void UcitajLokacije()
    {
        try
        {
            var lokacije = AppContext.LokacijaRepository.GetAll();
            foreach (var l in lokacije)
                cmbLokacija.Items.Add($"{l.LokacijaID} - Zona {l.ZonaID}, Red {l.Red}, Polica {l.Polica}");
        }
        catch { }
    }

    private void UcitajArtikle()
    {
        try
        {
            var artikli = AppContext.ArtikalRepository.GetActive();
            foreach (var a in artikli)
                cmbArtikal.Items.Add($"{a.ArtikalID} - {a.SifraArtikla} - {a.NazivArtikla}");
        }
        catch { }
    }

    private void BtnSacuvaj_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBrojDokumenta.Text))
        { MessageBox.Show("Unesite broj dokumenta.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbTipInventure.SelectedIndex < 0)
        { MessageBox.Show("Izaberite tip inventure.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            var svc = AppContext.InventuraService;
            int id = svc.DodajInventuru(txtBrojDokumenta.Text.Trim(), cmbTipInventure.Text, _korisnik, txtNapomena.Text.Trim());
            _inventuraId = id;
            lblInventuraId.Text = $"[Inventura ID: {id}]";
            lblStatus.Text = "Status: U TOKU"; lblStatus.ForeColor = System.Drawing.Color.DarkOrange;
            btnSacuvaj.Enabled = false;
            MessageBox.Show($"Inventura kreirana (ID: {id}). Dodajte stavke.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (!_inventuraId.HasValue)
        { MessageBox.Show("Prvo sačuvajte zaglavlje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbLokacija.SelectedIndex < 0 || cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtUtvrdjenaKolicina.Text))
        { MessageBox.Show("Popunite sva polja.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!int.TryParse(txtUtvrdjenaKolicina.Text, out int kol) || kol < 0)
        { MessageBox.Show("Količina mora biti nenegativan broj.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            int lokId = int.Parse(cmbLokacija.Text.Split('-')[0].Trim());
            int artId = int.Parse(cmbArtikal.Text.Split('-')[0].Trim());

            var svc = AppContext.InventuraService;
            svc.DodajStavku(_inventuraId.Value, lokId, artId, kol, _korisnik);

            // Provera sistemske baze za prikaz
            var zaliha = AppContext.ZaliheRepository.GetByArtikalAndLokacija(artId, lokId);
            int sistemska = zaliha?.Kolicina ?? 0;
            int razlika = kol - sistemska;

            dgvStavke.Rows.Add(lokId, cmbLokacija.Text, artId, cmbArtikal.Text, kol, sistemska, razlika);
            txtUtvrdjenaKolicina.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnNovaInventura_Click(object? sender, EventArgs e)
    {
        _inventuraId = null;
        txtBrojDokumenta.Clear(); cmbTipInventure.SelectedIndex = -1;
        txtNapomena.Clear(); dgvStavke.Rows.Clear();
        chkPrimeniRazlike.Checked = true;
        lblStatus.Text = "Status: NOVA"; lblStatus.ForeColor = System.Drawing.Color.Blue;
        lblInventuraId.Text = ""; btnSacuvaj.Enabled = true;
    }

    private void BtnZavrsi_Click(object? sender, EventArgs e)
    {
        if (!_inventuraId.HasValue) { MessageBox.Show("Nema aktivne inventure.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (dgvStavke.Rows.Count == 0) { MessageBox.Show("Dodajte bar jednu stavku.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        string msg = chkPrimeniRazlike.Checked
            ? "Inventura će biti završena i razlike će biti primenjene na zalihe.\nNastaviti?"
            : "Inventura će biti završena BEZ primene razlika.\nNastaviti?";

        var result = MessageBox.Show(msg, "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes) return;

        try
        {
            AppContext.InventuraService.ZavrsiInventuru(_inventuraId.Value, _korisnik, chkPrimeniRazlike.Checked);
            lblStatus.Text = "Status: ZAVRŠENA"; lblStatus.ForeColor = System.Drawing.Color.Green;

            string info = chkPrimeniRazlike.Checked
                ? "Inventura je završena. Razlike su primenjene na zalihe."
                : "Inventura je završena. Razlike NISU primenjene.";
            MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
