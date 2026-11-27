using System.Collections.Generic;
using System.Drawing;

namespace FlappyCat
{
    /// <summary>Ekranın altında kayan bulut zemin. Kedi buna değerse oyun biter.</summary>
    public class Zemin : OyunNesnesi
    {
        private const int Aralik = 64;

        private readonly List<PointF> arkaSira = new List<PointF>();
        private readonly List<PointF> onSira = new List<PointF>();
        private readonly float dongu;

        public float Hiz { get; set; }

        public Zemin()
        {
            X = 0;
            Y = Ayarlar.ZeminY;
            Genislik = Ayarlar.EkranGenislik;
            Yukseklik = Ayarlar.EkranYukseklik - Ayarlar.ZeminY;

            int adet = Ayarlar.EkranGenislik / Aralik + 4;
            dongu = adet * Aralik;
            for (int i = 0; i < adet; i++)
            {
                arkaSira.Add(new PointF(i * Aralik - 40, Ayarlar.ZeminY - 8 + (i % 2) * 7));
                onSira.Add(new PointF(i * Aralik - 70 + Aralik / 2, Ayarlar.ZeminY + 16 + (i % 3) * 5));
            }
        }

        public override void Guncelle(float dt)
        {
            Kaydir(arkaSira, Hiz * 0.8f * dt);
            Kaydir(onSira, Hiz * dt);
        }

        private void Kaydir(List<PointF> sira, float miktar)
        {
            for (int i = 0; i < sira.Count; i++)
            {
                float x = sira[i].X - miktar;
                if (x < -140) x += dongu;
                sira[i] = new PointF(x, sira[i].Y);
            }
        }

        public override void Ciz(Graphics g)
        {
            using (var firca = new SolidBrush(Color.FromArgb(226, 229, 255)))
                g.FillRectangle(firca, 0, Ayarlar.ZeminY + 30, Ayarlar.EkranGenislik, Yukseklik);

            SiraCiz(g, arkaSira, 0.85f, 0);
            SiraCiz(g, onSira, 1f, 4);
        }

        private void SiraCiz(Graphics g, List<PointF> sira, float olcek, int kayma)
        {
            Bitmap[] bulutlar = Varliklar.Bulutlar;
            for (int i = 0; i < sira.Count; i++)
            {
                Bitmap b = bulutlar[(i + kayma) % bulutlar.Length];
                g.DrawImage(b, sira[i].X, sira[i].Y, b.Width * olcek, b.Height * olcek);
            }
        }
    }
}
