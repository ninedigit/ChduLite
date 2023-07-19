using System;
using System.Globalization;

namespace NineDigit.ChduLite
{
    /// <summary>
    /// Adresa bloku
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "<Pending>")]
    public struct BlockAddress : IEquatable<BlockAddress>, IComparable<BlockAddress>
    {
        readonly uint value;

        public BlockAddress(uint value)
        {
            this.value = value;
        }

        public static implicit operator BlockAddress(uint value)
            => new BlockAddress(value);

        public static implicit operator uint(BlockAddress amount)
            => amount.value;

        public uint ToUInt32()
            => value;

        public override int GetHashCode()
            => value.GetHashCode();

        public override bool Equals(object obj)
            => obj is BlockAddress && Equals((BlockAddress)obj);

        public bool Equals(BlockAddress other)
            => Equals(this, other);

        public static bool Equals(BlockAddress x, BlockAddress y)
            => x.value == y.value;

        public static bool operator ==(BlockAddress x, BlockAddress y)
            => Equals(x, y);

        public static bool operator !=(BlockAddress x, BlockAddress y)
            => !Equals(x, y);

        public int CompareTo(BlockAddress other)
            => value.CompareTo(other.value);

        public override string ToString()
            => value.ToString(CultureInfo.InvariantCulture);

        public static BlockAddress operator +(BlockAddress left, BlockAddress right)
            => left.value + right.value;

        public static BlockAddress operator +(BlockAddress left, uint right)
            => left.value + right;

        public static BlockAddress operator -(BlockAddress left, BlockAddress right)
            => left.value - right.value;

        public static bool operator <(BlockAddress left, BlockAddress right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(BlockAddress left, BlockAddress right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(BlockAddress left, BlockAddress right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(BlockAddress left, BlockAddress right)
            => left.CompareTo(right) >= 0;
    }
}
