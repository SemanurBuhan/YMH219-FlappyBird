using System.Drawing;

namespace FlappyCat
{
    public abstract class OyunNesnesi
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Genislik { get; protected set; }
        public int Yukseklik { get; protected set; }

        public virtual RectangleF Sinirlar
        {
            get { return new RectangleF(X, Y, Genislik, Yukseklik); }
        }

        public abstract void Guncelle(float dt);

        public abstract void Ciz(Graphics g);
    }
}
