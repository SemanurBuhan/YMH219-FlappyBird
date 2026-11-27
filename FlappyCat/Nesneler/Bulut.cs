using System;
using System.Drawing;

namespace FlappyCat
{
    public class Bulut : OyunNesnesi
    {
        private readonly Bitmap gorsel;
        private readonly float saydamlik;
        private readonly float temelHiz;
        private readonly Random rastgele;

        public float HizCarpani { get; set; }

        public Bulut(Bitmap gorsel, float olcek, float saydamlik, float hiz, float x, float y, Random rastgele)
        {
            this.gorsel = gorsel;
            this.saydamlik = saydamlik;
            this.rastgele = rastgele;
            temelHiz = hiz;
            HizCarpani = 1f;
            Genislik = (int)(gorsel.Width * olcek);
            Yukseklik = (int)(gorsel.Height * olcek);
            X = x;
            Y = y;
        }

        public override void Guncelle(float dt)
        {
            X -= temelHiz * HizCarpani * dt;
            if (X + Genislik < -10)
            {
                X = Ayarlar.EkranGenislik + rastgele.Next(20, 160);
                Y = rastgele.Next(40, 430);
            }
        }

        public override void Ciz(Graphics g)
        {
            Cizim.SaydamGorsel(g, gorsel, new RectangleF(X, Y, Genislik, Yukseklik), saydamlik);
        }
    }
}
