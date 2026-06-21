using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public class FrmArtikli : Form
{
    private DataGridView dgvArtikli = null!;
    private Panel pnlDetalji = null!;
    private TextBox txtSifra = null!;
    private TextBox txtNaziv = null!;
    private TextBox txtCenaKupovine = null!;
    private TextBox txtCenaProdaje = null!;
    private TextBox txtMinStalje = null!;
    private TextBox txtMaxStalje = null!;
    private TextBox txtBarkod = null!;
    private TextBox txtOpis = null!;
    private TextBox txtTezina = null!;
    private TextBox txtZapremina = null!;
    private ComboBox cmbKategorija = null!;
    private ComboBox cmbJedinicaMere = null!;
    private CheckBox chkAktivan = null!;
    private Button btnNovi = null!;
    private Button btnSacuvaj = null!;
    private Button btnObrisi = null!;
    private Button btnOsvezi = null!;
    private Label lblUkupno = null!;

    private int? _selektovaniId = null;

    public FrmArtikli()
    {
        InitializeComponent();
        UcitajPodatke();
    }

    private void InitializeComponent()
    {
        this.Text = "Artikli - Master podaci";
        this.Size = new System.Drawing.Size(950, 650);
        this.StartPosition = FormStartPosition.CenterParent;

        // Header
        var lblHeader = new Label
        {
            Text = "ARTIKLI - Šifarnik",
            Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(10, 5),
            AutoSize = true
        };
        this.Controls.Add(lblHeader);

        lblUkupno = new Label { Text = "Ukupno: 0", Location = new System.Drawing.Point(750, 10), AutoSize = true };
        this.Controls.Add(lblUkupno);

        // DataGridView
        dgvArtikli = new DataGridView
        {
            Location = new System.Drawing.Point(10, 35),
            Size = new System.Drawing.Size(910, 250),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            RowHeadersVisible = false
        };
        dgvArtikli.SelectionChanged += DgvArtikli_SelectionChanged;
        this.Controls.Add(dgvArtikli);

        // Panel za detalje
        pnlDetalji = new Panel
        {
            Location = new System.Drawing.Point(10, 295),
            Size = new System.Drawing.Size(910, 270),
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(pnlDetalji);

        var lblDetalji = new Label
        {
            Text = "Detalji artikla:",
            Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(10, 5),
            AutoSize = true
        };
        pnlDetalji.Controls.Add(lblDetalji);

        int y = 30;
        int col2 = 470;

        // Red 1: Šifra, Naziv
        AddLabel(pnlDetalji, "Šifra artikla:", 10, y);
        txtSifra = AddTextBox(pnlDetalji, 130, y, 150);
        AddLabel(pnlDetalji, "Naziv artikla:", col2, y);
        txtNaziv = AddTextBox(pnlDetalji, col2 + 120, y, 290);

        y += 30;

        // Red 2: Kategorija, Jedinica mere
        AddLabel(pnlDetalji, "Kategorija:", 10, y);
        cmbKategorija = new ComboBox { Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbKategorija.Items.AddRange(new object[] { "1 - Elektrotehnika", "2 - Mehanika", "3 - Hemikalije", "4 - Tekstil", "5 - Metali", "6 - Plastika", "7 - Ambalaža", "8 - Razno" });
        pnlDetalji.Controls.Add(cmbKategorija);

        AddLabel(pnlDetalji, "Jedinica mere:", col2, y);
        cmbJedinicaMere = new ComboBox { Location = new System.Drawing.Point(col2 + 120, y), Size = new System.Drawing.Size(120, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbJedinicaMere.Items.AddRange(new object[] { "1 - kom", "2 - kg", "3 - l", "4 - m", "5 - pak", "6 - pal", "7 - kut" });
        pnlDetalji.Controls.Add(cmbJedinicaMere);

        y += 30;

        // Red 3: Cene
        AddLabel(pnlDetalji, "Cena kupovine:", 10, y);
        txtCenaKupovine = AddTextBox(pnlDetalji, 130, y, 100);
        AddLabel(pnlDetalji, "Cena prodaje:", 260, y);
        txtCenaProdaje = AddTextBox(pnlDetalji, 370, y, 100);
        AddLabel(pnlDetalji, "Barkod:", col2, y);
        txtBarkod = AddTextBox(pnlDetalji, col2 + 120, y, 200);

        y += 30;

        // Red 4: Min/Max stalje, Težina, Zapremina
        AddLabel(pnlDetalji, "Min. stalje:", 10, y);
        txtMinStalje = AddTextBox(pnlDetalji, 130, y, 70);
        AddLabel(pnlDetalji, "Max. stalje:", 220, y);
        txtMaxStalje = AddTextBox(pnlDetalji, 310, y, 70);
        AddLabel(pnlDetalji, "Težina (kg):", col2, y);
        txtTezina = AddTextBox(pnlDetalji, col2 + 120, y, 80);
        AddLabel(pnlDetalji, "Zapremina:", col2 + 220, y);
        txtZapremina = AddTextBox(pnlDetalji, col2 + 310, y, 80);

        y += 30;

        // Red 5: Opis
        AddLabel(pnlDetalji, "Opis:", 10, y);
        txtOpis = AddTextBox(pnlDetalji, 130, y, 620);

        y += 30;

        // Red 6: Aktivan + Dugmad
        chkAktivan = new CheckBox { Text = "Aktivan", Location = new System.Drawing.Point(130, y), AutoSize = true, Checked = true };
        pnlDetalji.Controls.Add(chkAktivan);

        y += 35;

        btnNovi = new Button { Text = "Novi artikal", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(100, 30) };
        btnNovi.Click += BtnNovi_Click;
        pnlDetalji.Controls.Add(btnNovi);

        btnSacuvaj = new Button { Text = "Sačuvaj", Location = new System.Drawing.Point(120, y), Size = new System.Drawing.Size(100, 30), BackColor = System.Drawing.Color.LightGreen };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        pnlDetalji.Controls.Add(btnSacuvaj);

        btnObrisi = new Button { Text = "Deaktiviraj", Location = new System.Drawing.Point(230, y), Size = new System.Drawing.Size(100, 30), BackColor = System.Drawing.Color.LightCoral };
        btnObrisi.Click += BtnObrisi_Click;
        pnlDetalji.Controls.Add(btnObrisi);

        btnOsvezi = new Button { Text = "Osveži listu", Location = new System.Drawing.Point(340, y), Size = new System.Drawing.Size(100, 30) };
        btnOsvezi.Click += (s, e) => UcitajPodatke();
        pnlDetalji.Controls.Add(btnOsvezi);
    }

    private void UcitajPodatke()
    {
        try
        {
            var artikli = AppContext.ArtikalRepository.GetAll().ToList();

            var dt = new System.Data.DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Šifra", typeof(string));
            dt.Columns.Add("Naziv", typeof(string));
            dt.Columns.Add("Kategorija", typeof(int));
            dt.Columns.Add("JM", typeof(int));
            dt.Columns.Add("Cena kupovine", typeof(decimal));
            dt.Columns.Add("Cena prodaje", typeof(decimal));
            dt.Columns.Add("Min. stalje", typeof(int));
            dt.Columns.Add("Max. stalje", typeof(int));
            dt.Columns.Add("Aktivan", typeof(bool));

            foreach (var a in artikli)
            {
                dt.Rows.Add(a.ArtikalID, a.SifraArtikla, a.NazivArtikla, a.KategorijaID,
                    a.JedinicaMereID, a.CenaKupovine ?? 0, a.CenaProdaje ?? 0,
                    a.MinimalneStalje, a.MaksimalneStalje, a.Aktivan);
            }

            dgvArtikli.DataSource = dt;
            lblUkupno.Text = $"Ukupno: {artikli.Count} artikala";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri učitavanju podataka:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DgvArtikli_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvArtikli.CurrentRow == null) return;

        _selektovaniId = (int)dgvArtikli.CurrentRow.Cells["ID"].Value;

        try
        {
            var artikal = AppContext.ArtikalRepository.GetById(_selektovaniId.Value);
            if (artikal == null) return;

            txtSifra.Text = artikal.SifraArtikla;
            txtNaziv.Text = artikal.NazivArtikla;
            cmbKategorija.SelectedIndex = Math.Min(artikal.KategorijaID - 1, cmbKategorija.Items.Count - 1);
            cmbJedinicaMere.SelectedIndex = Math.Min(artikal.JedinicaMereID - 1, cmbJedinicaMere.Items.Count - 1);
            txtCenaKupovine.Text = artikal.CenaKupovine?.ToString("F2") ?? "";
            txtCenaProdaje.Text = artikal.CenaProdaje?.ToString("F2") ?? "";
            txtMinStalje.Text = artikal.MinimalneStalje.ToString();
            txtMaxStalje.Text = artikal.MaksimalneStalje.ToString();
            txtTezina.Text = artikal.Tezina?.ToString("F3") ?? "";
            txtZapremina.Text = artikal.Zapremina?.ToString("F3") ?? "";
            txtBarkod.Text = artikal.Barkod ?? "";
            txtOpis.Text = artikal.Opis ?? "";
            chkAktivan.Checked = artikal.Aktivan;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška: {ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnNovi_Click(object? sender, EventArgs e)
    {
        _selektovaniId = null;
        txtSifra.Clear(); txtNaziv.Clear();
        txtCenaKupovine.Clear(); txtCenaProdaje.Clear();
        txtMinStalje.Text = "10"; txtMaxStalje.Text = "1000";
        txtTezina.Clear(); txtZapremina.Clear();
        txtBarkod.Clear(); txtOpis.Clear();
        cmbKategorija.SelectedIndex = -1; cmbJedinicaMere.SelectedIndex = -1;
        chkAktivan.Checked = true;
        txtSifra.Focus();
    }

    private void BtnSacuvaj_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSifra.Text) || string.IsNullOrWhiteSpace(txtNaziv.Text))
        {
            MessageBox.Show("Šifra i naziv artikla su obavezni.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbKategorija.SelectedIndex < 0 || cmbJedinicaMere.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite kategoriju i jedinicu mere.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var artikal = new Artikal
            {
                SifraArtikla = txtSifra.Text.Trim(),
                NazivArtikla = txtNaziv.Text.Trim(),
                KategorijaID = cmbKategorija.SelectedIndex + 1,
                JedinicaMereID = cmbJedinicaMere.SelectedIndex + 1,
                CenaKupovine = decimal.TryParse(txtCenaKupovine.Text, out var ck) ? ck : null,
                CenaProdaje = decimal.TryParse(txtCenaProdaje.Text, out var cp) ? cp : null,
                MinimalneStalje = int.TryParse(txtMinStalje.Text, out var mins) ? mins : 10,
                MaksimalneStalje = int.TryParse(txtMaxStalje.Text, out var maxs) ? maxs : 1000,
                Tezina = decimal.TryParse(txtTezina.Text, out var t) ? t : null,
                Zapremina = decimal.TryParse(txtZapremina.Text, out var z) ? z : null,
                Barkod = string.IsNullOrWhiteSpace(txtBarkod.Text) ? null : txtBarkod.Text.Trim(),
                Opis = string.IsNullOrWhiteSpace(txtOpis.Text) ? null : txtOpis.Text.Trim(),
                Aktivan = chkAktivan.Checked
            };

            if (_selektovaniId.HasValue)
            {
                artikal.ArtikalID = _selektovaniId.Value;
                AppContext.ArtikalRepository.Update(artikal);
                MessageBox.Show("Artikal je uspešno ažuriran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                AppContext.ArtikalRepository.Add(artikal);
                _selektovaniId = artikal.ArtikalID;
                MessageBox.Show($"Novi artikal je kreiran (ID: {artikal.ArtikalID}).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            UcitajPodatke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri čuvanju:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnObrisi_Click(object? sender, EventArgs e)
    {
        if (!_selektovaniId.HasValue)
        {
            MessageBox.Show("Izaberite artikal za deaktivaciju.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show("Da li ste sigurni da želite da deaktivirate artikal?", "Potvrda",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            try
            {
                AppContext.ArtikalRepository.Delete(_selektovaniId.Value);
                MessageBox.Show("Artikal je deaktiviran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UcitajPodatke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private static Label AddLabel(Panel panel, string text, int x, int y)
    {
        var lbl = new Label { Text = text, Location = new System.Drawing.Point(x, y + 3), AutoSize = true };
        panel.Controls.Add(lbl);
        return lbl;
    }

    private static TextBox AddTextBox(Panel panel, int x, int y, int width)
    {
        var txt = new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(width, 23) };
        panel.Controls.Add(txt);
        return txt;
    }
}
