using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public partial class FrmKorisnici : Form
{
    private DataGridView dgvKorisnici = null!;
    private Panel pnlDetalji = null!;
    private TextBox txtKorisnickoIme = null!;
    private TextBox txtLozinka = null!;
    private TextBox txtIme = null!;
    private TextBox txtPrezime = null!;
    private TextBox txtEmail = null!;
    private ComboBox cmbUloga = null!;
    private CheckBox chkAktivan = null!;
    private Button btnNovi = null!;
    private Button btnSacuvaj = null!;
    private Button btnDeaktiviraj = null!;

    public FrmKorisnici()
    {
        InitializeComponent();
        UcitajPodatke();
    }

    private void InitializeComponent()
    {
        this.Text = "Administracija - Korisnici";
        this.Size = new System.Drawing.Size(800, 550);

        // DataGridView
        dgvKorisnici = new DataGridView
        {
            Location = new System.Drawing.Point(10, 10),
            Size = new System.Drawing.Size(760, 200),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvKorisnici.SelectionChanged += DgvKorisnici_SelectionChanged;

        // Panel za detalje
        pnlDetalji = new Panel
        {
            Location = new System.Drawing.Point(10, 220),
            Size = new System.Drawing.Size(760, 270),
            BorderStyle = BorderStyle.FixedSingle
        };

        int y = 15;

        pnlDetalji.Controls.Add(new Label { Text = "Detalji korisnika", Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(10, y), AutoSize = true });
        y += 30;

        // Korisničko ime
        pnlDetalji.Controls.Add(new Label { Text = "Korisničko ime:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtKorisnickoIme = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        pnlDetalji.Controls.Add(txtKorisnickoIme);

        // Uloga
        pnlDetalji.Controls.Add(new Label { Text = "Uloga:", Location = new System.Drawing.Point(370, y), AutoSize = true });
        cmbUloga = new ComboBox { Location = new System.Drawing.Point(420, y), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbUloga.Items.AddRange(new object[] { "Admin", "Magaciner", "Pregled" });
        pnlDetalji.Controls.Add(cmbUloga);

        y += 35;

        // Lozinka
        pnlDetalji.Controls.Add(new Label { Text = "Lozinka:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtLozinka = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23), PasswordChar = '*' };
        pnlDetalji.Controls.Add(txtLozinka);

        // Aktivan
        chkAktivan = new CheckBox { Text = "Aktivan", Location = new System.Drawing.Point(420, y), AutoSize = true, Checked = true };
        pnlDetalji.Controls.Add(chkAktivan);

        y += 35;

        // Ime
        pnlDetalji.Controls.Add(new Label { Text = "Ime:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtIme = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23) };
        pnlDetalji.Controls.Add(txtIme);

        // Prezime
        pnlDetalji.Controls.Add(new Label { Text = "Prezime:", Location = new System.Drawing.Point(370, y), AutoSize = true });
        txtPrezime = new TextBox { Location = new System.Drawing.Point(420, y), Size = new System.Drawing.Size(200, 23) };
        pnlDetalji.Controls.Add(txtPrezime);

        y += 35;

        // Email
        pnlDetalji.Controls.Add(new Label { Text = "Email:", Location = new System.Drawing.Point(10, y), AutoSize = true });
        txtEmail = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(300, 23) };
        pnlDetalji.Controls.Add(txtEmail);

        y += 40;

        // Buttons
        btnNovi = new Button { Text = "Novi korisnik", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(110, 30) };
        btnNovi.Click += BtnNovi_Click;
        pnlDetalji.Controls.Add(btnNovi);

        btnSacuvaj = new Button { Text = "Sačuvaj", Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(90, 30) };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        pnlDetalji.Controls.Add(btnSacuvaj);

        btnDeaktiviraj = new Button { Text = "Deaktiviraj", Location = new System.Drawing.Point(230, y), Size = new System.Drawing.Size(100, 30), BackColor = System.Drawing.Color.LightCoral };
        btnDeaktiviraj.Click += BtnDeaktiviraj_Click;
        pnlDetalji.Controls.Add(btnDeaktiviraj);

        this.Controls.AddRange(new Control[] { dgvKorisnici, pnlDetalji });
    }

    private void UcitajPodatke()
    {
        // TODO: Load from repository
        var dt = new System.Data.DataTable();
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Korisničko ime", typeof(string));
        dt.Columns.Add("Ime", typeof(string));
        dt.Columns.Add("Prezime", typeof(string));
        dt.Columns.Add("Uloga", typeof(string));
        dt.Columns.Add("Aktivan", typeof(bool));

        dt.Rows.Add(1, "admin", "Administratski", "Korisnik", "Admin", true);
        dt.Rows.Add(2, "magaciner", "Marko", "Marković", "Magaciner", true);
        dt.Rows.Add(3, "magaciner2", "Jovana", "Jovanović", "Magaciner", true);
        dt.Rows.Add(4, "pregled", "Pregleda", "Korisnik", "Pregled", true);

        dgvKorisnici.DataSource = dt;
    }

    private void DgvKorisnici_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvKorisnici.CurrentRow == null) return;

        txtKorisnickoIme.Text = dgvKorisnici.CurrentRow.Cells["Korisničko ime"].Value?.ToString() ?? "";
        txtIme.Text = dgvKorisnici.CurrentRow.Cells["Ime"].Value?.ToString() ?? "";
        txtPrezime.Text = dgvKorisnici.CurrentRow.Cells["Prezime"].Value?.ToString() ?? "";
        var uloga = dgvKorisnici.CurrentRow.Cells["Uloga"].Value?.ToString() ?? "";
        cmbUloga.SelectedItem = uloga;
        chkAktivan.Checked = (bool)(dgvKorisnici.CurrentRow.Cells["Aktivan"].Value ?? true);
    }

    private void BtnNovi_Click(object? sender, EventArgs e)
    {
        txtKorisnickoIme.Clear();
        txtLozinka.Clear();
        txtIme.Clear();
        txtPrezime.Clear();
        txtEmail.Clear();
        cmbUloga.SelectedIndex = -1;
        chkAktivan.Checked = true;
        txtKorisnickoIme.Focus();
    }

    private void BtnSacuvaj_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtKorisnickoIme.Text) || string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
        {
            MessageBox.Show("Korisničko ime, ime i prezime su obavezni.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbUloga.SelectedIndex < 0)
        {
            MessageBox.Show("Izaberite ulogu korisnika.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // TODO: Save via KorisnikService
        MessageBox.Show("Korisnik je uspešno sačuvan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        UcitajPodatke();
    }

    private void BtnDeaktiviraj_Click(object? sender, EventArgs e)
    {
        if (dgvKorisnici.CurrentRow == null) return;

        var result = MessageBox.Show("Da li ste sigurni da želite da deaktivirate korisnika?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            // TODO: Deactivate via KorisnikService
            MessageBox.Show("Korisnik je deaktiviran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            UcitajPodatke();
        }
    }
}
