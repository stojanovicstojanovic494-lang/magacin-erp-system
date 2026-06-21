using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public class FrmIzdavanje : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbTipIzdavanja = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private Label lblIzdavanjeId = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnNovoIzdavanje = null!;
    private Button btnSacuvaj = null!;

    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;
    private TextBox txtCena = null!;

    private int? _izdavanjeId = null;

    public FrmIzdavanje(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
        UcitajArtikle();
    }

    private void InitializeComponent()
    {
        this.Text = "Izdavanje materijala";
        this.Size = new System.Drawing.Size(900, 620);

        int y = 10;

        this.Controls.Add(new Label { Text = "IZDAVANJE MATERIJALA", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        lblIzdavanjeId = new Label { Text = "", Location = new System.Drawing.Point(700, y + 5), AutoSize = true, ForeColor = System.Drawing.Color.Gray };
        this.Controls.Add(lblIzdavanjeId);
        y += 35;

        // Zaglavlje
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(180, 23) };
        this.Controls.Add(txtBrojDokumenta);

        this.Controls.Add(new Label { Text = "Tip izdavanja:", Location = new System.Drawing.Point(330, y + 3), AutoSize = true });
        cmbTipIzdavanja = new ComboBox { Location = new System.Drawing.Point(430, y), Size = new System.Drawing.Size(130, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipIzdavanja.Items.AddRange(new object[] { "Proizvodnja", "Prodaja", "Povracaj", "Transfer" });
        this.Controls.Add(cmbTipIzdavanja);

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
        this.Controls.Add(new Label { Text = "── Stavke izdavanja ──────────────────────────────────", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 22;

        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(65, y), Size = new System.Drawing.Size(280, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Količina:", Location = new System.Drawing.Point(355, y + 3), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(415, y), Size = new System.Drawing.Size(70, 23) };
        this.Controls.Add(txtKolicina);

        this.Controls.Add(new Label { Text = "Cena:", Location = new System.Drawing.Point(495, y + 3), AutoSize = true });
        txtCena = new TextBox { Location = new System.Drawing.Point(535, y), Size = new System.Drawing.Size(70, 23) };
        this.Controls.Add(txtCena);

        btnDodajStavku = new Button { Text = "+Dodaj", Location = new System.Drawing.Point(620, y - 2), Size = new System.Drawing.Size(65, 27), BackColor = System.Drawing.Color.LightGreen };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(690, y - 2), Size = new System.Drawing.Size(30, 27), BackColor = System.Drawing.Color.LightCoral };
        btnObrisiStavku.Click += (s, ev) => { if (dgvStavke.CurrentRow != null) dgvStavke.Rows.Remove(dgvStavke.CurrentRow); };
        this.Controls.Add(btnObrisiStavku);
        y += 32;

        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 250),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            RowHeadersVisible = false
        };
        dgvStavke.Columns.Add("ArtikalID", "ID");
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina");
        dgvStavke.Columns.Add("Cena", "Cena");
        dgvStavke.Columns.Add("Ukupno", "Ukupno");
        this.Controls.Add(dgvStavke);
        y += 260;

        btnNovoIzdavanje = new Button { Text = "Novo izdavanje", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(120, 35) };
        btnNovoIzdavanje.Click += BtnNovoIzdavanje_Click;
        this.Controls.Add(btnNovoIzdavanje);

        btnZavrsi = new Button { Text = "Završi izdavanje", Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);
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
        if (cmbTipIzdavanja.SelectedIndex < 0)
        { MessageBox.Show("Izaberite tip izdavanja.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            var svc = AppContext.IzdavanjeService;
            int id = svc.DodajIzdavanje(txtBrojDokumenta.Text.Trim(), cmbTipIzdavanja.Text, _korisnik, txtNapomena.Text.Trim());
            _izdavanjeId = id;
            lblIzdavanjeId.Text = $"[Izdavanje ID: {id}]";
            lblStatus.Text = "Status: OTVORENA";
            lblStatus.ForeColor = System.Drawing.Color.DarkOrange;
            btnSacuvaj.Enabled = false;
            MessageBox.Show($"Izdavanje kreirano (ID: {id}).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (!_izdavanjeId.HasValue)
        { MessageBox.Show("Prvo sačuvajte zaglavlje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtKolicina.Text))
        { MessageBox.Show("Izaberite artikal i unesite količinu.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!int.TryParse(txtKolicina.Text, out int kol) || kol <= 0)
        { MessageBox.Show("Količina mora biti pozitivna.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        decimal? cena = decimal.TryParse(txtCena.Text, out var c) ? c : null;

        try
        {
            int artikalId = int.Parse(cmbArtikal.Text.Split('-')[0].Trim());
            var svc = AppContext.IzdavanjeService;
            svc.DodajStavku(_izdavanjeId.Value, artikalId, kol, _korisnik, cena, null);

            dgvStavke.Rows.Add(artikalId, cmbArtikal.Text, kol, cena?.ToString("F2") ?? "0.00", (kol * (cena ?? 0)).ToString("F2"));
            txtKolicina.Clear();
            txtCena.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnNovoIzdavanje_Click(object? sender, EventArgs e)
    {
        _izdavanjeId = null;
        txtBrojDokumenta.Clear(); cmbTipIzdavanja.SelectedIndex = -1;
        txtNapomena.Clear(); dgvStavke.Rows.Clear();
        lblStatus.Text = "Status: NOVA"; lblStatus.ForeColor = System.Drawing.Color.Blue;
        lblIzdavanjeId.Text = ""; btnSacuvaj.Enabled = true;
    }

    private void BtnZavrsi_Click(object? sender, EventArgs e)
    {
        if (!_izdavanjeId.HasValue) { MessageBox.Show("Nema aktivnog izdavanja.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (dgvStavke.Rows.Count == 0) { MessageBox.Show("Dodajte bar jednu stavku.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            AppContext.IzdavanjeService.ZavrsiIzdavanje(_izdavanjeId.Value, _korisnik);
            lblStatus.Text = "Status: ZAVRŠENA"; lblStatus.ForeColor = System.Drawing.Color.Green;
            MessageBox.Show("Izdavanje je uspešno završeno.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
