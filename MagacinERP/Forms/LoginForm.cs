using MagacinERP.Helpers;

namespace MagacinERP.Forms;

public partial class LoginForm : Form
{
    private int _failedAttempts;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {
        txtKorisnickoIme.Focus();
        AcceptButton = btnPrijava;
    }

    private void btnPrijava_Click(object sender, EventArgs e)
    {
        lblGreska.Visible = false;

        string korisnickoIme = txtKorisnickoIme.Text.Trim();
        string lozinka = txtLozinka.Text;

        if (string.IsNullOrWhiteSpace(korisnickoIme))
        {
            PrikaziGresku("Unesite korisničko ime.");
            txtKorisnickoIme.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(lozinka))
        {
            PrikaziGresku("Unesite lozinku.");
            txtLozinka.Focus();
            return;
        }

        try
        {
            btnPrijava.Enabled = false;
            btnPrijava.Text = "Prijavljivanje...";
            Cursor = Cursors.WaitCursor;

            var (korisnikId, uloga) = DatabaseHelper.ValidacijaKorisnika(korisnickoIme, lozinka);

            if (korisnikId == -1 || uloga == null)
            {
                _failedAttempts++;
                PrikaziGresku($"Pogrešno korisničko ime ili lozinka. (Pokušaj {_failedAttempts})");
                txtLozinka.Clear();
                txtLozinka.Focus();

                if (_failedAttempts >= 5)
                {
                    MessageBox.Show(
                        "Previše neuspešnih pokušaja prijavljivanja.\nAplikacija će biti zatvorena.",
                        "Zaključano",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Application.Exit();
                }

                return;
            }

            SessionInfo.KorisnikID = korisnikId;
            SessionInfo.KorisnickoIme = korisnickoIme;
            SessionInfo.Uloga = uloga;

            MessageBox.Show(
                $"Dobrodošli, {korisnickoIme}!\nUloga: {uloga}",
                "Uspešna prijava",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // TODO: Open main form
            // var mainForm = new GlavnaForma();
            // mainForm.Show();
            // this.Hide();
        }
        catch (Exception ex)
        {
            PrikaziGresku("Greška pri povezivanju sa bazom podataka.");
            MessageBox.Show(
                $"Detalji greške:\n{ex.Message}",
                "Greška",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            btnPrijava.Enabled = true;
            btnPrijava.Text = "Prijava";
            Cursor = Cursors.Default;
        }
    }

    private void PrikaziGresku(string poruka)
    {
        lblGreska.Text = poruka;
        lblGreska.Visible = true;
    }

    private void lnkZaboravljenaLozinka_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        MessageBox.Show(
            "Kontaktirajte administratora za resetovanje lozinke.",
            "Zaboravljena lozinka",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnIzlaz_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }
}
