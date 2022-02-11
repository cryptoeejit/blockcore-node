using System.IO;
using Blockcore.Consensus.BlockInfo;
using Chaincoin.Networks.Crypto;
using NBitcoin;
using NBitcoin.Crypto;

namespace Chaincoin.Networks.Consensus
{
   [System.Obsolete]
   public class ChaincoinBlockHeader : BlockHeader
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
            return HashC11.Instance.Hash(ms.ToArray());
         }
      }
   }
}

