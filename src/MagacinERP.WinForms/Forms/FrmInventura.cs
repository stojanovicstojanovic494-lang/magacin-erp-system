namespace MagacinERP.WinForms.Forms;

public partial class FrmInventura : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbTipInventure = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnZavrsi = null!;
    private Button btnNovaInventura = null!;
    private CheckBox chkPrimeniRazlike = null!;

    // Stavka input
    private ComboBox cmbLokacija = null!;
    private ComboBox cmbArtikal = null!;
    private TextBox txtUtvrdjenaKolicina = null!;

    public FrmInventura(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Inventura - Prebrojavanje";
        this.Size = new System.Drawing.Size(900, 650);

        int y = 10;

        // Header
        this.Controls.Add(new Label { Text = "Inventura (Prebrojavanje)", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 40;

        // Broj dokumenta
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        this.Controls.Add(txtBrojDokumenta);

        // Tip inventure
        this.Controls.Add(new Label { Text = "Tip inventure:", Location = new System.Drawing.Point(360, y), AutoSize = true });
        cmbTipInventure = new ComboBox { Location = new System.Drawing.Point(460, y), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipInventure.Items.AddRange(new object[] { "Parcijalna", "Potpuna" });
        this.Controls.Add(cmbTipInventure);

        // Status
        lblStatus = new Label { Text = "Status: U toku", ForeColor = System.Drawing.Color.Orange, Location = new System.Drawing.Point(640, y), AutoSize = true };
        this.Controls.Add(lblStatus);

        y += 35;

        // Napomena
        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(550, 23) };
        this.Controls.Add(txtNapomena);

        y += 40;

        // Stavke header
        this.Controls.Add(new Label { Text = "── Stavke inventure ──", Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 25;

        // Stavka input
        this.Controls.Add(new Label { Text = "Lokacija:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbLokacija = new ComboBox { Location = new System.Drawing.Point(80, y), Size = new System.Drawing.Size(180, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbLokacija.Items.AddRange(new object[] { "Polica A1-1", "Polica A1-2", "Polica B1-1", "Polica C1-1", "Polica D1-1" });
        this.Controls.Add(cmbLokacija);

        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(270, y), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(325, y), Size = new System.Drawing.Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbArtikal.Items.AddRange(new object[] { "EL001 - Kondenzator", "EL002 - Otpornik", "ME001 - Vijak M8x20", "HE001 - Ulje motorno" });
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Prebrojano:", Location = new System.Drawing.Point(535, y), AutoSize = true });
        txtUtvrdjenaKolicina = new TextBox { Location = new System.Drawing.Point(610, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtUtvrdjenaKolicina);

        btnDodajStavku = new Button { Text = "+", Location = new System.Drawing.Point(700, y), Size = new System.Drawing.Size(40, 25) };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(745, y), Size = new System.Drawing.Size(40, 25) };
        btnObrisiStavku.Click += BtnObrisiStavku_Click;
        this.Controls.Add(btnObrisiStavku);

        y += 35;

        // DataGridView
        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 230),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvStavke.Columns.Add("Lokacija", "Lokacija");
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Prebrojano", "Prebrojano");
        dgvStavke.Columns.Add("Sistemska", "Sistemska količina");
        dgvStavke.Columns.Add("Razlika", "Razlika");
        this.Controls.Add(dgvStavke);

        y += 240;

        // Primeni razlike
        chkPrimeniRazlike = new CheckBox { Text = "Primeni razlike na zalihe nakon završetka", Location = new System.Drawing.Point(10, y), AutoSize = true };
        this.Controls.Add(chkPrimeniRazlike);

        y += 30;

        // Buttons
        btnNovaInventura = new Button { Text = "Nova inventura", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(130, 35) };
        btnNovaInventura.Click += BtnNovaInventura_Click;
        this.Controls.Add(btnNovaInventura);

        btnZavrsi = new Button { Text = "Završi inventuru", Location = new System.Drawing.Point(150, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnZavrsi.Click += BtnZavrsi_Click;
        this.Controls.Add(btnZavrsi);
    }

    private void BtnDodajStavku_Click(object? sender, EventArgs e)
    {
        if (cmbLokacija.SelectedIndex < 0 || cmbArtikal.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtUtvrdjenaKolicina.Text))
        {
            MessageBox.Show("Izaberite lokaciju, artikal i unesite prebrojanu količinu.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtUtvrdjenaKolicina.Text, out int kolicina) || kolicina < 0)
        {
            MessageBox.Show("Količina mora biti nenegativan broj.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Sistemska količina demo
        int sistemska = 100;
        int razlika = kolicina - sistemska;

        dgvStavke.Rows.Add(cmbLokacija.Text, cmbArtikal.Text, kolicina, sistemska, razlika);
        txtUtvrdjenaKolicina.Clear();
    }

    private void BtnObrisiStavku_Click(object? sender, EventArgs e)
    {
        if (dgvStavke.CurrentRow != null)
            dgvStavke.Rows.Remove(dgvStavke.CurrentRow);
    }

    private void BtnNovaInventura_Click(object? sender, EventArgs e)
    {
        txtBrojDokumenta.Clear();
        cmbTipInventure.SelectedIndex = -1;
        txtNapomena.Clear();
        dgvStavke.Rows.Clear();
        chkPrimeniRazlike.Checked = false;
        lblStatus.Text = "Status: U toku";
        lblStatus.ForeColor = System.Drawing.Color.Orange;
    }

    private void BtnZavrsi_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBrojDokumenta.Text))
        {
            MessageBox.Show("Unesite broj dokumenta.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbTipInventure.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite tip inventure.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvStavke.Rows.Count == 0)
        {
            MessageBox.Show("Dodajte bar jednu stavku.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string poruka = chkPrimeniRazlike.Checked
            ? "Inventura je završena i razlike su primenjene na zalihe."
            : "Inventura je završena (razlike NISU primenjene).";

        // TODO: Execute via InventuraService
        lblStatus.Text = "Status: Završena";
        lblStatus.ForeColor = System.Drawing.Color.Green;
        MessageBox.Show(poruka, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
