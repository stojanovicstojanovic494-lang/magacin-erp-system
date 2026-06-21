namespace MagacinERP.WinForms.Forms;

public partial class FrmTransfer : Form
{
    private readonly string _korisnik;

    private TextBox txtBrojDokumenta = null!;
    private ComboBox cmbIzLokacije = null!;
    private ComboBox cmbULokaciju = null!;
    private TextBox txtNapomena = null!;
    private Label lblStatus = null!;
    private DataGridView dgvStavke = null!;
    private Button btnDodajStavku = null!;
    private Button btnObrisiStavku = null!;
    private Button btnIzvrsi = null!;
    private Button btnNoviTransfer = null!;

    // Stavka input
    private ComboBox cmbArtikal = null!;
    private TextBox txtKolicina = null!;

    public FrmTransfer(string korisnik)
    {
        _korisnik = korisnik;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Transfer materijala";
        this.Size = new System.Drawing.Size(900, 600);

        int y = 10;

        // Header
        this.Controls.Add(new Label { Text = "Transfer materijala", Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 40;

        // Broj dokumenta
        this.Controls.Add(new Label { Text = "Broj dokumenta:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtBrojDokumenta = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        this.Controls.Add(txtBrojDokumenta);

        // Status
        lblStatus = new Label { Text = "Status: Nova", ForeColor = System.Drawing.Color.Blue, Location = new System.Drawing.Point(640, y), AutoSize = true };
        this.Controls.Add(lblStatus);

        y += 35;

        // Iz lokacije
        this.Controls.Add(new Label { Text = "Iz lokacije:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbIzLokacije = new ComboBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbIzLokacije.Items.AddRange(new object[] { "Polica A1-1 (Zona A)", "Polica A1-2 (Zona A)", "Polica B1-1 (Zona B)", "Polica C1-1 (Zona C)", "Polica D1-1 (Zona D)" });
        this.Controls.Add(cmbIzLokacije);

        // U lokaciju
        this.Controls.Add(new Label { Text = "U lokaciju:", Location = new System.Drawing.Point(410, y), AutoSize = true });
        cmbULokaciju = new ComboBox { Location = new System.Drawing.Point(490, y), Size = new System.Drawing.Size(250, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbULokaciju.Items.AddRange(new object[] { "Polica A1-1 (Zona A)", "Polica A1-2 (Zona A)", "Polica B1-1 (Zona B)", "Polica C1-1 (Zona C)", "Polica D1-1 (Zona D)" });
        this.Controls.Add(cmbULokaciju);

        y += 35;

        // Napomena
        this.Controls.Add(new Label { Text = "Napomena:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtNapomena = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(550, 23) };
        this.Controls.Add(txtNapomena);

        y += 40;

        // Stavke header
        this.Controls.Add(new Label { Text = "── Stavke transfera ──", Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 25;

        // Stavka input
        this.Controls.Add(new Label { Text = "Artikal:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        cmbArtikal = new ComboBox { Location = new System.Drawing.Point(70, y), Size = new System.Drawing.Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbArtikal.Items.AddRange(new object[] { "EL001 - Kondenzator", "EL002 - Otpornik", "ME001 - Vijak M8x20", "HE001 - Ulje motorno" });
        this.Controls.Add(cmbArtikal);

        this.Controls.Add(new Label { Text = "Količina:", Location = new System.Drawing.Point(380, y), AutoSize = true });
        txtKolicina = new TextBox { Location = new System.Drawing.Point(445, y), Size = new System.Drawing.Size(80, 23) };
        this.Controls.Add(txtKolicina);

        btnDodajStavku = new Button { Text = "+", Location = new System.Drawing.Point(535, y), Size = new System.Drawing.Size(40, 25) };
        btnDodajStavku.Click += BtnDodajStavku_Click;
        this.Controls.Add(btnDodajStavku);

        btnObrisiStavku = new Button { Text = "-", Location = new System.Drawing.Point(580, y), Size = new System.Drawing.Size(40, 25) };
        btnObrisiStavku.Click += BtnObrisiStavku_Click;
        this.Controls.Add(btnObrisiStavku);

        y += 35;

        // DataGridView
        dgvStavke = new DataGridView
        {
            Location = new System.Drawing.Point(10, y),
            Size = new System.Drawing.Size(860, 200),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvStavke.Columns.Add("Artikal", "Artikal");
        dgvStavke.Columns.Add("Kolicina", "Količina");
        this.Controls.Add(dgvStavke);

        y += 210;

        // Buttons
        btnNoviTransfer = new Button { Text = "Novi transfer", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(120, 35) };
        btnNoviTransfer.Click += BtnNoviTransfer_Click;
        this.Controls.Add(btnNoviTransfer);

        btnIzvrsi = new Button { Text = "Izvrši transfer", Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(130, 35), BackColor = System.Drawing.Color.LightGreen };
        btnIzvrsi.Click += BtnIzvrsi_Click;
        this.Controls.Add(btnIzvrsi);
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

        dgvStavke.Rows.Add(cmbArtikal.Text, kolicina);
        txtKolicina.Clear();
    }

    private void BtnObrisiStavku_Click(object? sender, EventArgs e)
    {
        if (dgvStavke.CurrentRow != null)
            dgvStavke.Rows.Remove(dgvStavke.CurrentRow);
    }

    private void BtnNoviTransfer_Click(object? sender, EventArgs e)
    {
        txtBrojDokumenta.Clear();
        cmbIzLokacije.SelectedIndex = -1;
        cmbULokaciju.SelectedIndex = -1;
        txtNapomena.Clear();
        dgvStavke.Rows.Clear();
        lblStatus.Text = "Status: Nova";
        lblStatus.ForeColor = System.Drawing.Color.Blue;
    }

    private void BtnIzvrsi_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBrojDokumenta.Text))
        {
            MessageBox.Show("Unesite broj dokumenta.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbIzLokacije.SelectedIndex < 0 || cmbULokaciju.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite izlaznu i ulaznu lokaciju.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbIzLokacije.SelectedIndex == cmbULokaciju.SelectedIndex)
        {
            MessageBox.Show("Izlazna i ulazna lokacija ne mogu biti iste.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvStavke.Rows.Count == 0)
        {
            MessageBox.Show("Dodajte bar jednu stavku.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // TODO: Execute via TransferService
        lblStatus.Text = "Status: Izvršen";
        lblStatus.ForeColor = System.Drawing.Color.Green;
        MessageBox.Show("Transfer je uspešno izvršen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
