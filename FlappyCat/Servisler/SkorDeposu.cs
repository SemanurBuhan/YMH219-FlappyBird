using System;
using System.IO;

namespace FlappyCat
{
    public class SkorDeposu
    {
        private readonly string klasor;
        private readonly string dosyaYolu;

        public SkorDeposu()
        {
            klasor = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlappyCat");
            dosyaYolu = Path.Combine(klasor, "enyuksekskor.txt");
        }

        public int Oku()
        {
            try
            {
                if (!File.Exists(dosyaYolu)) return 0;
                int skor;
                return int.TryParse(File.ReadAllText(dosyaYolu).Trim(), out skor) ? skor : 0;
            }
            catch (IOException)
            {
                return 0;
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }
        }

        public void Kaydet(int skor)
        {
            try
            {
                Directory.CreateDirectory(klasor);
                File.WriteAllText(dosyaYolu, skor.ToString());
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
