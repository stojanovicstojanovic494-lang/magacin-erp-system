using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public class FrmKorisnici : Form
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
    private Button btnOsvezi = null!;
    private Label lblUkupno = null!;

    private int? _selektovaniId = null;

    public FrmKorisnici()
    {
        InitializeComponent();
        UcitajPodatke();
    }

    private void InitializeComponent()
    {
        this.Text = "Administracija - Korisnici";
        this.Size = new System.Drawing.Size(850, 600);

        var lblHeader = new Label
        {
            Text = "KORISNICI - Administracija",
            Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(10, 5),
            AutoSize = true
        };
        this.Controls.Add(lblHeader);

        lblUkupno = new Label { Text = "", Location = new System.Drawing.Point(650, 10), AutoSize = true };
        this.Controls.Add(lblUkupno);

        dgvKorisnici = new DataGridView
        {
            Location = new System.Drawing.Point(10, 35),
            Size = new System.Drawing.Size(810, 220),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            RowHeadersVisible = false
        };
        dgvKorisnici.SelectionChanged += DgvKorisnici_SelectionChanged;
        this.Controls.Add(dgvKorisnici);

        pnlDetalji = new Panel
        {
            Location = new System.Drawing.Point(10, 265),
            Size = new System.Drawing.Size(810, 250),
            BorderStyle = BorderStyle.FixedSingle
        };
        this.Controls.Add(pnlDetalji);

        var lblDetalji = new Label
        {
            Text = "Detalji korisnika:",
            Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(10, 5),
            AutoSize = true
        };
        pnlDetalji.Controls.Add(lblDetalji);

        int y = 30;

        AddLabel(pnlDetalji, "Korisničko ime:", 10, y);
        txtKorisnickoIme = AddTextBox(pnlDetalji, 140, y, 200);
        AddLabel(pnlDetalji, "Uloga:", 370, y);
        cmbUloga = new ComboBox { Location = new System.Drawing.Point(420, y), Size = new System.Drawing.Size(150, 23), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbUloga.Items.AddRange(new object[] { "Admin", "Magaciner", "Pregled" });
        pnlDetalji.Controls.Add(cmbUloga);

        y += 30;
        AddLabel(pnlDetalji, "Lozinka:", 10, y);
        txtLozinka = new TextBox { Location = new System.Drawing.Point(140, y), Size = new System.Drawing.Size(200, 23), PasswordChar = '*' };
        pnlDetalji.Controls.Add(txtLozinka);
        chkAktivan = new CheckBox { Text = "Aktivan", Location = new System.Drawing.Point(420, y), AutoSize = true, Checked = true };
        pnlDetalji.Controls.Add(chkAktivan);

        y += 30;
        AddLabel(pnlDetalji, "Ime:", 10, y);
        txtIme = AddTextBox(pnlDetalji, 140, y, 200);
        AddLabel(pnlDetalji, "Prezime:", 370, y);
        txtPrezime = AddTextBox(pnlDetalji, 420, y, 200);

        y += 30;
        AddLabel(pnlDetalji, "Email:", 10, y);
        txtEmail = AddTextBox(pnlDetalji, 140, y, 300);

        y += 40;

        btnNovi = new Button { Text = "Novi korisnik", Location = new System.Drawing.Point(10, y), Size = new System.Drawing.Size(110, 30) };
        btnNovi.Click += BtnNovi_Click;
        pnlDetalji.Controls.Add(btnNovi);

        btnSacuvaj = new Button { Text = "Sačuvaj", Location = new System.Drawing.Point(130, y), Size = new System.Drawing.Size(100, 30), BackColor = System.Drawing.Color.LightGreen };
        btnSacuvaj.Click += BtnSacuvaj_Click;
        pnlDetalji.Controls.Add(btnSacuvaj);

        btnDeaktiviraj = new Button { Text = "Deaktiviraj", Location = new System.Drawing.Point(240, y), Size = new System.Drawing.Size(100, 30), BackColor = System.Drawing.Color.LightCoral };
        btnDeaktiviraj.Click += BtnDeaktiviraj_Click;
        pnlDetalji.Controls.Add(btnDeaktiviraj);

        btnOsvezi = new Button { Text = "Osveži", Location = new System.Drawing.Point(350, y), Size = new System.Drawing.Size(80, 30) };
        btnOsvezi.Click += (s, e) => UcitajPodatke();
        pnlDetalji.Controls.Add(btnOsvezi);
    }

    private void UcitajPodatke()
    {
        try
        {
            var korisnici = AppContext.KorisnikRepository.GetAll().ToList();

            var dt = new System.Data.DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Korisničko ime", typeof(string));
            dt.Columns.Add("Ime", typeof(string));
            dt.Columns.Add("Prezime", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Uloga", typeof(string));
            dt.Columns.Add("Aktivan", typeof(bool));

            foreach (var k in korisnici)
                dt.Rows.Add(k.KorisnikID, k.KorisnickoIme, k.ImeKorisnika, k.Prezime, k.Email ?? "", k.Uloga ?? "", k.Aktivan);

            dgvKorisnici.DataSource = dt;
            lblUkupno.Text = $"Ukupno: {korisnici.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DgvKorisnici_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvKorisnici.CurrentRow == null) return;

        _selektovaniId = (int)dgvKorisnici.CurrentRow.Cells["ID"].Value;
        txtKorisnickoIme.Text = dgvKorisnici.CurrentRow.Cells["Korisničko ime"].Value?.ToString() ?? "";
        txtIme.Text = dgvKorisnici.CurrentRow.Cells["Ime"].Value?.ToString() ?? "";
        txtPrezime.Text = dgvKorisnici.CurrentRow.Cells["Prezime"].Value?.ToString() ?? "";
        txtEmail.Text = dgvKorisnici.CurrentRow.Cells["Email"].Value?.ToString() ?? "";
        var uloga = dgvKorisnici.CurrentRow.Cells["Uloga"].Value?.ToString() ?? "";
        cmbUloga.SelectedItem = uloga;
        chkAktivan.Checked = (bool)(dgvKorisnici.CurrentRow.Cells["Aktivan"].Value ?? true);
        txtLozinka.Clear();
    }

    private void BtnNovi_Click(object? sender, EventArgs e)
    {
        _selektovaniId = null;
        txtKorisnickoIme.Clear(); txtLozinka.Clear();
        txtIme.Clear(); txtPrezime.Clear(); txtEmail.Clear();
        cmbUloga.SelectedIndex = -1; chkAktivan.Checked = true;
        txtKorisnickoIme.Focus();
    }

    private void BtnSacuvaj_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtKorisnickoIme.Text) || string.IsNullOrWhiteSpace(txtIme.Text) || string.IsNullOrWhiteSpace(txtPrezime.Text))
        { MessageBox.Show("Korisničko ime, ime i prezime su obavezni.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbUloga.SelectedIndex < 0)
        { MessageBox.Show("Izaberite ulogu.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            if (_selektovaniId.HasValue)
            {
                var korisnik = AppContext.KorisnikRepository.GetById(_selektovaniId.Value);
                if (korisnik == null) return;

                korisnik.KorisnickoIme = txtKorisnickoIme.Text.Trim();
                korisnik.ImeKorisnika = txtIme.Text.Trim();
                korisnik.Prezime = txtPrezime.Text.Trim();
                korisnik.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                korisnik.Uloga = cmbUloga.Text;
                korisnik.Aktivan = chkAktivan.Checked;
                if (!string.IsNullOrWhiteSpace(txtLozinka.Text))
                    korisnik.Lozinka = txtLozinka.Text;

                AppContext.KorisnikRepository.Update(korisnik);
                MessageBox.Show("Korisnik je ažuriran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtLozinka.Text) || txtLozinka.Text.Length < 6)
                { MessageBox.Show("Lozinka mora imati bar 6 karaktera.", "Validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var korisnikService = AppContext.KorisnikService;
                korisnikService.DodajKorisnika(
                    txtKorisnickoIme.Text.Trim(),
                    txtLozinka.Text,
                    txtIme.Text.Trim(),
                    txtPrezime.Text.Trim(),
                    cmbUloga.Text,
                    string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
                );
                MessageBox.Show("Novi korisnik je kreiran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UcitajPodatke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDeaktiviraj_Click(object? sender, EventArgs e)
    {
        if (!_selektovaniId.HasValue) { MessageBox.Show("Izaberite korisnika.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        var result = MessageBox.Show("Da li ste sigurni da želite da deaktivirate korisnika?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            try
            {
                AppContext.KorisnikService.DeaktivirajKorisnika(_selektovaniId.Value);
                MessageBox.Show("Korisnik je deaktiviran.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
