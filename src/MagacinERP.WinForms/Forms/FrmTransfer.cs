namespace MagacinERP.WinForms.Forms;

public class FrmTransfer : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbIzLokacije = null!;
    private ComboBox cmbULokaciju = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private Label lblTransferId = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnIzvrsi = null!;
    private Button btnNoviTransfer = null!;
    private Button btnSacuvaj = null!;

    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;

    private int? _transferId = null;

    public FrmTransfer(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
        UcitajLokacije();
        UcitajArtikle();
    }

    private void InitializeComponent()
    {
        this.Text = "Transfer materijala";
        this.Size = new System.Drawing.Size(900, 600);

        int y = 10;

        this.Controls.Add(new Label { Text = "TRANSFER MATERIJALA", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        lblTransferId = new Label { Text = "", Location = new System.Drawing.Point(700, y + 5), AutoSize = true, ForeColor = System.Drawing.Color.Gray };
        this.Controls.Add(lblTransferId);
        y += 35;

        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(180, 23) };
        this.Controls.Add(txtBrojDokumenta);

        lblStatus = new Label { Text = "Status: NOVA", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(600, y + 3), AutoSize = true };
        this.Controls.Add(lblStatus);
        y += 30;

        this.Controls.Add(new Label { Text = "Iz lokacije:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        cmbIzLokacije = new ComboBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(280, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbIzLokacije);

        this.Controls.Add(new Label { Text = "U lokaciju:", Location = new System.Drawing.Point(430, y + 3), AutoSize = true });
        cmbULokaciju = new ComboBox { Location = new System.Drawing.Point(510, y), Size = new System.Drawing.Size(280, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbULokaciju);
        y += 30;

        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(430, 23) };
        this.Controls.Add(txtNapomena);

        btnSacuvaj = new Button { Text = "Sačuvaj zaglavlje", Location = new System.Drawing.Point(600, y - 2), Size = new System.Drawing.Size(130, 27), BackColor = System.Drawing.Color.FromArgb(0, 120, 215), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        this.Controls.Add(btnSacuvaj);
        y += 40;

        // Stavke
        this.Controls.Add(new Label { Text = "── Stavke transfera ──────────────────────────────────", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 22;

        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(65, y), Size = new System.Drawing.Size(350, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Količina:", Location = new System.Drawing.Point(425, y + 3), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(485, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtKolicina);

        btnDodajStavku = new Button { Text = "+Dodaj", Location = new System.Drawing.Point(575, y - 2), Size = new System.Drawing.Size(65, 27), BackColor = System.Drawing.Color.LightGreen };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);
        y += 32;

        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 200),
            AllowUserToAddRows = false, ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White, RowHeadersVisible = false
        };
        dgvStavke.Columns.Add("ArtikalID", "ID");
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina");
        this.Controls.Add(dgvStavke);
        y += 210;

        btnNoviTransfer = new Button { Text = "Novi transfer", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(120, 35) };
        btnNoviTransfer.Click += BtnNoviTransfer_Click;
        this.Controls.Add(btnNoviTransfer);

        btnIzvrsi = new Button { Text = "Izvrši transfer", Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnIzvrsi.Click += BtnIzvrsi_Click;
        this.Controls.Add(btnIzvrsi);
    }

    private void UcitajLokacije()
    {
        try
        {
            var lokacije = AppContext.LokacijaRepository.GetAll();
            foreach (var l in lokacije)
            {
                string item = $"{l.LokacijaID} - Zona {l.ZonaID}, Red {l.Red}, Polica {l.Polica}";
                cmbIzLokacije.Items.Add(item);
                cmbULokaciju.Items.Add(item);
            }
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
        if (cmbIzLokacije.SelectedIndex < 0 || cmbULokaciju.SelectedIndex < 0)
        { MessageBox.Show("Izaberite obe lokacije.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbIzLokacije.SelectedIndex == cmbULokaciju.SelectedIndex)
        { MessageBox.Show("Izlazna i ulazna lokacija moraju biti različite.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            int izLokId = int.Parse(cmbIzLokacije.Text.Split('-')[0].Trim());
            int uLokId = int.Parse(cmbULokaciju.Text.Split('-')[0].Trim());

            var svc = AppContext.TransferService;
            int id = svc.DodajTransfer(txtBrojDokumenta.Text.Trim(), izLokId, uLokId, _korisnik, txtNapomena.Text.Trim());
            _transferId = id;
            lblTransferId.Text = $"[Transfer ID: {id}]";
            lblStatus.Text = "Status: OTVOREN"; lblStatus.ForeColor = System.Drawing.Color.DarkOrange;
            btnSacuvaj.Enabled = false;
            MessageBox.Show($"Transfer kreiran (ID: {id}).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (!_transferId.HasValue)
        { MessageBox.Show("Prvo sačuvajte zaglavlje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtKolicina.Text))
        { MessageBox.Show("Izaberite artikal i unesite količinu.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!int.TryParse(txtKolicina.Text, out int kol) || kol <= 0)
        { MessageBox.Show("Količina mora biti pozitivna.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        int artikalId = int.Parse(cmbArtikal.Text.Split('-')[0].Trim());
        dgvStavke.Rows.Add(artikalId, cmbArtikal.Text, kol);
        txtKolicina.Clear();
    }

    private void BtnNoviTransfer_Click(object? sender, EventArgs e)
    {
        _transferId = null;
        txtBrojDokumenta.Clear(); cmbIzLokacije.SelectedIndex = -1;
        cmbULokaciju.SelectedIndex = -1; txtNapomena.Clear();
        dgvStavke.Rows.Clear(); lblStatus.Text = "Status: NOVA";
        lblStatus.ForeColor = System.Drawing.Color.Blue;
        lblTransferId.Text = ""; btnSacuvaj.Enabled = true;
    }

    private void BtnIzvrsi_Click(object? sender, EventArgs e)
    {
        if (!_transferId.HasValue) { MessageBox.Show("Nema aktivnog transfera.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (dgvStavke.Rows.Count == 0) { MessageBox.Show("Dodajte bar jednu stavku.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            AppContext.TransferService.IzvrsiTransfer(_transferId.Value, _korisnik);
            lblStatus.Text = "Status: IZVRŠEN"; lblStatus.ForeColor = System.Drawing.Color.Green;
            MessageBox.Show("Transfer je uspešno izvršen.\nZalihe su ažurirane.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
