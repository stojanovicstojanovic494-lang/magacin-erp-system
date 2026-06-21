using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public partial class FrmIzdavanje : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbTipIzdavanja = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnNovoIzdavanje = null!;

    // Stavka input
    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;
    private TextBox txtCena = null!;

    public FrmIzdavanje(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Izdavanje materijala";
        this.Size = new System.Drawing.Size(900, 600);

        int y = 10;

        // Header
        this.Controls.Add(new Label { Text = "Izdavanje materijala", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 40;

        // Broj dokumenta
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        this.Controls.Add(txtBrojDokumenta);

        // Tip izdavanja
        this.Controls.Add(new Label { Text = "Tip izdavanja:", Location = new System.Drawing.Point(360, y), AutoSize = true });
        cmbTipIzdavanja = new ComboBox { Location = new System.Drawing.Point(460, y), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipIzdavanja.Items.AddRange(new object[] { "Proizvodnja", "Prodaja", "Povracaj", "Transfer" });
        this.Controls.Add(cmbTipIzdavanja);

        // Status
        lblStatus = new Label { Text = "Status: Nova", ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(640, y), AutoSize = true };
        this.Controls.Add(lblStatus);

        y += 35;

        // Napomena
        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(550, 23) };
        this.Controls.Add(txtNapomena);

        y += 40;

        // Stavke header
        this.Controls.Add(new Label { Text = "── Stavke izdavanja ──", Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 25;

        // Stavka inputs
        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(70, y), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbArtikal.Items.AddRange(new object[] { "EL001 - Kondenzator", "EL002 - Otpornik", "ME001 - Vijak M8x20", "HE001 - Ulje motorno" });
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Količina:", Location = new System.Drawing.Point(330, y), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(395, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtKolicina);

        this.Controls.Add(new Label { Text = "Cena:", Location = new System.Drawing.Point(485, y), AutoSize = true });
        txtCena = new TextBox { Location = new System.Drawing.Point(530, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtCena);

        btnDodajStavku = new Button { Text = "+", Location = new System.Drawing.Point(620, y), Size = new System.Drawing.Size(40, 25) };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(665, y), Size = new System.Drawing.Size(40, 25) };
        btnObrisiStavku.Click += BtnObrisiStavku_Click;
        this.Controls.Add(btnObrisiStavku);

        y += 35;

        // DataGridView
        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 220),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina");
        dgvStavke.Columns.Add("Cena", "Cena");
        dgvStavke.Columns.Add("Ukupno", "Ukupno");
        this.Controls.Add(dgvStavke);

        y += 230;

        // Buttons
        btnNovoIzdavanje = new Button { Text = "Novo izdavanje", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(130, 35) };
        btnNovoIzdavanje.Click += BtnNovoIzdavanje_Click;
        this.Controls.Add(btnNovoIzdavanje);

        btnZavrsi = new Button { Text = "Završi izdavanje", Location = new System.Drawing.Point(150, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtKolicina.Text))
        {
            MessageBox.Show("Izaberite artikal i unesite količinu.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtKolicina.Text, out int kolicina) || kolicina <= 0)
        {
            MessageBox.Show("Količina mora biti pozitivan broj.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal.TryParse(txtCena.Text, out decimal cena);
        dgvStavke.Rows.Add(cmbArtikal.Text, kolicina, cena, kolicina * cena);
        txtKolicina.Clear();
        txtCena.Clear();
    }

    private void BtnObrisiStavku_Click(object? sender, EventArgs e)
    {
        if (dgvStavke.CurrentRow != null)
            dgvStavke.Rows.Remove(dgvStavke.CurrentRow);
    }

    private void BtnNovoIzdavanje_Click(object? sender, EventArgs e)
    {
        txtBrojDokumenta.Clear();
        cmbTipIzdavanja.SelectedIndex = -1;
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

        if (cmbTipIzdavanja.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite tip izdavanja.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvStavke.Rows.Count == 0)
        {
            MessageBox.Show("Dodajte bar jednu stavku.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // TODO: Save via IzdavanjeService
        lblStatus.Text = "Status: Završena";
        lblStatus.ForeColor = System.Drawing.Color.Green;
        MessageBox.Show("Izdavanje je uspešno završeno.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
