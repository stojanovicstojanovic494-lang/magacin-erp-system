namespace MagacinERP.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.pnlHeader = new Panel();
        this.lblNaslov = new Label();
        this.lblPodnaslov = new Label();
        this.pnlLogin = new Panel();
        this.lblKorisnickoIme = new Label();
        this.txtKorisnickoIme = new TextBox();
        this.lblLozinka = new Label();
        this.txtLozinka = new TextBox();
        this.chkPrikaziLozinku = new CheckBox();
        this.btnPrijava = new Button();
        this.btnIzlaz = new Button();
        this.lblGreska = new Label();
        this.lnkZaboravljenaLozinka = new LinkLabel();
        this.lblVerzija = new Label();
        this.pnlHeader.SuspendLayout();
        this.pnlLogin.SuspendLayout();
        this.SuspendLayout();

        // pnlHeader
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(0, 71, 160);
        this.pnlHeader.Controls.Add(this.lblNaslov);
        this.pnlHeader.Controls.Add(this.lblPodnaslov);
        this.pnlHeader.Dock = DockStyle.Top;
        this.pnlHeader.Location = new System.Drawing.Point(0, 0);
        this.pnlHeader.Name = "pnlHeader";
        this.pnlHeader.Size = new System.Drawing.Size(420, 100);

        // lblNaslov
        this.lblNaslov.AutoSize = true;
        this.lblNaslov.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
        this.lblNaslov.ForeColor = System.Drawing.Color.White;
        this.lblNaslov.Location = new System.Drawing.Point(90, 18);
        this.lblNaslov.Name = "lblNaslov";
        this.lblNaslov.Text = "MAGACIN ERP";

        // lblPodnaslov
        this.lblPodnaslov.AutoSize = true;
        this.lblPodnaslov.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.lblPodnaslov.ForeColor = System.Drawing.Color.FromArgb(200, 220, 255);
        this.lblPodnaslov.Location = new System.Drawing.Point(92, 60);
        this.lblPodnaslov.Name = "lblPodnaslov";
        this.lblPodnaslov.Text = "Sistem za upravljanje magacinom";

        // pnlLogin
        this.pnlLogin.BackColor = System.Drawing.Color.White;
        this.pnlLogin.BorderStyle = BorderStyle.FixedSingle;
        this.pnlLogin.Controls.Add(this.lblKorisnickoIme);
        this.pnlLogin.Controls.Add(this.txtKorisnickoIme);
        this.pnlLogin.Controls.Add(this.lblLozinka);
        this.pnlLogin.Controls.Add(this.txtLozinka);
        this.pnlLogin.Controls.Add(this.chkPrikaziLozinku);
        this.pnlLogin.Controls.Add(this.btnPrijava);
        this.pnlLogin.Controls.Add(this.btnIzlaz);
        this.pnlLogin.Controls.Add(this.lblGreska);
        this.pnlLogin.Controls.Add(this.lnkZaboravljenaLozinka);
        this.pnlLogin.Location = new System.Drawing.Point(35, 120);
        this.pnlLogin.Name = "pnlLogin";
        this.pnlLogin.Size = new System.Drawing.Size(350, 320);

        // lblKorisnickoIme
        this.lblKorisnickoIme.AutoSize = true;
        this.lblKorisnickoIme.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.lblKorisnickoIme.Location = new System.Drawing.Point(25, 25);
        this.lblKorisnickoIme.Name = "lblKorisnickoIme";
        this.lblKorisnickoIme.Text = "Korisničko ime:";

        // txtKorisnickoIme
        this.txtKorisnickoIme.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.txtKorisnickoIme.Location = new System.Drawing.Point(25, 50);
        this.txtKorisnickoIme.Name = "txtKorisnickoIme";
        this.txtKorisnickoIme.Size = new System.Drawing.Size(295, 27);
        this.txtKorisnickoIme.MaxLength = 50;
        this.txtKorisnickoIme.PlaceholderText = "Unesite korisničko ime";

        // lblLozinka
        this.lblLozinka.AutoSize = true;
        this.lblLozinka.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.lblLozinka.Location = new System.Drawing.Point(25, 90);
        this.lblLozinka.Name = "lblLozinka";
        this.lblLozinka.Text = "Lozinka:";

        // txtLozinka
        this.txtLozinka.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.txtLozinka.Location = new System.Drawing.Point(25, 115);
        this.txtLozinka.Name = "txtLozinka";
        this.txtLozinka.Size = new System.Drawing.Size(295, 27);
        this.txtLozinka.MaxLength = 255;
        this.txtLozinka.PasswordChar = '*';
        this.txtLozinka.PlaceholderText = "Unesite lozinku";

        // chkPrikaziLozinku
        this.chkPrikaziLozinku.AutoSize = true;
        this.chkPrikaziLozinku.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkPrikaziLozinku.Location = new System.Drawing.Point(25, 150);
        this.chkPrikaziLozinku.Name = "chkPrikaziLozinku";
        this.chkPrikaziLozinku.Text = "Prikaži lozinku";
        this.chkPrikaziLozinku.CheckedChanged += (s, e) =>
        {
            txtLozinka.PasswordChar = chkPrikaziLozinku.Checked ? '\0' : '*';
        };

        // btnPrijava
        this.btnPrijava.BackColor = System.Drawing.Color.FromArgb(0, 71, 160);
        this.btnPrijava.FlatAppearance.BorderSize = 0;
        this.btnPrijava.FlatStyle = FlatStyle.Flat;
        this.btnPrijava.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        this.btnPrijava.ForeColor = System.Drawing.Color.White;
        this.btnPrijava.Location = new System.Drawing.Point(25, 185);
        this.btnPrijava.Name = "btnPrijava";
        this.btnPrijava.Size = new System.Drawing.Size(140, 40);
        this.btnPrijava.Text = "Prijava";
        this.btnPrijava.Cursor = Cursors.Hand;
        this.btnPrijava.Click += new EventHandler(this.btnPrijava_Click);

        // btnIzlaz
        this.btnIzlaz.BackColor = System.Drawing.Color.FromArgb(180, 180, 180);
        this.btnIzlaz.FlatAppearance.BorderSize = 0;
        this.btnIzlaz.FlatStyle = FlatStyle.Flat;
        this.btnIzlaz.Font = new System.Drawing.Font("Segoe UI", 11F);
        this.btnIzlaz.ForeColor = System.Drawing.Color.White;
        this.btnIzlaz.Location = new System.Drawing.Point(180, 185);
        this.btnIzlaz.Name = "btnIzlaz";
        this.btnIzlaz.Size = new System.Drawing.Size(140, 40);
        this.btnIzlaz.Text = "Izlaz";
        this.btnIzlaz.Cursor = Cursors.Hand;
        this.btnIzlaz.Click += new EventHandler(this.btnIzlaz_Click);

        // lblGreska
        this.lblGreska.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblGreska.ForeColor = System.Drawing.Color.Red;
        this.lblGreska.Location = new System.Drawing.Point(25, 235);
        this.lblGreska.Name = "lblGreska";
        this.lblGreska.Size = new System.Drawing.Size(295, 35);
        this.lblGreska.Text = "";
        this.lblGreska.Visible = false;

        // lnkZaboravljenaLozinka
        this.lnkZaboravljenaLozinka.AutoSize = true;
        this.lnkZaboravljenaLozinka.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lnkZaboravljenaLozinka.Location = new System.Drawing.Point(25, 278);
        this.lnkZaboravljenaLozinka.Name = "lnkZaboravljenaLozinka";
        this.lnkZaboravljenaLozinka.Text = "Zaboravljena lozinka?";
        this.lnkZaboravljenaLozinka.LinkClicked +=
            new LinkLabelLinkClickedEventHandler(this.lnkZaboravljenaLozinka_LinkClicked);

        // lblVerzija
        this.lblVerzija.AutoSize = true;
        this.lblVerzija.Font = new System.Drawing.Font("Segoe UI", 8F);
        this.lblVerzija.ForeColor = System.Drawing.Color.Gray;
        this.lblVerzija.Location = new System.Drawing.Point(155, 455);
        this.lblVerzija.Name = "lblVerzija";
        this.lblVerzija.Text = "Verzija 1.0.0";

        // LoginForm
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        this.ClientSize = new System.Drawing.Size(420, 480);
        this.Controls.Add(this.pnlHeader);
        this.Controls.Add(this.pnlLogin);
        this.Controls.Add(this.lblVerzija);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "LoginForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Magacin ERP - Prijava";
        this.Load += new EventHandler(this.LoginForm_Load);
        this.pnlHeader.ResumeLayout(false);
        this.pnlHeader.PerformLayout();
        this.pnlLogin.ResumeLayout(false);
        this.pnlLogin.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Panel pnlHeader;
    private Label lblNaslov;
    private Label lblPodnaslov;
    private Panel pnlLogin;
    private Label lblKorisnickoIme;
    private TextBox txtKorisnickoIme;
    private Label lblLozinka;
    private TextBox txtLozinka;
    private CheckBox chkPrikaziLozinku;
    private Button btnPrijava;
    private Button btnIzlaz;
    private Label lblGreska;
    private LinkLabel lnkZaboravljenaLozinka;
    private Label lblVerzija;
}
