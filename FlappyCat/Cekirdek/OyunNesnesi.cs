using System.Drawing;

namespace FlappyCat
{
    /// <summary>
    /// Ekranda konumu olan, her karede güncellenen ve çizilen tüm nesnelerin temel sınıfı.
    /// </summary>
    public abstract class OyunNesnesi
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Genislik { get; protected set; }
        public int Yukseklik { get; protected set; }

        /// <summary>Çarpışma kontrolünde kullanılan dikdörtgen.</summary>
        public virtual RectangleF Sinirlar
        {
            get { return new RectangleF(X, Y, Genislik, Yukseklik); }
        }

        /// <param name="dt">Geçen süre (1 = 60 FPS'te bir kare).</param>
        public abstract void Guncelle(float dt);

        public abstract void Ciz(Graphics g);
    }
}
