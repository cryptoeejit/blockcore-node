using System.IO;
using Blockcore.Consensus.BlockInfo;
using Dash.Networks.Crypto;
using NBitcoin;
using NBitcoin.Crypto;

namespace Dash.Networks.Consensus
{
   [System.Obsolete]
   public class DashBlockHeader : BlockHeader
   {
      public override int CurrentVersion => 1;

      /// <inheritdoc />
      public override uint256 GetHash()
      {
         uint256 hash = null;
         uint256[] innerHashes = hashes;

         if (innerHashes != null)
            hash = innerHashes[0];

         //if (hash != null)
         //   return hash;

         hash = GetPoWHash();

         innerHashes = hashes;
         if (innerHashes != null)
         {
            innerHashes[0] = hash;
         }

         return hash;
      }

      /// <inheritdoc />
      public override uint256 GetPoWHash()
      {
         using (var ms = new MemoryStream())
         {
            ReadWriteHashingStream(new BitcoinStream(ms, true));
            return HashX11.Instance.Hash(ms.ToArray());
         }
      }
   }
}
//#pragma warning disable CS0618 // Type or member is obsolete
//   public class DashBlockHeader : BlockHeader
//   {
//      // https://github.com/dashpay/dash/blob/e596762ca22d703a79c6880a9d3edb1c7c972fd3/src/primitives/block.cpp#L13
//      static byte[] CalculateHash(byte[] data, int offset, int count)
//      {
//         return new HashX11.X11().ComputeBytes(data.Skip(offset).Take(count).ToArray());
//      }

//      protected override HashStreamBase CreateHashStream()
//      {
//         return BufferedHashStream.CreateFrom(CalculateHash);
//      }
//   }
}
