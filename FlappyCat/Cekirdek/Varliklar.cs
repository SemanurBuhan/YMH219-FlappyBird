using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FlappyCat
{
    /// <summary>
    /// Oyunda kullanılan tüm görselleri program başında bir kez yükler.
    /// </summary>
    public static class Varliklar
    {
        public static string Klasor { get; private set; }

        public static Bitmap Arkaplan { get; private set; }
        public static Dictionary<KarakterTuru, Bitmap[]> KediKareleri { get; private set; }
        public static Dictionary<KarakterTuru, Bitmap> SecimGorselleri { get; private set; }

        public static Bitmap EngelKapak { get; private set; }
        public static Bitmap EngelKapakTers { get; private set; }
        public static Bitmap EngelGovde { get; private set; }
        public static Bitmap EngelGovdeTers { get; private set; }
        public static Bitmap EngelDolgu { get; private set; }

        public static Bitmap[] Bulutlar { get; private set; }
        public static Bitmap PatiPembe { get; private set; }
        public static Bitmap PatiMor { get; private set; }
        public static Bitmap PatiKrem { get; private set; }
        public static Bitmap Altin { get; private set; }
        public static Bitmap Parilti { get; private set; }
        public static Bitmap Pat { get; private set; }
        public static Bitmap Panel { get; private set; }
        public static Bitmap ButonPlay { get; private set; }
        public static Bitmap ButonMenu { get; private set; }
        public static Bitmap ButonMenuKucuk { get; private set; }
        public static Bitmap ButonDevam { get; private set; }

        public static void Yukle(string klasor)
        {
            Klasor = klasor;

            Arkaplan = Gorsel("arkaplan");

            KediKareleri = new Dictionary<KarakterTuru, Bitmap[]>();
            SecimGorselleri = new Dictionary<KarakterTuru, Bitmap>();
            foreach (KarakterTuru tur in new[] { KarakterTuru.Gri, KarakterTuru.Turuncu, KarakterTuru.Siyah })
            {
                string ek = KarakterBilgisi.DosyaEki(tur);
                var kareler = new Bitmap[4];
                for (int i = 0; i < kareler.Length; i++)
                    kareler[i] = Gorsel("kedi_" + ek + "_" + i);
                KediKareleri[tur] = kareler;
                SecimGorselleri[tur] = Gorsel("secim_" + ek);
            }

            EngelKapak = Gorsel("engel_kapak");
            EngelGovde = Gorsel("engel_govde");
            EngelDolgu = Gorsel("engel_dolgu");
            EngelKapakTers = Ters(EngelKapak);
            EngelGovdeTers = Ters(EngelGovde);

            Bulutlar = new Bitmap[9];
            for (int i = 0; i < Bulutlar.Length; i++)
                Bulutlar[i] = Gorsel("bulut_" + i);

            PatiPembe = Gorsel("pati_pembe");
            PatiMor = Gorsel("pati_mor");
            PatiKrem = Gorsel("pati_krem");
            Altin = Gorsel("altin");
            Parilti = Gorsel("parilti");
            Pat = Gorsel("pat");
            Panel = Gorsel("panel");
            ButonPlay = Gorsel("buton_play");
            ButonMenu = Gorsel("buton_menu");
            ButonMenuKucuk = Gorsel("buton_menu_kucuk");
            ButonDevam = Gorsel("buton_devam");
        }

        /// <summary>
        /// PNG dosyasını okur ve hızlı çizim için PArgb biçimine çevirir.
        /// Dosya kilitli kalmasın diye orijinal hemen kapatılır.
        /// </summary>
        private static Bitmap Gorsel(string ad)
        {
            string yol = Path.Combine(Klasor, "Sprites", ad + ".png");
            if (!File.Exists(yol))
                throw new FileNotFoundException("Görsel bulunamadı: " + yol, yol);

            using (var kaynak = new Bitmap(yol))
            {
                var sonuc = new Bitmap(kaynak.Width, kaynak.Height, PixelFormat.Format32bppPArgb);
                using (Graphics g = Graphics.FromImage(sonuc))
                {
                    g.DrawImage(kaynak, new Rectangle(0, 0, kaynak.Width, kaynak.Height));
                }
                return sonuc;
            }
        }

        private static Bitmap Ters(Bitmap kaynak)
        {
            var kopya = (Bitmap)kaynak.Clone();
            kopya.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return kopya;
        }
    }
}
