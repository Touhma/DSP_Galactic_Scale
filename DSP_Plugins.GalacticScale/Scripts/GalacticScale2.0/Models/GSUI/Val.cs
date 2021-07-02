namespace GalacticScale
{
    public class Val
    {
        public object val;
        public string String()
        {
            return val.ToString();
        }
        public int Int(int def = -1)
        {
            if (int.TryParse(ToString(), out int i)) return i;
            GS2.Error("Failed to parse int");
            return def;
        }
        public float Float(float def = -1f)
        {
            if (float.TryParse(ToString(), out float f)) return f;
            if (double.TryParse(ToString(), out double d)) return (float)d;
            if (int.TryParse(ToString(), out int i)) return (float)i;
            GS2.Error("Failed to parse float "+val.ToString());
            return def;
        }
        public double Double(double def = -1.0)
        {
            if (double.TryParse(ToString(), out double i)) return i;
            GS2.Error("Failed to parse double");
            return def;
        }
        public bool Bool(bool def = false)
        {
            if (bool.TryParse(ToString(), out bool i)) return i;
            GS2.Error("Failed to parse bool");
            return def;
        }
        public GSSliderConfig GSSliderConfig(GSSliderConfig def = new GSSliderConfig())
        {
            if (val is GSSliderConfig) return (GSSliderConfig)val;
            GS2.Error("Failed to parse GSSliderConfig");
            return def;
        }
        public Val (object o)
        {
            val = o;
        }
        public bool IsNull()
        {
            return (val == null);
        }
        public bool empty { get => IsNull(); }
        public static implicit operator Val(int i) => new Val(i);
        public static implicit operator Val(float i) => new Val(i);
        public static implicit operator Val(double i) => new Val(i);
        public static implicit operator Val(bool i) => new Val(i);
        public static implicit operator Val(string i) => new Val(i);
        public static implicit operator int(Val v) => v.Int();
        public static implicit operator float(Val v) => v.Float();
        public static implicit operator bool(Val v) => v.Bool();
        public static implicit operator string(Val v) => v.String();
        public static implicit operator double(Val v) => v.Double();
        public static implicit operator GSSliderConfig(Val v) => v.GSSliderConfig();
        public static implicit operator Val(GSSliderConfig g) => new Val(g);
        public static bool operator ==(Val left, object right)
        {
            return (left.val == right);
        }

        public static bool operator !=(Val left, object right)
        {
            return (left.val != right);
        }
        public override int GetHashCode()
        {
            return val.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return val.Equals(obj);
        }
        public override string ToString()
        {
            if (val != null) return val.ToString();
            return null;
        }
    }
}