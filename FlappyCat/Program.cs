using System;
using System.IO;
using System.Windows.Forms;

namespace FlappyCat
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                Varliklar.Yukle(Path.Combine(AppContext.BaseDirectory, "Assets"));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(
                    "Oyun dosyaları yüklenemedi.\n\n" + ex.Message +
                    "\n\nAssets klasörünün programla aynı klasörde olduğundan emin olun.",
                    "Flappy Cat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new OyunFormu());
        }
    }
}
