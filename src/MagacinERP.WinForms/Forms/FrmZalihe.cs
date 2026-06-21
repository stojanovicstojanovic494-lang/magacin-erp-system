using MagacinERP.Core.Models;

namespace MagacinERP.WinForms.Forms;

public class FrmZalihe : Form
{
    private DataGridView dgvZalihe = null!;
    private Button btnOsvezi = null!;
    private Button btnNiskaZaliha = null!;
    private Label lblUkupno = null!;
    private Label lblFilter = null!;
    private TextBox txtFilter = null!;

    public FrmZalihe()
    {
        InitializeComponent();
        UcitajPodatke();
    }

    private void InitializeComponent()
    {
        this.Text = "Pregled zaliha";
        this.Size = new System.Drawing.Size(900, 550);

        var lblHeader = new Label
        {
            Text = "PREGLED ZALIHA",
            Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
            Location = new System.Drawing.Point(10, 5),
            AutoSize = true
        };
        this.Controls.Add(lblHeader);

        lblFilter = new Label { Text = "Pretraga:", Location = new System.Drawing.Point(300, 10), AutoSize = true };
        this.Controls.Add(lblFilter);
        txtFilter = new TextBox { Location = new System.Drawing.Point(370, 7), Size = new System.Drawing.Size(200, 23) };
        txtFilter.TextChanged += (s, e) => Filtriraj();
        this.Controls.Add(txtFilter);

        lblUkupno = new Label { Text = "", Location = new System.Drawing.Point(600, 10), AutoSize = true };
        this.Controls.Add(lblUkupno);

        dgvZalihe = new DataGridView
        {
            Location = new System.Drawing.Point(10, 40),
            Size = new System.Drawing.Size(860, 410),
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = System.Drawing.Color.White,
            RowHeadersVisible = false
        };
        this.Controls.Add(dgvZalihe);

        btnOsvezi = new Button { Text = "Osveži", Location = new System.Drawing.Point(10, 460), Size = new System.Drawing.Size(100, 30) };
        btnOsvezi.Click += (s, e) => UcitajPodatke();
        this.Controls.Add(btnOsvezi);

        btnNiskaZaliha = new Button { Text = "Niska zaliha", Location = new System.Drawing.Point(120, 460), Size = new System.Drawing.Size(120, 30), BackColor = System.Drawing.Color.LightYellow };
        btnNiskaZaliha.Click += BtnNiskaZaliha_Click;
        this.Controls.Add(btnNiskaZaliha);
    }

    private System.Data.DataTable? _dataTable;

    private void UcitajPodatke()
    {
        try
        {
            var zalihe = AppContext.ZaliheRepository.GetAll().ToList();

            _dataTable = new System.Data.DataTable();
            _dataTable.Columns.Add("Artikal ID", typeof(int));
            _dataTable.Columns.Add("Lokacija ID", typeof(int));
            _dataTable.Columns.Add("Količina", typeof(int));
            _dataTable.Columns.Add("Rezervovano", typeof(int));
            _dataTable.Columns.Add("Disponibilna", typeof(int));
            _dataTable.Columns.Add("Poslednja izmena", typeof(DateTime));

            foreach (var z in zalihe)
            {
                _dataTable.Rows.Add(z.ArtikalID, z.LokacijaID, z.Kolicina,
                    z.RezervovanoKolicina, z.DisponibilnaKolicina, z.DatumZadnjeIzmene);
            }

            dgvZalihe.DataSource = _dataTable;
            lblUkupno.Text = $"Ukupno: {zalihe.Count} stavki";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška pri učitavanju zaliha:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Filtriraj()
    {
        if (_dataTable == null) return;
        string filter = txtFilter.Text.Trim();
        if (string.IsNullOrEmpty(filter))
            _dataTable.DefaultView.RowFilter = "";
        else
            _dataTable.DefaultView.RowFilter = $"[Artikal ID] = {(int.TryParse(filter, out var id) ? id : -1)}";
    }

    private void BtnNiskaZaliha_Click(object? sender, EventArgs e)
    {
        try
        {
            var niska = AppContext.ZaliheService.GetArtikliSaNiskomZalijhom().ToList();
            if (niska.Count == 0)
            {
                MessageBox.Show("Nema artikala sa niskom zalihom.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string poruka = "Artikli sa niskom zalihom:\n\n";
            foreach (var a in niska)
                poruka += $"  - {a.SifraArtikla}: {a.NazivArtikla} (min: {a.MinimalneStalje})\n";

            MessageBox.Show(poruka, "Niska zaliha", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Greška:\n{ex.Message}", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
