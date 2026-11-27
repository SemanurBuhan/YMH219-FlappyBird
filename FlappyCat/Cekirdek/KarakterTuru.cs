namespace FlappyCat
{
    public enum KarakterTuru
    {
        Gri,
        Turuncu,
        Siyah
    }

    public static class KarakterBilgisi
    {
        public static string Ad(KarakterTuru tur)
        {
            switch (tur)
            {
                case KarakterTuru.Turuncu: return "Tarçın";
                case KarakterTuru.Siyah: return "Gece";
                default: return "Bulut";
            }
        }

        public static string DosyaEki(KarakterTuru tur)
        {
            switch (tur)
            {
                case KarakterTuru.Turuncu: return "turuncu";
                case KarakterTuru.Siyah: return "siyah";
                default: return "gri";
            }
        }
    }
}
