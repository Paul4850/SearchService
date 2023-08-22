using System.Runtime.InteropServices;

namespace SearchAPI.Controllers
{
    [StructLayout(LayoutKind.Explicit)]
    public class GuidUnion
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public Guid Guid;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public int Hash1;
        [System.Runtime.InteropServices.FieldOffset(4)]
        public int Hash2;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public int Hash3;
        [System.Runtime.InteropServices.FieldOffset(12)]
        public int Hash4;
    }
}
