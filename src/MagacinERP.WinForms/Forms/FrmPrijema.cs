using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public partial class FrmPrijema : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbDobavljac = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnOtkazi = null!;
    private Button btnNovaPrijema = null!;

    // Stavka input
    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;
    private TextBox txtCena = null!;
    private TextBox txtLotBroj = null!;

    public FrmPrijema(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Prijema materijala (GRN)";
        this.Size = new System.Drawing.Size(900, 650);

        int y = 10;

        // Header
        this.Controls.Add(new Label { Text = "Prijema materijala", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 40;

        // Broj dokumenta
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        this.Controls.Add(txtBrojDokumenta);

        // Dobavljač
        this.Controls.Add(new Label { Text = "Dobavljač:", Location = new System.Drawing.Point(360, y), AutoSize = true });
        cmbDobavljac = new ComboBox { Location = new System.Drawing.Point(440, y), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDobavljac.Items.AddRange(new object[] { "ELEKTRO d.o.o.", "METAL Imports", "HEMIJA i hemikalije", "PLASTIK pro" });
        this.Controls.Add(cmbDobavljac);

        // Status
        lblStatus = new Label { Text = "Status: Nova", ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(710, y), AutoSize = true };
        this.Controls.Add(lblStatus);

        y += 35;

        // Napomena
        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(550, 23) };
        this.Controls.Add(txtNapomena);

        y += 40;

        // Separator
        this.Controls.Add(new Label { Text = "── Stavke prijeme ──", Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 25;

        // Stavka inputs
        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(70, y), Size = new System.Drawing.Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbArtikal.Items.AddRange(new object[] { "EL001 - Kondenzator", "EL002 - Otpornik", "ME001 - Vijak M8x20", "HE001 - Ulje motorno" });
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Količina:", Location = new System.Drawing.Point(280, y), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(345, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtKolicina);

        this.Controls.Add(new Label { Text = "Cena:", Location = new System.Drawing.Point(435, y), AutoSize = true });
        txtCena = new TextBox { Location = new System.Drawing.Point(480, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtCena);

        this.Controls.Add(new Label { Text = "LOT:", Location = new System.Drawing.Point(570, y), AutoSize = true });
        txtLotBroj = new TextBox { Location = new System.Drawing.Point(605, y), Size = new System.Drawing.Size(100, 23) };
        this.Controls.Add(txtLotBroj);

        btnDodajStavku = new Button { Text = "+", Location = new System.Drawing.Point(715, y), Size = new System.Drawing.Size(40, 25) };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(760, y), Size = new System.Drawing.Size(40, 25) };
        btnObrisiStavku.Click += BtnObrisiStavku_Click;
        this.Controls.Add(btnObrisiStavku);

        y += 35;

        // DataGridView za stavke
        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 250),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina");
        dgvStavke.Columns.Add("Cena", "Cena");
        dgvStavke.Columns.Add("Ukupno", "Ukupno");
        dgvStavke.Columns.Add("LOT", "LOT Broj");
        this.Controls.Add(dgvStavke);

        y += 260;

        // Action buttons
        btnNovaPrijema = new Button { Text = "Nova prijema", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(120, 35) };
        btnNovaPrijema.Click += BtnNovaPrijema_Click;
        this.Controls.Add(btnNovaPrijema);

        btnZavrsi = new Button { Text = "Završi prijemu", Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(120, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);

        btnOtkazi = new Button { Text = "Otkaži", Location = new System.Drawing.Point(270, y), Size = new System.Drawing.Size(100, 35), BackColor = System.Drawing.Color.LightCoral };
        btnOtkazi.Click += BtnOtkazi_Click;
        this.Controls.Add(btnOtkazi);
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtKolicina.Text) || string.IsNullOrWhiteSpace(txtCena.Text))
        {
            MessageBox.Show("Izaberite artikal i unesite količinu i cenu.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtKolicina.Text, out int kolicina) || kolicina <= 0)
        {
            MessageBox.Show("Količina mora biti pozitivan broj.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!decimal.TryParse(txtCena.Text, out decimal cena) || cena < 0)
        {
            MessageBox.Show("Cena mora biti validna.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        dgvStavke.Rows.Add(cmbArtikal.Text, kolicina, cena, kolicina * cena, txtLotBroj.Text);
        txtKolicina.Clear();
        txtCena.Clear();
        txtLotBroj.Clear();
    }

    private void BtnObrisiStavku_Click(object? sender, EventArgs e)
    {
        if (dgvStavke.CurrentRow != null)
            dgvStavke.Rows.Remove(dgvStavke.CurrentRow);
    }

    private void BtnNovaPrijema_Click(object? sender, EventArgs e)
    {
        txtBrojDokumenta.Clear();
        cmbDobavljac.SelectedIndex = -1;
        txtNapomena.Clear();
        dgvStavke.Rows.Clear();
        lblStatus.Text = "Status: Nova";
        lblStatus.ForeColor = System.Drawing.Color.Blue;
    }

    private void BtnZavrsi_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBrojDokumenta.Text))
        {
            MessageBox.Show("Unesite broj dokumenta.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvStavke.Rows.Count == 0)
        {
            MessageBox.Show("Dodajte bar jednu stavku.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // TODO: Save via PrijemaService
        lblStatus.Text = "Status: Završena";
        lblStatus.ForeColor = System.Drawing.Color.Green;
        MessageBox.Show("Prijema je uspešno završena.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnOtkazi_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show("Da li ste sigurni da želite da otkažete prijemu?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            lblStatus.Text = "Status: Otkazana";
            lblStatus.ForeColor = System.Drawing.Color.Red;
        }
    }
}
