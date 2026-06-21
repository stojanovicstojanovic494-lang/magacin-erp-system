namespace MagacinERP.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.FrmLogin());
    }
}
