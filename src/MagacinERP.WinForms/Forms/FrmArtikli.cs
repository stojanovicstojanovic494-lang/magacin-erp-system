using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public partial class FrmArtikli : Form
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
    private ComboBox cmbKategorija = null!;
    private ComboBox cmbJedinicaMere = null!;
    private Button btnNovi = null!;
    private Button btnSacuvaj = null!;
    private Button btnObrisi = null!;
    private Button btnOsvezi = null!;

    public FrmArtikli()
    {
        InitializeComponent();
        UcitajPodatke();
    }

    private void InitializeComponent()
    {
        this.Text = "Artikli - Master podaci";
        this.Size = new System.Drawing.Size(900, 600);

        // DataGridView
        dgvArtikli = new DataGridView
        {
            Location = new System.Drawing.Point(10, 10),
            Size = new System.Drawing.Size(860, 250),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvArtikli.SelectionChanged += DgvArtikli_SelectionChanged;

        // Panel za detalje
        pnlDetalji = new Panel
        {
            Location = new System.Drawing.Point(10, 270),
            Size = new System.Drawing.Size(860, 250),
            BorderStyle = BorderStyle.FixedSingle
        };

        int y = 10;
        int labelWidth = 120;
        int inputWidth = 200;

        // Šifra
        pnlDetalji.Controls.Add(new Label { Text = "Šifra artikla:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtSifra = new TextBox { Location = new System.Drawing.Point(labelWidth, y), Size = new System.Drawing.Size(inputWidth, 23) };
        pnlDetalji.Controls.Add(txtSifra);

        // Naziv
        pnlDetalji.Controls.Add(new Label { Text = "Naziv:", Location = new System.Drawing.Point(440, y), AutoSize = true });
        txtNaziv = new TextBox { Location = new System.Drawing.Point(560, y), Size = new System.Drawing.Size(inputWidth, 23) };
        pnlDetalji.Controls.Add(txtNaziv);

        y += 35;

        // Kategorija
        pnlDetalji.Controls.Add(new Label { Text = "Kategorija:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbKategorija = new ComboBox { Location = new System.Drawing.Point(labelWidth, y), Size = new System.Drawing.Size(inputWidth, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbKategorija.Items.AddRange(new object[] { "Elektrotehnika", "Mehanika", "Hemikalije", "Tekstil", "Metali", "Plastika", "Ambalaža", "Razno" });
        pnlDetalji.Controls.Add(cmbKategorija);

        // Jedinica mere
        pnlDetalji.Controls.Add(new Label { Text = "Jedinica mere:", Location = new System.Drawing.Point(440, y), AutoSize = true });
        cmbJedinicaMere = new ComboBox { Location = new System.Drawing.Point(560, y), Size = new System.Drawing.Size(inputWidth, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbJedinicaMere.Items.AddRange(new object[] { "kom", "kg", "l", "m", "pak", "pal", "kut" });
        pnlDetalji.Controls.Add(cmbJedinicaMere);

        y += 35;

        // Cena kupovine
        pnlDetalji.Controls.Add(new Label { Text = "Cena kupovine:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtCenaKupovine = new TextBox { Location = new System.Drawing.Point(labelWidth, y), Size = new System.Drawing.Size(inputWidth, 23) };
        pnlDetalji.Controls.Add(txtCenaKupovine);

        // Cena prodaje
        pnlDetalji.Controls.Add(new Label { Text = "Cena prodaje:", Location = new System.Drawing.Point(440, y), AutoSize = true });
        txtCenaProdaje = new TextBox { Location = new System.Drawing.Point(560, y), Size = new System.Drawing.Size(inputWidth, 23) };
        pnlDetalji.Controls.Add(txtCenaProdaje);

        y += 35;

        // Min/Max stalje
        pnlDetalji.Controls.Add(new Label { Text = "Min. stalje:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtMinStalje = new TextBox { Location = new System.Drawing.Point(labelWidth, y), Size = new System.Drawing.Size(100, 23) };
        pnlDetalji.Controls.Add(txtMinStalje);

        pnlDetalji.Controls.Add(new Label { Text = "Max. stalje:", Location = new System.Drawing.Point(240, y), AutoSize = true });
        txtMaxStalje = new TextBox { Location = new System.Drawing.Point(340, y), Size = new System.Drawing.Size(100, 23) };
        pnlDetalji.Controls.Add(txtMaxStalje);

        // Barkod
        pnlDetalji.Controls.Add(new Label { Text = "Barkod:", Location = new System.Drawing.Point(440, y), AutoSize = true });
        txtBarkod = new TextBox { Location = new System.Drawing.Point(560, y), Size = new System.Drawing.Size(inputWidth, 23) };
        pnlDetalji.Controls.Add(txtBarkod);

        y += 35;

        // Opis
        pnlDetalji.Controls.Add(new Label { Text = "Opis:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtOpis = new TextBox { Location = new System.Drawing.Point(labelWidth, y), Size = new System.Drawing.Size(620, 23) };
        pnlDetalji.Controls.Add(txtOpis);

        y += 35;

        // Buttons
        btnNovi = new Button { Text = "Novi", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(80, 30) };
        btnNovi.Click += BtnNovi_Click;
        pnlDetalji.Controls.Add(btnNovi);

        btnSacuvaj = new Button { Text = "Sačuvaj", Location = new System.Drawing.Point(100, y), Size = new System.Drawing.Size(80, 30) };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        pnlDetalji.Controls.Add(btnSacuvaj);

        btnObrisi = new Button { Text = "Obriši", Location = new System.Drawing.Point(190, y), Size = new System.Drawing.Size(80, 30) };
        btnObrisi.Click += BtnObrisi_Click;
        pnlDetalji.Controls.Add(btnObrisi);

        btnOsvezi = new Button { Text = "Osveži", Location = new System.Drawing.Point(280, y), Size = new System.Drawing.Size(80, 30) };
        btnOsvezi.Click += (s, e) => UcitajPodatke();
        pnlDetalji.Controls.Add(btnOsvezi);

        this.Controls.AddRange(new Control[] { dgvArtikli, pnlDetalji });
    }

    private void UcitajPodatke()
    {
        // TODO: Load from repository/database
        // Demo data for layout verification
        var dt = new System.Data.DataTable();
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Šifra", typeof(string));
        dt.Columns.Add("Naziv", typeof(string));
        dt.Columns.Add("Kategorija", typeof(string));
        dt.Columns.Add("JM", typeof(string));
        dt.Columns.Add("Cena Kup.", typeof(decimal));
        dt.Columns.Add("Cena Prod.", typeof(decimal));

        dt.Rows.Add(1, "EL001", "Kondenzator 100μF", "Elektrotehnika", "kom", 50.00m, 75.00m);
        dt.Rows.Add(2, "EL002", "Otpornik 1kΩ", "Elektrotehnika", "kom", 10.00m, 15.00m);
        dt.Rows.Add(3, "ME001", "Vijak M8x20", "Mehanika", "kom", 2.50m, 4.00m);
        dt.Rows.Add(4, "HE001", "Ulje motorno 10W40", "Hemikalije", "l", 800.00m, 1200.00m);

        dgvArtikli.DataSource = dt;
    }

    private void DgvArtikli_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvArtikli.CurrentRow == null) return;

        txtSifra.Text = dgvArtikli.CurrentRow.Cells["Šifra"].Value?.ToString() ?? "";
        txtNaziv.Text = dgvArtikli.CurrentRow.Cells["Naziv"].Value?.ToString() ?? "";
    }

    private void BtnNovi_Click(object? sender, EventArgs e)
    {
        txtSifra.Clear();
        txtNaziv.Clear();
        txtCenaKupovine.Clear();
        txtCenaProdaje.Clear();
        txtMinStalje.Clear();
        txtMaxStalje.Clear();
        txtBarkod.Clear();
        txtOpis.Clear();
        txtSifra.Focus();
    }

    private void BtnSacuvaj_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtSifra.Text) || string.IsNullOrWhiteSpace(txtNaziv.Text))
        {
            MessageBox.Show("Šifra i naziv artikla su obavezni.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // TODO: Save to repository/database
        MessageBox.Show("Artikal je uspešno sačuvan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        UcitajPodatke();
    }

    private void BtnObrisi_Click(object? sender, EventArgs e)
    {
        if (dgvArtikli.CurrentRow == null) return;

        var result = MessageBox.Show("Da li ste sigurni da želite da obrišete artikal?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            // TODO: Delete from repository/database
            MessageBox.Show("Artikal je obrisan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UcitajPodatke();
        }
    }
}
