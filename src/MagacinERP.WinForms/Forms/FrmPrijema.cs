using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public class FrmPrijema : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbDobavljac = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private Label lblPrijemaId = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnOtkazi = null!;
    private Button btnNovaPrijema = null!;
    private Button btnSacuvaj = null!;

    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;
    private TextBox txtCena = null!;
    private TextBox txtLotBroj = null!;
    private DateTimePicker dtpRok = null!;

    private int? _prijemaId = null;

    public FrmPrijema(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
        UcitajArtikle();
    }

    private void InitializeComponent()
    {
        this.Text = "Prijema materijala (GRN)";
        this.Size = new System.Drawing.Size(950, 680);

        int y = 10;

        this.Controls.Add(new Label { Text = "PRIJEMA MATERIJALA", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        lblPrijemaId = new Label { Text = "", Location = new System.Drawing.Point(750, y + 5), AutoSize = true, ForeColor = System.Drawing.Color.Gray };
        this.Controls.Add(lblPrijemaId);
        y += 35;

        // Zaglavlje dokumenta
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(180, 23) };
        this.Controls.Add(txtBrojDokumenta);

        this.Controls.Add(new Label { Text = "Dobavljač:", Location = new System.Drawing.Point(330, y + 3), AutoSize = true });
        cmbDobavljac = new ComboBox { Location = new System.Drawing.Point(410, y), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDobavljac.Items.AddRange(new object[] { "1 - ELEKTRO d.o.o.", "2 - METAL Import", "3 - HEMIJA i hemikalije", "4 - PLASTIK pro", "5 - TEKSTIL Plus" });
        this.Controls.Add(cmbDobavljac);

        lblStatus = new Label { Text = "Status: NOVA", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(700, y + 3), AutoSize = true };
        this.Controls.Add(lblStatus);
        y += 30;

        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(530, 23) };
        this.Controls.Add(txtNapomena);

        btnSacuvaj = new Button { Text = "Sačuvaj zaglavlje", Location = new System.Drawing.Point(700, y - 2), Size = new System.Drawing.Size(130, 27), BackColor = System.Drawing.Color.FromArgb(0, 120, 215), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        this.Controls.Add(btnSacuvaj);
        y += 40;

        // Separator
        this.Controls.Add(new Label { Text = "── Stavke prijeme ──────────────────────────────────────────", Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 22;

        // Stavka input
        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y + 3), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(65, y), Size = new System.Drawing.Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Kol:", Location = new System.Drawing.Point(275, y + 3), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(305, y), Size = new System.Drawing.Size(60, 23) };
        this.Controls.Add(txtKolicina);

        this.Controls.Add(new Label { Text = "Cena:", Location = new System.Drawing.Point(375, y + 3), AutoSize = true });
        txtCena = new TextBox { Location = new System.Drawing.Point(415, y), Size = new System.Drawing.Size(70, 23) };
        this.Controls.Add(txtCena);

        this.Controls.Add(new Label { Text = "LOT:", Location = new System.Drawing.Point(495, y + 3), AutoSize = true });
        txtLotBroj = new TextBox { Location = new System.Drawing.Point(530, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtLotBroj);

        this.Controls.Add(new Label { Text = "Rok:", Location = new System.Drawing.Point(620, y + 3), AutoSize = true });
        dtpRok = new DateTimePicker { Location = new System.Drawing.Point(650, y), Size = new System.Drawing.Size(130, 23), Format = DateTimePickerFormat.Short, Checked = false, ShowCheckBox = true };
        this.Controls.Add(dtpRok);

        btnDodajStavku = new Button { Text = "+Dodaj", Location = new System.Drawing.Point(790, y - 2), Size = new System.Drawing.Size(65, 27), BackColor = System.Drawing.Color.LightGreen };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(860, y - 2), Size = new System.Drawing.Size(30, 27), BackColor = System.Drawing.Color.LightCoral };
        btnObrisiStavku.Click += BtnObrisiStavku_Click;
        this.Controls.Add(btnObrisiStavku);
        y += 32;

        // Grid
        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(910, 270),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            RowHeadersVisible = false
        };
        dgvStavke.Columns.Add("ArtikalID", "Artikal ID");
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina naručena");
        dgvStavke.Columns.Add("Cena", "Cena");
        dgvStavke.Columns.Add("Ukupno", "Ukupno");
        dgvStavke.Columns.Add("LOT", "LOT");
        dgvStavke.Columns.Add("Rok", "Rok upotrebe");
        this.Controls.Add(dgvStavke);
        y += 280;

        // Dugmad
        btnNovaPrijema = new Button { Text = "Nova prijema", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(120, 35) };
        btnNovaPrijema.Click += BtnNovaPrijema_Click;
        this.Controls.Add(btnNovaPrijema);

        btnZavrsi = new Button { Text = "Završi prijemu", Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);

        btnOtkazi = new Button { Text = "Otkaži prijemu", Location = new System.Drawing.Point(280, y), Size = new System.Drawing.Size(120, 35), BackColor = System.Drawing.Color.LightCoral };
        btnOtkazi.Click += BtnOtkazi_Click;
        this.Controls.Add(btnOtkazi);
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
        {
            MessageBox.Show("Unesite broj dokumenta.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbDobavljac.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite dobavljača.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            int dobavljacId = cmbDobavljac.SelectedIndex + 1;
            var prijemaService = AppContext.PrijemaService;
            int id = prijemaService.DodajPrijemu(txtBrojDokumenta.Text.Trim(), dobavljacId, _korisnik, txtNapomena.Text.Trim());
            _prijemaId = id;
            lblPrijemaId.Text = $"[Prijema ID: {id}]";
            lblStatus.Text = "Status: OTVORENA";
            lblStatus.ForeColor = System.Drawing.Color.DarkOrange;

            btnSacuvaj.Enabled = false;
            MessageBox.Show($"Prijema kreirana (ID: {id}). Dodajte stavke.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (!_prijemaId.HasValue)
        {
            MessageBox.Show("Prvo sačuvajte zaglavlje prijeme.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtKolicina.Text) || string.IsNullOrWhiteSpace(txtCena.Text))
        {
            MessageBox.Show("Izaberite artikal, unesite količinu i cenu.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!int.TryParse(txtKolicina.Text, out int kol) || kol <= 0)
        {
            MessageBox.Show("Količina mora biti pozitivan ceo broj.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!decimal.TryParse(txtCena.Text, out decimal cena) || cena < 0)
        {
            MessageBox.Show("Cena mora biti validna.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            string artikalText = cmbArtikal.Text;
            int artikalId = int.Parse(artikalText.Split('-')[0].Trim());
            string? lot = string.IsNullOrWhiteSpace(txtLotBroj.Text) ? null : txtLotBroj.Text.Trim();
            DateTime? rok = dtpRok.Checked ? dtpRok.Value : null;

            var prijemaService = AppContext.PrijemaService;
            prijemaService.DodajStavku(_prijemaId.Value, artikalId, kol, cena, _korisnik, lot, rok);

            dgvStavke.Rows.Add(artikalId, artikalText, kol, cena.ToString("F2"), (kol * cena).ToString("F2"), lot ?? "", rok?.ToString("dd.MM.yyyy") ?? "");

            txtKolicina.Clear();
            txtCena.Clear();
            txtLotBroj.Clear();
            dtpRok.Checked = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri dodavanju stavke:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnObrisiStavku_Click(object? sender, EventArgs e)
    {
        if (dgvStavke.CurrentRow != null)
            dgvStavke.Rows.Remove(dgvStavke.CurrentRow);
    }

    private void BtnNovaPrijema_Click(object? sender, EventArgs e)
    {
        _prijemaId = null;
        txtBrojDokumenta.Clear();
        cmbDobavljac.SelectedIndex = -1;
        txtNapomena.Clear();
        dgvStavke.Rows.Clear();
        lblStatus.Text = "Status: NOVA";
        lblStatus.ForeColor = System.Drawing.Color.Blue;
        lblPrijemaId.Text = "";
        btnSacuvaj.Enabled = true;
    }

    private void BtnZavrsi_Click(object? sender, EventArgs e)
    {
        if (!_prijemaId.HasValue)
        {
            MessageBox.Show("Nema aktivne prijeme za završavanje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (dgvStavke.Rows.Count == 0)
        {
            MessageBox.Show("Dodajte bar jednu stavku.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var prijemaService = AppContext.PrijemaService;
            prijemaService.ZavrsiPrijemu(_prijemaId.Value, _korisnik);

            lblStatus.Text = "Status: ZAVRŠENA";
            lblStatus.ForeColor = System.Drawing.Color.Green;
            MessageBox.Show("Prijema je uspešno završena.\nZalihe su ažurirane.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnOtkazi_Click(object? sender, EventArgs e)
    {
        if (!_prijemaId.HasValue) return;

        var result = MessageBox.Show("Da li ste sigurni da želite da otkažete prijemu?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            try
            {
                var prijemaService = AppContext.PrijemaService;
                prijemaService.OtkaziPrijemu(_prijemaId.Value, _korisnik);

                lblStatus.Text = "Status: OTKAZANA";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
